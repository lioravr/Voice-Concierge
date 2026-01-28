/**
 * React Query hooks for Unanswered Questions management
 */
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { unansweredQuestionsApi, handleApiError } from '../lib/api';
import type { ConvertToFAQRequest } from '../types';
import { toast } from 'sonner';

export const useUnansweredQuestions = () => {
  return useQuery({
    queryKey: ['unanswered-questions'],
    queryFn: unansweredQuestionsApi.getAllPending,
    refetchInterval: 30000, // Refetch every 30 seconds
  });
};

export const useConvertToFAQ = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: ConvertToFAQRequest }) =>
      unansweredQuestionsApi.convertToFAQ(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['unanswered-questions'] });
      queryClient.invalidateQueries({ queryKey: ['faqs'] });
      toast.success('Question converted to FAQ successfully');
    },
    onError: (error) => {
      toast.error(handleApiError(error));
    },
  });
};

export const useDismissQuestion = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => unansweredQuestionsApi.dismiss(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['unanswered-questions'] });
      toast.success('Question dismissed');
    },
    onError: (error) => {
      toast.error(handleApiError(error));
    },
  });
};
