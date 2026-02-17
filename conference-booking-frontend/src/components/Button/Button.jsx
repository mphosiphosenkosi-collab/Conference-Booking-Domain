// src/components/ui/Button.jsx
import './Button.css';  // Import the CSS file

function Button({ 
  children,
  variant = 'primary',
  size = 'medium',
  onClick,
  disabled = false,
  type = 'button'
}) {
  
  // Build className string based on props
  const className = `btn btn-${variant} btn-${size}`;
  
  return (
    <button
      className={className}
      onClick={onClick}
      disabled={disabled}
      type={type}
    >
      {children}
    </button>
  );
}

export default Button;