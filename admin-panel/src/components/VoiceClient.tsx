/**
 * LiveKit Voice Client Component
 * Provides real-time voice conversation with the agent
 */
import { useState, useEffect } from 'react';
import { Room } from 'livekit-client';
import { Mic, MicOff, Phone, PhoneOff, Volume2, VolumeX } from 'lucide-react';
import axios from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';

interface VoiceClientProps {
  onConnectionChange?: (connected: boolean) => void;
}

export default function VoiceClient({ onConnectionChange }: VoiceClientProps) {
  const [room] = useState(() => new Room());
  const [connected, setConnected] = useState(false);
  const [connecting, setConnecting] = useState(false);
  const [micEnabled, setMicEnabled] = useState(true);
  const [speakerEnabled, setSpeakerEnabled] = useState(true);
  const [transcript, setTranscript] = useState<string[]>([]);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    return () => {
      room.disconnect();
    };
  }, [room]);

  const connect = async () => {
    try {
      setConnecting(true);
      setError(null);
      
      setTranscript((prev) => [...prev, `[${new Date().toLocaleTimeString()}] Getting token...`]);

      const roomName = `voice-concierge-${Date.now()}`;
      const response = await axios.post(`${API_BASE_URL}/livekit/token`, {
        identity: `guest-${Date.now()}`,
        roomName: roomName,
      });

      const { token, url } = response.data;
      setTranscript((prev) => [...prev, `[${new Date().toLocaleTimeString()}] Token received, connecting...`]);

      await room.connect(url, token);
      
      setConnected(true);
      setConnecting(false);
      onConnectionChange?.(true);

      setTranscript((prev) => [
        ...prev,
        `[${new Date().toLocaleTimeString()}] ✅ Connected! Enabling microphone...`,
      ]);

      await room.localParticipant.setMicrophoneEnabled(true);
      
      setTranscript((prev) => [
        ...prev,
        `[${new Date().toLocaleTimeString()}] 🎤 Microphone enabled - Start speaking!`,
      ]);

      room.on('trackSubscribed', (track, publication, participant) => {
        if (track.kind === 'audio') {
          const audioElement = track.attach();
          audioElement.autoplay = true;
          audioElement.playsInline = true;
          document.body.appendChild(audioElement);
          
          setTranscript((prev) => [
            ...prev,
            `[${new Date().toLocaleTimeString()}] 🔊 Agent is speaking...`,
          ]);
        }
      });

      room.on('dataReceived', (payload) => {
        const decoder = new TextDecoder();
        const message = decoder.decode(payload);
        setTranscript((prev) => [
          ...prev,
          `[${new Date().toLocaleTimeString()}] Agent: ${message}`,
        ]);
      });
      
      room.on('participantConnected', (participant) => {
        setTranscript((prev) => [
          ...prev,
          `[${new Date().toLocaleTimeString()}] 👤 ${participant.identity} joined`,
        ]);
      });
      
    } catch (err: any) {
      const errorMsg = err.response?.data?.error || err.message || 'Failed to connect to voice agent';
      setError(`❌ Error: ${errorMsg}`);
      setTranscript((prev) => [
        ...prev,
        `[${new Date().toLocaleTimeString()}] ❌ Connection failed: ${errorMsg}`,
      ]);
      setConnecting(false);
      setConnected(false);
      onConnectionChange?.(false);
    }
  };

  const disconnect = async () => {
    // Clean up audio elements
    document.querySelectorAll('audio').forEach((audio) => {
      audio.pause();
      audio.remove();
    });
    
    await room.disconnect();
    setConnected(false);
    setConnecting(false);
    onConnectionChange?.(false);
    setTranscript((prev) => [
      ...prev,
      `[${new Date().toLocaleTimeString()}] Disconnected from voice agent`,
    ]);
  };

  const toggleMic = async () => {
    const enabled = !micEnabled;
    await room.localParticipant.setMicrophoneEnabled(enabled);
    setMicEnabled(enabled);
    setTranscript((prev) => [
      ...prev,
      `[${new Date().toLocaleTimeString()}] Microphone ${enabled ? 'enabled' : 'muted'}`,
    ]);
  };

  const toggleSpeaker = () => {
    const enabled = !speakerEnabled;
    setSpeakerEnabled(enabled);
    // Mute/unmute all remote audio tracks
    room.remoteParticipants.forEach((participant: any) => {
      participant.audioTracks.forEach((publication: any) => {
        if (publication.track) {
          publication.track.setVolume(enabled ? 1 : 0);
        }
      });
    });
    setTranscript((prev) => [
      ...prev,
      `[${new Date().toLocaleTimeString()}] Speaker ${enabled ? 'enabled' : 'muted'}`,
    ]);
  };

  return (
    <div className="bg-white rounded-lg shadow-lg p-6">
      <div className="flex items-center justify-between mb-6">
        <h3 className="text-xl font-semibold text-gray-900">Voice Agent</h3>
        <div className="flex items-center space-x-2">
          {connected && (
            <span className="flex items-center text-sm text-green-600">
              <span className="w-2 h-2 bg-green-500 rounded-full mr-2 animate-pulse"></span>
              Connected
            </span>
          )}
          {connecting && (
            <span className="text-sm text-blue-600">Connecting...</span>
          )}
        </div>
      </div>

      {error && (
        <div className="mb-4 p-4 bg-red-50 border border-red-200 rounded-lg">
          <p className="text-sm text-red-800">{error}</p>
        </div>
      )}

      {/* Control Buttons */}
      <div className="flex items-center justify-center space-x-4 mb-6">
        {!connected ? (
          <button
            onClick={connect}
            disabled={connecting}
            className="flex items-center px-6 py-3 bg-green-600 text-white font-semibold rounded-lg hover:bg-green-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
          >
            <Phone className="w-5 h-5 mr-2" />
            {connecting ? 'Connecting...' : 'Connect'}
          </button>
        ) : (
          <>
            <button
              onClick={toggleMic}
              className={`p-4 rounded-full transition-colors ${
                micEnabled
                  ? 'bg-blue-600 hover:bg-blue-700 text-white'
                  : 'bg-red-600 hover:bg-red-700 text-white'
              }`}
              title={micEnabled ? 'Mute microphone' : 'Unmute microphone'}
            >
              {micEnabled ? (
                <Mic className="w-6 h-6" />
              ) : (
                <MicOff className="w-6 h-6" />
              )}
            </button>

            <button
              onClick={toggleSpeaker}
              className={`p-4 rounded-full transition-colors ${
                speakerEnabled
                  ? 'bg-blue-600 hover:bg-blue-700 text-white'
                  : 'bg-gray-600 hover:bg-gray-700 text-white'
              }`}
              title={speakerEnabled ? 'Mute speaker' : 'Unmute speaker'}
            >
              {speakerEnabled ? (
                <Volume2 className="w-6 h-6" />
              ) : (
                <VolumeX className="w-6 h-6" />
              )}
            </button>

            <button
              onClick={disconnect}
              className="flex items-center px-6 py-3 bg-red-600 text-white font-semibold rounded-lg hover:bg-red-700 transition-colors"
            >
              <PhoneOff className="w-5 h-5 mr-2" />
              Disconnect
            </button>
          </>
        )}
      </div>

      {/* Transcript - Always show after first connection attempt */}
      {transcript.length > 0 && (
        <div className="bg-gray-50 rounded-lg p-4 max-h-64 overflow-y-auto">
          <h4 className="text-sm font-semibold text-gray-700 mb-2">Connection Log</h4>
          <div className="space-y-1">
            {transcript.map((entry, index) => (
              <p key={index} className="text-sm text-gray-600 font-mono">
                {entry}
              </p>
            ))}
          </div>
        </div>
      )}
      
      {/* Speaking Instructions */}
      {connected && (
        <div className="mt-4 p-4 bg-green-50 border-2 border-green-300 rounded-lg">
          <h4 className="text-sm font-bold text-green-900 mb-2 flex items-center">
            <Mic className="w-4 h-4 mr-2" />
            🎉 Ready! You can now speak:
          </h4>
          <ul className="text-sm text-green-800 space-y-1 list-disc list-inside">
            <li>Say: "What time is check-in?"</li>
            <li>Say: "Do you have a pool?"</li>
            <li>Say: "Tell me about the spa"</li>
          </ul>
          <p className="text-xs text-green-700 mt-2">
            💡 The agent will hear you and respond with voice
          </p>
        </div>
      )}

      {/* Instructions */}
      {!connected && !connecting && (
        <div className="mt-6 p-4 bg-blue-50 border border-blue-200 rounded-lg">
          <h4 className="text-sm font-semibold text-blue-900 mb-2">How to use:</h4>
          <ol className="text-sm text-blue-800 space-y-1 list-decimal list-inside">
            <li>Click "Connect" to join the voice conversation</li>
            <li>Allow microphone access when prompted</li>
            <li>Start speaking - the agent will respond to your questions</li>
            <li>Use the controls to mute/unmute mic or speaker</li>
            <li>Click "Disconnect" when you're done</li>
          </ol>
        </div>
      )}
    </div>
  );
}
