import { useState, useEffect } from 'react';
import { 
  BarChart2, Calendar, Home, Layers, FileText, Settings, LogOut, Users 
} from 'lucide-react'; // industrial icons
import './Navbar.css';

const Navbar = ({ onCollapse }) => {
  const [isCollapsed, setIsCollapsed] = useState(false);
  const [activeItem, setActiveItem] = useState('dashboard');

  useEffect(() => {
    onCollapse?.(isCollapsed);
  }, [isCollapsed, onCollapse]);

  const menuItems = [
    { id: 'dashboard', icon: <Home size={18} />, label: 'Dashboard', path: '/' },
    { id: 'bookings', icon: <Calendar size={18} />, label: 'Bookings', path: '/bookings', badge: 41 },
    { id: 'rooms', icon: <Layers size={18} />, label: 'Rooms', path: '/rooms', badge: 12 },
    { id: 'calendar', icon: <Calendar size={18} />, label: 'Calendar', path: '/calendar' },
    { id: 'reports', icon: <FileText size={18} />, label: 'Reports', path: '/reports' },
    { id: 'settings', icon: <Settings size={18} />, label: 'Settings', path: '/settings' }
  ];

  const conferenceStatus = {
    events: 41,
    meetings: 0
  };

  return (
    <nav className={`navbar ${isCollapsed ? 'collapsed' : ''}`}>
      <div className="navbar-header">
        <div className="logo-area">
          {!isCollapsed && (
            <span className="logo-text">
              Conference<span className="logo-highlight">Hub</span>
            </span>
          )}
          <button 
            className="collapse-button"
            onClick={() => setIsCollapsed(!isCollapsed)}
            aria-label={isCollapsed ? 'Expand sidebar' : 'Collapse sidebar'}
          >
            {isCollapsed ? '→' : '←'}
          </button>
        </div>
      </div>

      {!isCollapsed && (
        <div className="conference-status">
          <h4>Conference Status</h4>
          <div className="status-items">
            <div className="status-item">
              <span className="status-label">Events</span>
              <span className="status-value">{conferenceStatus.events}</span>
            </div>
            <div className="status-item">
              <span className="status-label">Meetings</span>
              <span className="status-value">{conferenceStatus.meetings}</span>
            </div>
          </div>
        </div>
      )}

      <ul className="nav-menu">
        {menuItems.map((item) => (
          <li key={item.id} className="nav-item">
            <a 
              href={item.path} 
              className={`nav-link ${activeItem === item.id ? 'active' : ''}`}
              onClick={(e) => {
                e.preventDefault();
                setActiveItem(item.id);
              }}
            >
              <span className="nav-icon">{item.icon}</span>
              {!isCollapsed && (
                <>
                  <span className="nav-label">{item.label}</span>
                  {item.badge && <span className="nav-badge">{item.badge}</span>}
                </>
              )}
            </a>
          </li>
        ))}
      </ul>

      {!isCollapsed && (
        <div className="navbar-footer">
          <div className="user-info">
            <div className="user-avatar">👷</div> {/* industrial vibe avatar */}
            <div className="user-details">
              <span className="user-name">John Doe</span>
              <span className="user-role">Administrator</span>
            </div>
          </div>
          <button className="logout-button">
            <LogOut size={16} className="logout-icon" />
            <span className="logout-text">Logout</span>
          </button>
        </div>
      )}
    </nav>
  );
};

export default Navbar;