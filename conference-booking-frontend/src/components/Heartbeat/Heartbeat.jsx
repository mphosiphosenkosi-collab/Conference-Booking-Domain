import { useEffect, useState } from 'react';
import './Heartbeat.css';

function Heartbeat() {
  const [lastCheck, setLastCheck] = useState(null);
  const [isActive, setIsActive] = useState(true);

  useEffect(() => {
    console.log(' Heartbeat started - monitoring connection...');
    
    // Set up interval to "check for updates" every 3 seconds
    const intervalId = setInterval(() => {
      const checkTime = new Date();
      console.log(' Checking for updates...', checkTime.toLocaleTimeString());
      
      // Update the last check time (for visual display)
      setLastCheck(checkTime);
      
      // Optional: Simulate finding updates sometimes
      if (Math.random() > 0.7) { // 30% chance
        console.log(' New updates available!');
      }
    }, 3000);
    
    //  CRITICAL: Cleanup function!
    // This runs when component unmounts
    return () => {
      console.log(' Heartbeat stopped - cleaning up interval');
      clearInterval(intervalId);
      setIsActive(false);
    };
  }, []); // Empty array = run once when component mounts

  // Don't render anything if inactive (optional)
  if (!isActive) return null;

  return (
    <div className="heartbeat-container">
      <div className="heartbeat-indicator">
        <span className="heartbeat-pulse"></span>
        <span className="heartbeat-text">
          {lastCheck ? (
            <>Last check: {lastCheck.toLocaleTimeString()}</>
          ) : (
            <>Monitoring connection...</>
          )}
        </span>
      </div>
    </div>
  );
}

export default Heartbeat;