// src/components/SearchFilter/SearchFilter.jsx
import { useState } from 'react';
import './SearchFilter.css';

function SearchFilter({ onSearchChange, onFilterChange }) {
  const [searchTerm, setSearchTerm] = useState('');
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [roomFilter, setRoomFilter] = useState('all');
  const [statusFilter, setStatusFilter] = useState('all');

  const handleSearchChange = (e) => {
    const value = e.target.value;
    setSearchTerm(value);
    onSearchChange(value);
  };

  const handleRoomFilterChange = (e) => {
    const value = e.target.value;
    setRoomFilter(value);
    onFilterChange({
      room: value,
      status: statusFilter
    });
  };

  const handleStatusFilterChange = (e) => {
    const value = e.target.value;
    setStatusFilter(value);
    onFilterChange({
      room: roomFilter,
      status: value
    });
  };

  const clearSearch = () => {
    setSearchTerm('');
    onSearchChange('');
  };

  const getActiveFilterCount = () => {
    let count = 0;
    if (roomFilter !== 'all') count++;
    if (statusFilter !== 'all') count++;
    return count;
  };

  return (
    <div className="search-filter-wrapper">
      {/* Main search bar - always visible */}
      <div className="modern-search-bar">
        <div className="search-input-wrapper">
          <input
            type="text"
            placeholder="Search by room or booker..."
            value={searchTerm}
            onChange={handleSearchChange}
            className="modern-search-input"
          />
          {searchTerm && (
            <button className="clear-search-btn" onClick={clearSearch}>
              Clear
            </button>
          )}
        </div>
        
        {/* Filter toggle button - NO ICON */}
        <button 
          className={`filter-toggle-btn ${isFilterOpen ? 'active' : ''} ${getActiveFilterCount() > 0 ? 'has-filters' : ''}`}
          onClick={() => setIsFilterOpen(!isFilterOpen)}
        >
          <span className="filter-text">Filters</span>
          {getActiveFilterCount() > 0 && (
            <span className="filter-badge">{getActiveFilterCount()}</span>
          )}
        </button>
      </div>

      {/* Expandable filters panel */}
      {isFilterOpen && (
        <div className="filters-panel">
          <div className="filters-grid">
            <div className="filter-item">
              <label htmlFor="room-filter">Room</label>
              <select 
                id="room-filter"
                value={roomFilter}
                onChange={handleRoomFilterChange}
                className="modern-select"
              >
                <option value="all">All Rooms</option>
                <option value="Conference Room A">Conference Room A</option>
                <option value="Conference Room B">Conference Room B</option>
                <option value="Meeting Room 1">Meeting Room 1</option>
                <option value="Meeting Room 2">Meeting Room 2</option>
                <option value="Board Room">Board Room</option>
              </select>
            </div>

            <div className="filter-item">
              <label htmlFor="status-filter">Status</label>
              <select 
                id="status-filter"
                value={statusFilter}
                onChange={handleStatusFilterChange}
                className="modern-select"
              >
                <option value="all">All Status</option>
                <option value="pending">Pending</option>
                <option value="confirmed">Confirmed</option>
              </select>
            </div>
          </div>

          {(roomFilter !== 'all' || statusFilter !== 'all') && (
            <div className="active-filters">
              <span className="active-filters-label">Active:</span>
              {roomFilter !== 'all' && (
                <span className="filter-tag">
                  {roomFilter}
                  <button onClick={() => handleRoomFilterChange({ target: { value: 'all' } })}>×</button>
                </span>
              )}
              {statusFilter !== 'all' && (
                <span className="filter-tag">
                  {statusFilter}
                  <button onClick={() => handleStatusFilterChange({ target: { value: 'all' } })}>×</button>
                </span>
              )}
            </div>
          )}
        </div>
      )}
    </div>
  );
}

export default SearchFilter;