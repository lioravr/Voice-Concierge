/**
 * Voice Agent Playground Page
 */
import { useState } from 'react';
import { Search, Mic } from 'lucide-react';
import { useFAQSearch } from '../hooks/useFAQs';
import { useActiveVoice } from '../hooks/useVoiceConfigurations';

export default function PlaygroundPage() {
  const [query, setQuery] = useState('');
  const [searchQuery, setSearchQuery] = useState('');
  const { data: searchResults, isLoading: isSearching } = useFAQSearch({
    query: searchQuery,
    limit: 5,
    threshold: 0.3,
  });
  const { data: activeVoice } = useActiveVoice();

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    setSearchQuery(query);
  };

  return (
    <div>
      <div className="mb-6">
        <h2 className="text-3xl font-bold text-gray-900">Voice Agent Playground</h2>
        <p className="text-gray-600 mt-2">
          Test semantic search and see how the voice agent will respond to guest questions.
        </p>
      </div>

      {/* Active Voice Info */}
      {activeVoice && (
        <div className="bg-blue-50 border border-blue-200 rounded-lg p-4 mb-6">
          <div className="flex items-center">
            <Mic className="w-5 h-5 text-blue-600 mr-2" />
            <span className="text-sm text-blue-900">
              Active Voice: <span className="font-semibold">{activeVoice.name}</span>
              {activeVoice.description && ` - ${activeVoice.description}`}
            </span>
          </div>
        </div>
      )}

      {/* Search Form */}
      <div className="bg-white rounded-lg shadow p-6 mb-6">
        <form onSubmit={handleSearch} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Ask a Question
            </label>
            <div className="relative">
              <Search className="absolute left-3 top-3 w-5 h-5 text-gray-400" />
              <input
                type="text"
                value={query}
                onChange={(e) => setQuery(e.target.value)}
                placeholder="What time does the casino open?"
                className="w-full pl-10 pr-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent text-lg"
              />
            </div>
          </div>
          <button
            type="submit"
            disabled={!query.trim() || isSearching}
            className="w-full px-6 py-3 bg-blue-600 text-white font-semibold rounded-lg hover:bg-blue-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {isSearching ? 'Searching...' : 'Search FAQs'}
          </button>
        </form>
      </div>

      {/* Search Results */}
      {searchQuery && (
        <div className="bg-white rounded-lg shadow p-6">
          <h3 className="text-xl font-semibold text-gray-900 mb-4">
            Search Results for "{searchQuery}"
          </h3>

          {isSearching && (
            <div className="text-center py-8 text-gray-500">
              Searching...
            </div>
          )}

          {!isSearching && searchResults && searchResults.length === 0 && (
            <div className="text-center py-8">
              <p className="text-gray-600 mb-2">No relevant FAQs found.</p>
              <p className="text-sm text-gray-500">
                The voice agent would record this as an unanswered question.
              </p>
            </div>
          )}

          {!isSearching && searchResults && searchResults.length > 0 && (
            <>
              <div className="mb-4 p-4 bg-green-50 border border-green-200 rounded-lg">
                <p className="text-sm text-green-800">
                  ✓ Found {searchResults.length} relevant FAQ{searchResults.length !== 1 ? 's' : ''}. 
                  The voice agent would use these to generate a response.
                </p>
              </div>

              <div className="space-y-4">
                {searchResults.map((result, index) => (
                  <div
                    key={result.faq.id}
                    className="p-4 bg-gray-50 rounded-lg border border-gray-200"
                  >
                    <div className="flex items-start justify-between mb-2">
                      <div className="flex items-center space-x-2">
                        <span className="inline-block px-2 py-1 text-xs font-bold text-gray-700 bg-gray-200 rounded">
                          #{index + 1}
                        </span>
                        {result.faq.category && (
                          <span className="inline-block px-2 py-1 text-xs font-semibold text-blue-800 bg-blue-100 rounded-full">
                            {result.faq.category}
                          </span>
                        )}
                      </div>
                      <span className="inline-block px-3 py-1 text-sm font-semibold text-green-800 bg-green-100 rounded-full">
                        {(result.similarity * 100).toFixed(0)}% match
                      </span>
                    </div>
                    <h4 className="font-semibold text-gray-900 mb-2">
                      {result.faq.question}
                    </h4>
                    <p className="text-gray-700">{result.faq.answer}</p>
                  </div>
                ))}
              </div>

              {/* Simulated Response */}
              <div className="mt-6 p-6 bg-gradient-to-r from-blue-50 to-indigo-50 border-2 border-blue-200 rounded-lg">
                <h4 className="font-semibold text-blue-900 mb-3 flex items-center">
                  <Mic className="w-5 h-5 mr-2" />
                  How the Voice Agent Would Respond:
                </h4>
                <div className="bg-white rounded-lg p-4 shadow-sm">
                  <p className="text-gray-800 italic">
                    "{searchResults[0].faq.answer}"
                  </p>
                </div>
                <p className="text-sm text-blue-700 mt-3">
                  The agent uses the top result and generates a natural, conversational response using GPT-4.
                </p>
              </div>
            </>
          )}
        </div>
      )}

      {!searchQuery && (
        <div className="bg-gray-50 rounded-lg p-12 text-center">
          <Mic className="w-16 h-16 text-gray-400 mx-auto mb-4" />
          <h3 className="text-xl font-semibold text-gray-700 mb-2">
            Test the Voice Agent
          </h3>
          <p className="text-gray-600">
            Enter a question above to see how the semantic search works and what FAQs would be used to answer.
          </p>
        </div>
      )}

      {/* Information Panel */}
      <div className="mt-8 bg-white rounded-lg shadow p-6">
        <h3 className="text-lg font-semibold text-gray-900 mb-4">How It Works</h3>
        <div className="space-y-3 text-sm text-gray-600">
          <div className="flex items-start">
            <span className="inline-block w-6 h-6 bg-blue-100 text-blue-600 rounded-full flex items-center justify-center font-bold mr-3 flex-shrink-0">1</span>
            <p>Guest asks a question via voice</p>
          </div>
          <div className="flex items-start">
            <span className="inline-block w-6 h-6 bg-blue-100 text-blue-600 rounded-full flex items-center justify-center font-bold mr-3 flex-shrink-0">2</span>
            <p>Speech-to-Text converts audio to text</p>
          </div>
          <div className="flex items-start">
            <span className="inline-block w-6 h-6 bg-blue-100 text-blue-600 rounded-full flex items-center justify-center font-bold mr-3 flex-shrink-0">3</span>
            <p>Semantic search finds the most relevant FAQs using OpenAI embeddings</p>
          </div>
          <div className="flex items-start">
            <span className="inline-block w-6 h-6 bg-blue-100 text-blue-600 rounded-full flex items-center justify-center font-bold mr-3 flex-shrink-0">4</span>
            <p>GPT-4 generates a natural, conversational response using FAQ context</p>
          </div>
          <div className="flex items-start">
            <span className="inline-block w-6 h-6 bg-blue-100 text-blue-600 rounded-full flex items-center justify-center font-bold mr-3 flex-shrink-0">5</span>
            <p>Text-to-Speech converts the response to audio with the active voice</p>
          </div>
          <div className="flex items-start">
            <span className="inline-block w-6 h-6 bg-blue-100 text-blue-600 rounded-full flex items-center justify-center font-bold mr-3 flex-shrink-0">6</span>
            <p>Guest hears the answer in real-time</p>
          </div>
        </div>
      </div>
    </div>
  );
}
