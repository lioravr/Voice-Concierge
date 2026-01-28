/**
 * API client for Voice Concierge backend
 */
import axios, { AxiosError } from 'axios';
import type {
  FAQ,
  CreateFAQRequest,
  UpdateFAQRequest,
  FAQSearchRequest,
  FAQSearchResult,
  UnansweredQuestion,
  RecordQuestionRequest,
  ConvertToFAQRequest,
  VoiceConfiguration,
  ApiError,
} from '../types';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Error handling helper
export const handleApiError = (error: unknown): string => {
  if (axios.isAxiosError(error)) {
    const axiosError = error as AxiosError<ApiError>;
    return axiosError.response?.data?.message || axiosError.message || 'An unknown error occurred';
  }
  return 'An unknown error occurred';
};

// ==================== FAQ API ====================

export const faqApi = {
  getAll: async (): Promise<FAQ[]> => {
    const response = await api.get<FAQ[]>('/faq');
    return response.data;
  },

  getById: async (id: string): Promise<FAQ> => {
    const response = await api.get<FAQ>(`/faq/${id}`);
    return response.data;
  },

  search: async (request: FAQSearchRequest): Promise<FAQSearchResult[]> => {
    const response = await api.post<FAQSearchResult[]>('/faq/search', request);
    return response.data;
  },

  create: async (request: CreateFAQRequest): Promise<FAQ> => {
    const response = await api.post<FAQ>('/faq', request);
    return response.data;
  },

  update: async (id: string, request: UpdateFAQRequest): Promise<FAQ> => {
    const response = await api.put<FAQ>(`/faq/${id}`, request);
    return response.data;
  },

  delete: async (id: string): Promise<void> => {
    await api.delete(`/faq/${id}`);
  },
};

// ==================== Unanswered Questions API ====================

export const unansweredQuestionsApi = {
  getAllPending: async (): Promise<UnansweredQuestion[]> => {
    const response = await api.get<UnansweredQuestion[]>('/unansweredquestions');
    return response.data;
  },

  getById: async (id: string): Promise<UnansweredQuestion> => {
    const response = await api.get<UnansweredQuestion>(`/unansweredquestions/${id}`);
    return response.data;
  },

  record: async (request: RecordQuestionRequest): Promise<UnansweredQuestion> => {
    const response = await api.post<UnansweredQuestion>('/unansweredquestions', request);
    return response.data;
  },

  convertToFAQ: async (id: string, request: ConvertToFAQRequest): Promise<FAQ> => {
    const response = await api.post<FAQ>(`/unansweredquestions/${id}/convert`, request);
    return response.data;
  },

  dismiss: async (id: string): Promise<void> => {
    await api.delete(`/unansweredquestions/${id}`);
  },
};

// ==================== Voice Configuration API ====================

export const voiceConfigurationApi = {
  getAll: async (): Promise<VoiceConfiguration[]> => {
    const response = await api.get<VoiceConfiguration[]>('/voiceconfigurations');
    return response.data;
  },

  getActive: async (): Promise<VoiceConfiguration> => {
    const response = await api.get<VoiceConfiguration>('/voiceconfigurations/active');
    return response.data;
  },

  getByVoiceId: async (voiceId: number): Promise<VoiceConfiguration> => {
    const response = await api.get<VoiceConfiguration>(`/voiceconfigurations/${voiceId}`);
    return response.data;
  },

  setActive: async (voiceId: number): Promise<void> => {
    await api.put(`/voiceconfigurations/${voiceId}/activate`);
  },
};

// ==================== Health Check ====================

export const healthApi = {
  check: async (): Promise<boolean> => {
    try {
      const response = await api.get('/test/db-connection');
      return response.status === 200;
    } catch {
      return false;
    }
  },
};

export default api;
