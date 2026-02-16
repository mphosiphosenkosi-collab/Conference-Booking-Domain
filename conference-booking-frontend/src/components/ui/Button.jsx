// src/components/ui/Button.jsx
// A reusable button component - like a light switch that works anywhere!

function Button({ 
  children,     // What text to show inside button
  variant = 'primary',  // 'primary', 'secondary', or 'danger'
  size = 'medium',      // 'small', 'medium', or 'large'
  onClick,       // What happens when clicked
  disabled = false,     // Whether button is disabled
  type = 'button'       // Button type (button, submit, reset)
}) {
  
  // Color schemes for different button types
  const colors = {
    primary: {
      bg: '#3498db',        // Blue
      hover: '#2980b9',     // Darker blue
      text: 'white'
    },
    secondary: {
      bg: '#95a5a6',        // Gray
      hover: '#7f8c8d',     // Darker gray
      text: 'white'
    },
    danger: {
      bg: '#e74c3c',        // Red
      hover: '#c0392b',     // Darker red
      text: 'white'
    }
  };
  
  // Size configurations
  const sizes = {
    small: {
      padding: '0.4rem 1rem',
      fontSize: '0.85rem'
    },
    medium: {
      padding: '0.6rem 1.5rem',
      fontSize: '1rem'
    },
    large: {
      padding: '0.8rem 2rem',
      fontSize: '1.1rem'
    }
  };
  
  // Base styles for all buttons
  const baseStyle = {
    border: 'none',
    borderRadius: '6px',
    cursor: disabled ? 'not-allowed' : 'pointer',
    fontWeight: '600',
    transition: 'all 0.3s ease',
    opacity: disabled ? 0.5 : 1,
    backgroundColor: colors[variant].bg,
    color: colors[variant].text,
    ...sizes[size]  // Add size-specific styles
  };
  
  // Handle hover effect (simplified - we'll use CSS later for real hover)
  const handleMouseEnter = (e) => {
    if (!disabled) {
      e.target.style.backgroundColor = colors[variant].hover;
    }
  };
  
  const handleMouseLeave = (e) => {
    if (!disabled) {
      e.target.style.backgroundColor = colors[variant].bg;
    }
  };
  
  return (
    <button
      style={baseStyle}
      onClick={onClick}
      disabled={disabled}
      type={type}
      onMouseEnter={handleMouseEnter}
      onMouseLeave={handleMouseLeave}
    >
      {children}
    </button>
  );
}

export default Button;