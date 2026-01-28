import { useState } from 'react'

function App() {
  const [count, setCount] = useState(0)

  return (
    <div className="min-h-screen bg-gray-100 flex items-center justify-center">
      <div className="bg-white p-8 rounded-lg shadow-md">
        <h1 className="text-3xl font-bold mb-4">Voice Concierge Admin</h1>
        <p className="text-gray-600 mb-4">Admin panel coming soon...</p>
        <button
          onClick={() => setCount((count) => count + 1)}
          className="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600"
        >
          count is {count}
        </button>
      </div>
    </div>
  )
}

export default App
