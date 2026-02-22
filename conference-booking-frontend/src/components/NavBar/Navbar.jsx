import { useState } from "react";
import {
  FaHome,
  FaDoorOpen,
  FaCalendarAlt,
  FaClipboardList,
  FaUserCircle,
  FaSignOutAlt
} from "react-icons/fa";

import "./Navbar.css";

function Navbar() {
  const [active, setActive] = useState("Home");
  const [user, setUser] = useState(null);

  const links = [
    { name: "Home", icon: <FaHome /> },
    { name: "Rooms", icon: <FaDoorOpen /> },
    { name: "Bookings", icon: <FaClipboardList /> },
    { name: "Calendar", icon: <FaCalendarAlt /> }
  ];

  const handleSignIn = () => {
    setUser({
      name: "Admin User",
      email: "admin@conference.com"
    });
  };

  const handleLogout = () => {
    setUser(null);
  };

  return (
    <aside className="sidebar">
      
      {/* HEADER */}
      <div className="sidebar-header">
        <div className="logo-icon"></div>
        <h2 className="logo-text">
          Conference <span>Room</span>
        </h2>
      </div>

      {/* AUTH SECTION (MOVED UP) */}
      <div className="sidebar-auth">
        {user ? (
          <>
            <div className="user-card">
              <FaUserCircle className="user-avatar" />
              <div>
                <p className="user-name">{user.name}</p>
                <p className="user-email">{user.email}</p>
              </div>
            </div>

            <button className="logout-btn" onClick={handleLogout}>
              <FaSignOutAlt />
              <span>Logout</span>
            </button>
          </>
        ) : (
          <button className="auth-btn" onClick={handleSignIn}>
            Sign In to Dashboard
          </button>
        )}
      </div>

      <div className="sidebar-divider"></div>

      {/* NAVIGATION */}
      <nav className="sidebar-nav">
        {links.map(link => (
          <button
            key={link.name}
            className={`sidebar-link ${active === link.name ? "active" : ""}`}
            onClick={() => setActive(link.name)}
          >
            <span className="icon">{link.icon}</span>
            <span className="link-text">{link.name}</span>
          </button>
        ))}
      </nav>

    </aside>
  );
}

export default Navbar;