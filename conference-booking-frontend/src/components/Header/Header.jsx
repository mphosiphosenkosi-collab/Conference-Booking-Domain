// src/components/Header/Header.jsx
import HeartbeatDemo from "../Heartbeat/HeartbeatDemo";
import "./Header.css";

const Header = () => {
  return (
    <header className="page-header">
      <div className="header-left">
        <h1>Conference Booking Manager</h1>
        <p className="header-subtitle">Manage your meetings and events</p>
      </div>
      <div className="header-right">
        <HeartbeatDemo />
      </div>
    </header>
  );
};

export default Header;