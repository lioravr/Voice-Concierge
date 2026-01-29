/**
 * Voice Configuration Management Page
 */
import { useState } from 'react';
import { useVoiceConfigurations, useActiveVoice, useSetActiveVoice } from '../hooks/useVoiceConfigurations';
import { CheckCircle, Volume2 } from 'lucide-react';
import axios from 'axios';
import { toast } from 'sonner';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/api';

export default function VoiceConfigurationPage() {
  const { data: voices, isLoading } = useVoiceConfigurations();
  const { data: activeVoice } = useActiveVoice();
  const setActiveVoice = useSetActiveVoice();
  const [playingVoiceId, setPlayingVoiceId] = useState<number | null>(null);

  const handleActivate = async (voiceId: number) => {
    await setActiveVoice.mutateAsync(voiceId);
  };

  const handlePreview = async (voiceId: number) => {
    try {
      setPlayingVoiceId(voiceId);
      toast.info('Generating voice preview...');

      // Fetch audio preview from backend
      const response = await axios.get(
        `${API_BASE_URL}/voiceconfigurations/${voiceId}/preview`,
        { responseType: 'blob' }
      );

      // Create audio blob and play
      const audioBlob = new Blob([response.data], { type: 'audio/mpeg' });
      const audioUrl = URL.createObjectURL(audioBlob);
      const audio = new Audio(audioUrl);

      audio.onended = () => {
        setPlayingVoiceId(null);
        URL.revokeObjectURL(audioUrl);
      };

      audio.onerror = () => {
        setPlayingVoiceId(null);
        URL.revokeObjectURL(audioUrl);
        toast.error('Failed to play audio preview');
      };

      await audio.play();
      toast.success('Playing voice preview');
    } catch (error) {
      console.error('Failed to preview voice:', error);
      setPlayingVoiceId(null);
      toast.error('Failed to generate voice preview');
    }
  };

  if (isLoading) {
    return <div className="text-center py-12">Loading voice configurations...</div>;
  }

  return (
    <div>
      <div className="mb-6">
        <h2 className="text-3xl font-bold text-gray-900">Voice Configuration</h2>
        <p className="text-gray-600 mt-2">
          Select the active voice personality for the voice concierge.
        </p>
      </div>

      {/* Current Active Voice */}
      {activeVoice && (
        <div className="bg-blue-50 border-2 border-blue-200 rounded-lg p-6 mb-8">
          <div className="flex items-center mb-2">
            <CheckCircle className="w-6 h-6 text-blue-600 mr-2" />
            <h3 className="text-lg font-semibold text-blue-900">Currently Active Voice</h3>
          </div>
          <p className="text-2xl font-bold text-blue-900">{activeVoice.name}</p>
          <p className="text-blue-700 mt-1">{activeVoice.description}</p>
        </div>
      )}

      {/* Voice Options */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        {voices?.map((voice) => (
          <div
            key={voice.id}
            className={`
              bg-white rounded-lg shadow p-6 transition-all
              ${voice.isActive ? 'ring-2 ring-blue-500' : 'hover:shadow-lg'}
            `}
          >
            <div className="flex justify-between items-start mb-4">
              <div>
                <h3 className="text-xl font-bold text-gray-900">{voice.name}</h3>
                <div className="flex items-center space-x-2 mt-2">
                  {voice.gender && (
                    <span className="inline-block px-2 py-1 text-xs font-semibold text-gray-700 bg-gray-100 rounded-full">
                      {voice.gender}
                    </span>
                  )}
                  {voice.accent && (
                    <span className="inline-block px-2 py-1 text-xs font-semibold text-gray-700 bg-gray-100 rounded-full">
                      {voice.accent}
                    </span>
                  )}
                </div>
              </div>
              {voice.isActive && (
                <span className="inline-flex items-center px-3 py-1 text-sm font-semibold text-blue-800 bg-blue-100 rounded-full">
                  <CheckCircle className="w-4 h-4 mr-1" />
                  Active
                </span>
              )}
            </div>

            {voice.description && (
              <p className="text-gray-600 mb-4">{voice.description}</p>
            )}

            <div className="mb-4">
              <p className="text-sm text-gray-500">
                OpenAI Voice: <span className="font-mono font-semibold">{voice.providerVoiceId}</span>
              </p>
            </div>

            {/* Preview Button */}
            <button
              onClick={() => handlePreview(voice.voiceId)}
              disabled={playingVoiceId !== null}
              className="w-full mb-3 px-4 py-2 bg-gray-100 text-gray-800 rounded-lg hover:bg-gray-200 transition-colors disabled:opacity-50 flex items-center justify-center"
            >
              <Volume2 className={`w-4 h-4 mr-2 ${playingVoiceId === voice.voiceId ? 'animate-pulse' : ''}`} />
              {playingVoiceId === voice.voiceId ? 'Playing Preview...' : 'Preview Voice'}
            </button>

            {/* Activate Button */}
            {!voice.isActive && (
              <button
                onClick={() => handleActivate(voice.voiceId)}
                disabled={setActiveVoice.isPending}
                className="w-full px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors disabled:opacity-50"
              >
                Activate This Voice
              </button>
            )}

            {voice.isActive && (
              <div className="w-full px-4 py-2 bg-blue-100 text-blue-700 font-semibold text-center rounded-lg">
                Currently Active
              </div>
            )}
          </div>
        ))}
      </div>

      {/* Voice Preview Information */}
      <div className="mt-8 bg-gray-50 rounded-lg p-6">
        <h3 className="text-lg font-semibold text-gray-900 mb-3">About Voice Personalities</h3>
        <div className="space-y-2 text-sm text-gray-600">
          <p>
            • Each voice has a unique personality, accent, and tone
          </p>
          <p>
            • Click "Preview Voice" to hear a sample of each voice before activating
          </p>
          <p>
            • The active voice is used for all guest interactions via the voice agent
          </p>
          <p>
            • Changes take effect immediately for new conversations
          </p>
          <p>
            • You can also test voices in the Playground with full conversations
          </p>
        </div>
      </div>
    </div>
  );
}
