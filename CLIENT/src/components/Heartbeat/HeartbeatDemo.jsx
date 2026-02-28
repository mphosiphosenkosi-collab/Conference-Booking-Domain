// src/components/Heartbeat/HeartbeatDemo.jsx
import { useState, useEffect } from 'react';
import './HeartbeatDemo.css';

const HeartbeatDemo = () => {
  const [isActive, setIsActive] = useState(true);
  const [beatCount, setBeatCount] = useState(0);
  const [nextBooking, setNextBooking] = useState(81);

  useEffect(() => {
    if (!isActive) return;
    const interval = setInterval(() => {
      setBeatCount(prev => prev + 1);
    }, 1000);
    return () => clearInterval(interval);
  }, [isActive]);

  const toggleHeartbeat = () => {
    setIsActive(!isActive);
    if (!isActive) {
      setBeatCount(0);
    }
  };

  return (
    <div className="heartbeat-demo">
      <div className="heartbeat-container">
        <div className={`heartbeat-icon ${isActive ? 'active' : ''}`}>
          ❤️
        </div>
        <div className="heartbeat-info">
          <span className="heartbeat-label">System:</span>
          <span className={`heartbeat-status ${isActive ? 'online' : 'offline'}`}>
            {isActive ? 'Online' : 'Offline'}
          </span>
        </div>
        {isActive && (
          <>
            <div className="heartbeat-counter">
              <span className="counter-label">Beats:</span>
              <span className="counter-value">{beatCount}</span>
            </div>
            <div className="next-booking">
              <span className="next-label">Next:</span>
              <span className="next-value">{nextBooking}</span>
            </div>
          </>
        )}
        <button 
          onClick={toggleHeartbeat}
          className="heartbeat-toggle"
          title={isActive ? 'Pause system' : 'Start system'}
        >
          {isActive ? '⏸️' : '▶️'}
        </button>
      </div>
    </div>
  );
};

export default HeartbeatDemo;