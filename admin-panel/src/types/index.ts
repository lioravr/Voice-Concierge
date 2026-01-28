/**
 * TypeScript type definitions for Voice Concierge Admin Panel
 */

// FAQ Types
export interface FAQ {
  id: string;
  question: string;
  answer: string;
  category: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface CreateFAQRequest {
  question: string;
  answer: string;
  category?: string;
}

export interface UpdateFAQRequest {
  question: string;
  answer: string;
  category?: string;
}

export interface FAQSearchRequest {
  query: string;
  limit?: number;
  threshold?: number;
}

export interface FAQSearchResult {
  faq: FAQ;
  distance: number;
  similarity: number;
}

// Unanswered Question Types
export interface UnansweredQuestion {
  id: string;
  question: string;
  frequency: number;
  firstAskedAt: string;
  lastAskedAt: string;
  status: 'pending' | 'converted' | 'dismissed';
}

export interface RecordQuestionRequest {
  question: string;
}

export interface ConvertToFAQRequest {
  answer: string;
  category?: string;
}

// Voice Configuration Types
export interface VoiceConfiguration {
  id: string;
  voiceId: number;
  name: string;
  description: string | null;
  gender: string | null;
  accent: string | null;
  providerVoiceId: string | null;
  isActive: boolean;
  createdAt: string;
}

// API Response Types
export interface ApiError {
  message: string;
  errors?: Record<string, string[]>;
}

// Pagination Types (for future use)
export interface PaginatedResponse<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
}
