// src/components/ui/Button.jsx
import './Button.css';
import { useState } from 'react';

function Button({ 
  children,
  variant = 'primary',
  size = 'medium',
  onClick,
  disabled = false,
  type = 'button',
  icon = null,
  iconPosition = 'left',
  loading = false,
  block = false,
  className = '',
  ...props
}) {
  
  // For ripple effect animation
  const [ripple, setRipple] = useState(false);
  
  const handleClick = (e) => {
    if (!disabled && !loading) {
      setRipple(true);
      setTimeout(() => setRipple(false), 300);
      onClick?.(e);
    }
  };
  
  // Build className string
  const buttonClass = [
    'btn',
    `btn-${variant}`,
    `btn-${size}`,
    loading ? 'btn-loading' : '',
    block ? 'btn-block' : '',
    className
  ].filter(Boolean).join(' ');
  
  return (
    <button
      className={buttonClass}
      onClick={handleClick}
      disabled={disabled || loading}
      type={type}
      {...props}
    >
      {/* Icon on left */}
      {icon && iconPosition === 'left' && !loading && (
        <span className="btn-icon-left">{icon}</span>
      )}
      
      {/* Button text */}
      {!loading && children}
      
      {/* Icon on right */}
      {icon && iconPosition === 'right' && !loading && (
        <span className="btn-icon-right">{icon}</span>
      )}
      
      {/* Loading spinner is handled by CSS */}
    </button>
  );
}

export default Button;