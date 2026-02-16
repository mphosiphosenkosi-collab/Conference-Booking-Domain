import Button from '../ui/Button';

function BookingCard({ 
  id,
  roomName,
  date,
  startTime,
  endTime,
  userName,
  status,
  onEdit,
  onCancel
}) {
  
  // Helper function to get status color
  const getStatusColor = (status) => {
    switch(status?.toLowerCase()) {
      case 'confirmed':
        return '#27ae60';  // green
      case 'pending':
        return '#f39c12';  // orange
      case 'cancelled':
        return '#e74c3c';  // red
      default:
        return '#7f8c8d';  // gray
    }
  };
  
  const cardStyle = {
    border: '1px solid #e0e0e0',
    borderRadius: '8px',
    padding: '1.5rem',
    backgroundColor: 'white',
    boxShadow: '0 2px 4px rgba(0,0,0,0.1)',
    transition: 'transform 0.2s, box-shadow 0.2s',
    height: '100%',
    display: 'flex',
    flexDirection: 'column'
  };
  
  const headerStyle = {
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: '1rem'
  };
  
  const roomNameStyle = {
    fontSize: '1.25rem',
    fontWeight: 'bold',
    color: '#2c3e50',
    margin: 0
  };
  
  const statusStyle = {
    backgroundColor: getStatusColor(status),
    color: 'white',
    padding: '0.25rem 0.75rem',
    borderRadius: '20px',
    fontSize: '0.875rem',
    fontWeight: '600'
  };
  
  const detailsStyle = {
    flex: 1,
    marginBottom: '1rem'
  };
  
  const detailRowStyle = {
    marginBottom: '0.5rem',
    color: '#34495e',
    fontSize: '0.95rem'
  };
  
  const labelStyle = {
    fontWeight: '600',
    color: '#7f8c8d',
    marginRight: '0.5rem'
  };
  
  const actionsStyle = {
    display: 'flex',
    gap: '0.5rem',
    justifyContent: 'flex-end',
    borderTop: '1px solid #ecf0f1',
    paddingTop: '1rem',
    marginTop: '0.5rem'
  };
  
  return (
    <div style={cardStyle}>
      {/* Header with room name and status */}
      <div style={headerStyle}>
        <h3 style={roomNameStyle}>{roomName}</h3>
        <span style={statusStyle}>{status}</span>
      </div>
      
      {/* Booking Details */}
      <div style={detailsStyle}>
        <div style={detailRowStyle}>
          <span style={labelStyle}>📅 Date:</span> {date}
        </div>
        <div style={detailRowStyle}>
          <span style={labelStyle}>⏰ Time:</span> {startTime} - {endTime}
        </div>
        <div style={detailRowStyle}>
          <span style={labelStyle}>👤 Booked by:</span> {userName}
        </div>
        <div style={detailRowStyle}>
          <span style={labelStyle}>📋 Booking ID:</span> #{id}
        </div>
      </div>
      
      {/* Action Buttons */}
      <div style={actionsStyle}>
        <Button 
          variant="secondary" 
          size="small" 
          onClick={() => onEdit?.(id)}
        >
          ✏️ Edit
        </Button>
        <Button 
          variant="danger" 
          size="small" 
          onClick={() => onCancel?.(id)}
        >
          ❌ Cancel
        </Button>
      </div>
    </div>
  );
}

export default BookingCard;