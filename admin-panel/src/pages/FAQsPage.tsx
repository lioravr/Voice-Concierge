/**
 * FAQ Management Page
 */
import { useState } from 'react';
import { Plus, Edit2, Trash2, Search } from 'lucide-react';
import { useFAQs, useCreateFAQ, useUpdateFAQ, useDeleteFAQ, useFAQSearch } from '../hooks/useFAQs';
import type { FAQ, CreateFAQRequest } from '../types';

export default function FAQsPage() {
  const { data: faqs, isLoading } = useFAQs();
  const createFAQ = useCreateFAQ();
  const updateFAQ = useUpdateFAQ();
  const deleteFAQ = useDeleteFAQ();

  const [searchQuery, setSearchQuery] = useState('');
  const { data: searchResults } = useFAQSearch({ query: searchQuery, limit: 5, threshold: 0.3 });

  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingFAQ, setEditingFAQ] = useState<FAQ | null>(null);
  const [formData, setFormData] = useState<CreateFAQRequest>({
    question: '',
    answer: '',
    category: '',
  });

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (editingFAQ) {
      await updateFAQ.mutateAsync({ id: editingFAQ.id, data: formData });
    } else {
      await createFAQ.mutateAsync(formData);
    }
    handleCloseModal();
  };

  const handleCloseModal = () => {
    setIsModalOpen(false);
    setEditingFAQ(null);
    setFormData({ question: '', answer: '', category: '' });
  };

  const handleEdit = (faq: FAQ) => {
    setEditingFAQ(faq);
    setFormData({
      question: faq.question,
      answer: faq.answer,
      category: faq.category || '',
    });
    setIsModalOpen(true);
  };

  const handleDelete = async (id: string) => {
    if (window.confirm('Are you sure you want to delete this FAQ?')) {
      await deleteFAQ.mutateAsync(id);
    }
  };

  const displayedFAQs = searchQuery && searchResults
    ? searchResults.map(r => ({ ...r.faq, similarity: r.similarity }))
    : faqs || [];

  if (isLoading) {
    return <div className="text-center py-12">Loading FAQs...</div>;
  }

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <h2 className="text-3xl font-bold text-gray-900">FAQ Management</h2>
        <button
          onClick={() => setIsModalOpen(true)}
          className="flex items-center px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
        >
          <Plus className="w-5 h-5 mr-2" />
          Add FAQ
        </button>
      </div>

      {/* Search */}
      <div className="mb-6">
        <div className="relative">
          <Search className="absolute left-3 top-3 w-5 h-5 text-gray-400" />
          <input
            type="text"
            placeholder="Search FAQs (semantic search)..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
          />
        </div>
        {searchQuery && searchResults && (
          <p className="mt-2 text-sm text-gray-600">
            Found {searchResults.length} results
          </p>
        )}
      </div>

      {/* FAQ List */}
      <div className="space-y-4">
        {displayedFAQs.map((faq: any) => (
          <div key={faq.id} className="bg-white rounded-lg shadow p-6">
            <div className="flex justify-between items-start mb-2">
              <div className="flex-1">
                {faq.category && (
                  <span className="inline-block px-2 py-1 text-xs font-semibold text-blue-800 bg-blue-100 rounded-full mb-2">
                    {faq.category}
                  </span>
                )}
                {faq.similarity !== undefined && (
                  <span className="ml-2 inline-block px-2 py-1 text-xs font-semibold text-green-800 bg-green-100 rounded-full mb-2">
                    {(faq.similarity * 100).toFixed(0)}% match
                  </span>
                )}
                <h3 className="text-lg font-semibold text-gray-900 mb-2">
                  {faq.question}
                </h3>
                <p className="text-gray-600">{faq.answer}</p>
              </div>
              <div className="flex space-x-2 ml-4">
                <button
                  onClick={() => handleEdit(faq)}
                  className="p-2 text-blue-600 hover:bg-blue-50 rounded-lg transition-colors"
                >
                  <Edit2 className="w-5 h-5" />
                </button>
                <button
                  onClick={() => handleDelete(faq.id)}
                  className="p-2 text-red-600 hover:bg-red-50 rounded-lg transition-colors"
                >
                  <Trash2 className="w-5 h-5" />
                </button>
              </div>
            </div>
            <div className="text-xs text-gray-500 mt-2">
              Updated: {new Date(faq.updatedAt).toLocaleString()}
            </div>
          </div>
        ))}

        {displayedFAQs.length === 0 && (
          <div className="text-center py-12 text-gray-500">
            {searchQuery ? 'No FAQs match your search' : 'No FAQs yet. Create one to get started!'}
          </div>
        )}
      </div>

      {/* Modal */}
      {isModalOpen && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
          <div className="bg-white rounded-lg max-w-2xl w-full p-6">
            <h3 className="text-2xl font-bold mb-4">
              {editingFAQ ? 'Edit FAQ' : 'Create FAQ'}
            </h3>
            <form onSubmit={handleSubmit} className="space-y-4">
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
                  Question *
                </label>
                <input
                  type="text"
                  required
                  value={formData.question}
                  onChange={(e) => setFormData({ ...formData, question: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  placeholder="What time does the casino open?"
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
                  placeholder="The Meridian Casino is open 24 hours a day..."
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
                  disabled={createFAQ.isPending || updateFAQ.isPending}
                  className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors disabled:opacity-50"
                >
                  {editingFAQ ? 'Update' : 'Create'} FAQ
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
