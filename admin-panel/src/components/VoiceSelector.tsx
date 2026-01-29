/**
 * Voice Selector Component
 * Allows users to choose their preferred voice speaker
 */
import { useState, useEffect } from 'react';
import { Volume2, Check } from 'lucide-react';
import { useVoiceConfigurations } from '../hooks/useVoiceConfigurations';
import axios from 'axios';
import { toast } from 'sonner';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';

interface VoiceSelectorProps {
  onVoiceSelect?: (voiceId: number) => void;
  className?: string;
}

export default function VoiceSelector({ onVoiceSelect, className = '' }: VoiceSelectorProps) {
  const { data: voices, isLoading } = useVoiceConfigurations();
  const [selectedVoiceId, setSelectedVoiceId] = useState<number | null>(null);
  const [playingVoiceId, setPlayingVoiceId] = useState<number | null>(null);

  // Load saved preference on mount
  useEffect(() => {
    const savedVoiceId = localStorage.getItem('preferred_voice_id');
    if (savedVoiceId) {
      const voiceId = parseInt(savedVoiceId, 10);
      setSelectedVoiceId(voiceId);
      onVoiceSelect?.(voiceId);
    } else if (voices && voices.length > 0) {
      // Default to first voice if none selected
      const defaultVoiceId = voices[0].voiceId;
      setSelectedVoiceId(defaultVoiceId);
      localStorage.setItem('preferred_voice_id', defaultVoiceId.toString());
      onVoiceSelect?.(defaultVoiceId);
    }
  }, [voices, onVoiceSelect]);

  const handleSelectVoice = (voiceId: number) => {
    setSelectedVoiceId(voiceId);
    localStorage.setItem('preferred_voice_id', voiceId.toString());
    onVoiceSelect?.(voiceId);
    toast.success('Voice preference saved');
  };

  const handlePreview = async (voiceId: number, e: React.MouseEvent) => {
    e.stopPropagation();
    
    try {
      setPlayingVoiceId(voiceId);
      
      const response = await axios.get(
        `${API_BASE_URL}/voiceconfigurations/${voiceId}/preview`,
        { responseType: 'blob' }
      );

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
    } catch (error) {
      setPlayingVoiceId(null);
      toast.error('Failed to generate voice preview');
    }
  };

  if (isLoading) {
    return (
      <div className={`bg-white rounded-lg shadow p-6 ${className}`}>
        <h3 className="text-lg font-semibold text-gray-900 mb-4">Choose Your Voice</h3>
        <p className="text-gray-600">Loading voices...</p>
      </div>
    );
  }

  if (!voices || voices.length === 0) {
    return null;
  }

  return (
    <div className={`bg-white rounded-lg shadow p-6 ${className}`}>
      <div className="flex items-center justify-between mb-4">
        <h3 className="text-lg font-semibold text-gray-900">Choose Your Voice</h3>
        <p className="text-sm text-gray-500">
          {selectedVoiceId ? 'Selected' : 'Select a voice'}
        </p>
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
        {voices.map((voice) => {
          const isSelected = selectedVoiceId === voice.voiceId;
          const isPlaying = playingVoiceId === voice.voiceId;

          return (
            <div
              key={voice.voiceId}
              onClick={() => handleSelectVoice(voice.voiceId)}
              className={`
                relative p-4 rounded-lg border-2 cursor-pointer transition-all
                ${isSelected 
                  ? 'border-indigo-600 bg-indigo-50' 
                  : 'border-gray-200 hover:border-indigo-300 bg-white'
                }
              `}
            >
              {/* Selection Indicator */}
              {isSelected && (
                <div className="absolute top-2 right-2">
                  <div className="w-6 h-6 bg-indigo-600 rounded-full flex items-center justify-center">
                    <Check className="w-4 h-4 text-white" />
                  </div>
                </div>
              )}

              {/* Voice Info */}
              <div className="mb-3">
                <h4 className="font-semibold text-gray-900 mb-1">{voice.name}</h4>
                <p className="text-sm text-gray-600">{voice.description}</p>
              </div>

              {/* Preview Button */}
              <button
                onClick={(e) => handlePreview(voice.voiceId, e)}
                disabled={isPlaying}
                className={`
                  w-full px-3 py-2 rounded-md text-sm font-medium transition-colors flex items-center justify-center
                  ${isSelected
                    ? 'bg-indigo-600 text-white hover:bg-indigo-700'
                    : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                  }
                  disabled:opacity-50 disabled:cursor-not-allowed
                `}
              >
                <Volume2 className={`w-4 h-4 mr-2 ${isPlaying ? 'animate-pulse' : ''}`} />
                {isPlaying ? 'Playing...' : 'Preview'}
              </button>
            </div>
          );
        })}
      </div>

      {selectedVoiceId && (
        <div className="mt-4 p-3 bg-blue-50 rounded-lg">
          <p className="text-sm text-blue-800">
            ✓ Your voice preference has been saved and will be used for all voice conversations.
          </p>
        </div>
      )}
    </div>
  );
}
