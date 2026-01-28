/**
 * React Query hooks for Voice Configuration management
 */
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { voiceConfigurationApi, handleApiError } from '../lib/api';
import { toast } from 'sonner';

export const useVoiceConfigurations = () => {
  return useQuery({
    queryKey: ['voice-configurations'],
    queryFn: voiceConfigurationApi.getAll,
  });
};

export const useActiveVoice = () => {
  return useQuery({
    queryKey: ['voice-configurations', 'active'],
    queryFn: voiceConfigurationApi.getActive,
  });
};

export const useSetActiveVoice = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (voiceId: number) => voiceConfigurationApi.setActive(voiceId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['voice-configurations'] });
      toast.success('Active voice updated successfully');
    },
    onError: (error) => {
      toast.error(handleApiError(error));
    },
  });
};
