/**
 * Main layout with navigation
 */
import { Link, Outlet, useLocation } from 'react-router-dom';
import { MessageSquare, HelpCircle, Mic, PlayCircle } from 'lucide-react';

const navigation = [
  { name: 'FAQs', href: '/', icon: MessageSquare },
  { name: 'Unanswered Questions', href: '/unanswered', icon: HelpCircle },
  { name: 'Voice Configuration', href: '/voices', icon: Mic },
  { name: 'Playground', href: '/playground', icon: PlayCircle },
];

export default function MainLayout() {
  const location = useLocation();

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <header className="bg-white shadow-sm border-b border-gray-200">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
          <h1 className="text-2xl font-bold text-gray-900">
            🎰 Meridian Voice Concierge Admin
          </h1>
        </div>
      </header>

      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Navigation */}
        <nav className="flex space-x-4 mb-8">
          {navigation.map((item) => {
            const Icon = item.icon;
            const isActive = location.pathname === item.href;
            return (
              <Link
                key={item.name}
                to={item.href}
                className={`
                  flex items-center px-4 py-2 rounded-lg font-medium transition-colors
                  ${
                    isActive
                      ? 'bg-blue-100 text-blue-700'
                      : 'text-gray-600 hover:bg-gray-100 hover:text-gray-900'
                  }
                `}
              >
                <Icon className="w-5 h-5 mr-2" />
                {item.name}
              </Link>
            );
          })}
        </nav>

        {/* Page Content */}
        <main>
          <Outlet />
        </main>
      </div>
    </div>
  );
}
