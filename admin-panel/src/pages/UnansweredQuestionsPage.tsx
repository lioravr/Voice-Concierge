/**
 * Unanswered Questions Management Page
 */
import { useState } from 'react';
import { CheckCircle, XCircle } from 'lucide-react';
import { useUnansweredQuestions, useConvertToFAQ, useDismissQuestion } from '../hooks/useUnansweredQuestions';
import type { UnansweredQuestion, ConvertToFAQRequest } from '../types';

export default function UnansweredQuestionsPage() {
  const { data: questions, isLoading } = useUnansweredQuestions();
  const convertToFAQ = useConvertToFAQ();
  const dismissQuestion = useDismissQuestion();

  const [convertingQuestion, setConvertingQuestion] = useState<UnansweredQuestion | null>(null);
  const [formData, setFormData] = useState<ConvertToFAQRequest>({
    answer: '',
    category: '',
  });

  const handleConvert = async (e: React.FormEvent) => {
    e.preventDefault();
    if (convertingQuestion) {
      await convertToFAQ.mutateAsync({ id: convertingQuestion.id, data: formData });
      handleCloseModal();
    }
  };

  const handleCloseModal = () => {
    setConvertingQuestion(null);
    setFormData({ answer: '', category: '' });
  };

  const handleDismiss = async (id: string) => {
    if (window.confirm('Are you sure you want to dismiss this question?')) {
      await dismissQuestion.mutateAsync(id);
    }
  };

  if (isLoading) {
    return <div className="text-center py-12">Loading questions...</div>;
  }

  const sortedQuestions = [...(questions || [])].sort((a, b) => b.frequency - a.frequency);

  return (
    <div>
      <div className="mb-6">
        <h2 className="text-3xl font-bold text-gray-900">Unanswered Questions</h2>
        <p className="text-gray-600 mt-2">
          Questions that guests asked but the system couldn't answer. Convert them to FAQs or dismiss them.
        </p>
      </div>

      {/* Statistics */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-8">
        <div className="bg-white rounded-lg shadow p-6">
          <div className="text-2xl font-bold text-gray-900">{questions?.length || 0}</div>
          <div className="text-sm text-gray-600">Pending Questions</div>
        </div>
        <div className="bg-white rounded-lg shadow p-6">
          <div className="text-2xl font-bold text-gray-900">
            {questions?.reduce((sum, q) => sum + q.frequency, 0) || 0}
          </div>
          <div className="text-sm text-gray-600">Total Asks</div>
        </div>
        <div className="bg-white rounded-lg shadow p-6">
          <div className="text-2xl font-bold text-gray-900">
            {questions && questions.length > 0
              ? Math.round(questions.reduce((sum, q) => sum + q.frequency, 0) / questions.length)
              : 0}
          </div>
          <div className="text-sm text-gray-600">Avg. Frequency</div>
        </div>
      </div>

      {/* Questions List */}
      <div className="space-y-4">
        {sortedQuestions.map((question) => (
          <div key={question.id} className="bg-white rounded-lg shadow p-6">
            <div className="flex justify-between items-start">
              <div className="flex-1">
                <div className="flex items-center mb-2">
                  <span className="inline-block px-3 py-1 text-sm font-semibold text-red-800 bg-red-100 rounded-full">
                    Asked {question.frequency} {question.frequency === 1 ? 'time' : 'times'}
                  </span>
                </div>
                <h3 className="text-lg font-semibold text-gray-900 mb-2">
                  {question.question}
                </h3>
                <div className="text-sm text-gray-500">
                  First asked: {new Date(question.firstAskedAt).toLocaleString()}
                  <br />
                  Last asked: {new Date(question.lastAskedAt).toLocaleString()}
                </div>
              </div>
              <div className="flex space-x-2 ml-4">
                <button
                  onClick={() => setConvertingQuestion(question)}
                  className="flex items-center px-4 py-2 text-green-700 bg-green-50 hover:bg-green-100 rounded-lg transition-colors"
                >
                  <CheckCircle className="w-5 h-5 mr-1" />
                  Convert to FAQ
                </button>
                <button
                  onClick={() => handleDismiss(question.id)}
                  className="flex items-center px-4 py-2 text-red-700 bg-red-50 hover:bg-red-100 rounded-lg transition-colors"
                >
                  <XCircle className="w-5 h-5 mr-1" />
                  Dismiss
                </button>
              </div>
            </div>
          </div>
        ))}

        {sortedQuestions.length === 0 && (
          <div className="text-center py-12 text-gray-500">
            No unanswered questions yet. Great job! 🎉
          </div>
        )}
      </div>

      {/* Convert Modal */}
      {convertingQuestion && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
          <div className="bg-white rounded-lg max-w-2xl w-full p-6">
            <h3 className="text-2xl font-bold mb-4">Convert to FAQ</h3>
            <div className="mb-4 p-4 bg-gray-50 rounded-lg">
              <p className="text-sm text-gray-600 mb-1">Question:</p>
              <p className="font-semibold text-gray-900">{convertingQuestion.question}</p>
            </div>
            <form onSubmit={handleConvert} className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Category (optional)
                </label>
                <input
                  type="text"
                  value={formData.category}
                  onChange={(e) => setFormData({ ...formData, category: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  placeholder="e.g., Gaming, Dining, Accommodations"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Answer *
                </label>
                <textarea
                  required
                  rows={4}
                  value={formData.answer}
                  onChange={(e) => setFormData({ ...formData, answer: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  placeholder="Provide a helpful answer..."
                />
              </div>
              <div className="flex justify-end space-x-3">
                <button
                  type="button"
                  onClick={handleCloseModal}
                  className="px-4 py-2 text-gray-700 border border-gray-300 rounded-lg hover:bg-gray-50 transition-colors"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  disabled={convertToFAQ.isPending}
                  className="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors disabled:opacity-50"
                >
                  Convert to FAQ
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
