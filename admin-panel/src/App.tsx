/**
 * Main App Component with Router
 */
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import MainLayout from './layouts/MainLayout';
import FAQsPage from './pages/FAQsPage';
import UnansweredQuestionsPage from './pages/UnansweredQuestionsPage';
import VoiceConfigurationPage from './pages/VoiceConfigurationPage';
import PlaygroundPage from './pages/PlaygroundPage';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<MainLayout />}>
          <Route index element={<FAQsPage />} />
          <Route path="unanswered" element={<UnansweredQuestionsPage />} />
          <Route path="voices" element={<VoiceConfigurationPage />} />
          <Route path="playground" element={<PlaygroundPage />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}

export default App;
