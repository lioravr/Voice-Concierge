/**
 * React Query hooks for FAQ management
 */
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { faqApi, handleApiError } from '../lib/api';
import type { CreateFAQRequest, UpdateFAQRequest, FAQSearchRequest } from '../types';
import { toast } from 'sonner';

export const useFAQs = () => {
  return useQuery({
    queryKey: ['faqs'],
    queryFn: faqApi.getAll,
  });
};

export const useFAQ = (id: string) => {
  return useQuery({
    queryKey: ['faqs', id],
    queryFn: () => faqApi.getById(id),
    enabled: !!id,
  });
};

export const useFAQSearch = (request: FAQSearchRequest) => {
  return useQuery({
    queryKey: ['faqs', 'search', request.query],
    queryFn: () => faqApi.search(request),
    enabled: request.query.length > 0,
  });
};

export const useCreateFAQ = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: CreateFAQRequest) => faqApi.create(request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['faqs'] });
      toast.success('FAQ created successfully');
    },
    onError: (error) => {
      toast.error(handleApiError(error));
    },
  });
};

export const useUpdateFAQ = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateFAQRequest }) =>
      faqApi.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['faqs'] });
      toast.success('FAQ updated successfully');
    },
    onError: (error) => {
      toast.error(handleApiError(error));
    },
  });
};

export const useDeleteFAQ = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => faqApi.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['faqs'] });
      toast.success('FAQ deleted successfully');
    },
    onError: (error) => {
      toast.error(handleApiError(error));
    },
  });
};
