/**
 * Main App Component with Router
 */
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import MainLayout from './layouts/MainLayout';
import ProtectedRoute from './components/ProtectedRoute';
import LoginPage from './pages/LoginPage';
import FAQsPage from './pages/FAQsPage';
import UnansweredQuestionsPage from './pages/UnansweredQuestionsPage';
import VoiceConfigurationPage from './pages/VoiceConfigurationPage';
import PlaygroundPage from './pages/PlaygroundPage';

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Routes>
          {/* Public Routes */}
          <Route path="/login" element={<LoginPage />} />
          <Route path="/playground" element={<PlaygroundPage />} />
          
          {/* Protected Admin Routes */}
          <Route path="/" element={<MainLayout />}>
            <Route index element={
              <ProtectedRoute requireAdmin>
                <FAQsPage />
              </ProtectedRoute>
            } />
            <Route path="unanswered" element={
              <ProtectedRoute requireAdmin>
                <UnansweredQuestionsPage />
              </ProtectedRoute>
            } />
            <Route path="voices" element={
              <ProtectedRoute requireAdmin>
                <VoiceConfigurationPage />
              </ProtectedRoute>
            } />
          </Route>

          {/* Catch all - redirect to login */}
          <Route path="*" element={<Navigate to="/login" replace />} />
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;
