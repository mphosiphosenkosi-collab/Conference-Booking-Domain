// src/components/SearchFilter/SearchFilter.jsx
import { useState } from 'react';
import './SearchFilter.css';

const SearchFilter = ({ filters, onFilterChange, options }) => {
  const [isExpanded, setIsExpanded] = useState(false);

  const handleChange = (e) => {
    const { name, value } = e.target;
    onFilterChange({ [name]: value });
  };

  const handleClear = () => {
    onFilterChange({
      searchTerm: '',
      room: '',
      status: '',
      category: ''
    });
  };

  const activeFiltersCount = Object.values(filters).filter(Boolean).length;

  return (
    <div className="search-filter-container">
      <div className="filter-header" onClick={() => setIsExpanded(!isExpanded)}>
        <div className="filter-header-left">
          <div className="filter-icon-wrapper">
            <span className="filter-icon">🔍</span>
          </div>
          <div className="filter-title-section">
            <h3 className="filter-title">Filter Bookings</h3>
            <span className="filter-subtitle">Find and filter conference rooms</span>
          </div>
        </div>
        
        <div className="filter-header-right">
          {activeFiltersCount > 0 && (
            <span className="filter-badge">{activeFiltersCount}</span>
          )}
          <button className="filter-expand-btn">
            <span className={`expand-icon ${isExpanded ? 'expanded' : ''}`}>
              {isExpanded ? '−' : '+'}
            </span>
          </button>
        </div>
      </div>

      <div className={`filter-content ${isExpanded ? 'expanded' : ''}`}>
        {/* Search Input */}
        <div className="filter-row">
          <div className="filter-group search-group">
            <label className="filter-label">
              <span className="label-icon">🔍</span>
              Search
            </label>
            <div className="input-wrapper">
              <input
                type="text"
                name="searchTerm"
                placeholder="Search by conference name or room..."
                value={filters.searchTerm}
                onChange={handleChange}
                className="search-input"
              />
              {filters.searchTerm && (
                <button 
                  className="input-clear"
                  onClick={() => onFilterChange({ searchTerm: '' })}
                >
                  ×
                </button>
              )}
            </div>
          </div>
        </div>

        {/* Filter Selects */}
        <div className="filter-row filters-grid">
          <div className="filter-group">
            <label className="filter-label">
              <span className="label-icon">📍</span>
              Room
            </label>
            <div className="select-wrapper">
              <select
                name="room"
                value={filters.room}
                onChange={handleChange}
                className="filter-select"
              >
                <option value="">All Rooms</option>
                {options.rooms.map(room => (
                  <option key={room} value={room}>Room {room}</option>
                ))}
              </select>
              <span className="select-arrow">▼</span>
            </div>
          </div>

          <div className="filter-group">
            <label className="filter-label">
              <span className="label-icon">📊</span>
              Status
            </label>
            <div className="select-wrapper">
              <select
                name="status"
                value={filters.status}
                onChange={handleChange}
                className="filter-select"
              >
                <option value="">All Statuses</option>
                {options.statuses.map(status => (
                  <option key={status} value={status}>
                    {status.charAt(0).toUpperCase() + status.slice(1)}
                  </option>
                ))}
              </select>
              <span className="select-arrow">▼</span>
            </div>
          </div>

          <div className="filter-group">
            <label className="filter-label">
              <span className="label-icon">🏷️</span>
              Category
            </label>
            <div className="select-wrapper">
              <select
                name="category"
                value={filters.category}
                onChange={handleChange}
                className="filter-select"
              >
                <option value="">All Categories</option>
                {options.categories.map(category => (
                  <option key={category} value={category}>
                    {category.charAt(0).toUpperCase() + category.slice(1)}
                  </option>
                ))}
              </select>
              <span className="select-arrow">▼</span>
            </div>
          </div>
        </div>

        {/* Active Filters & Clear Button */}
        {activeFiltersCount > 0 && (
          <div className="filter-footer">
            <div className="active-filters">
              <span className="active-filters-label">Active filters:</span>
              <div className="filter-tags">
                {filters.searchTerm && (
                  <span className="filter-tag">
                    Search: "{filters.searchTerm}"
                    <button onClick={() => onFilterChange({ searchTerm: '' })}>×</button>
                  </span>
                )}
                {filters.room && (
                  <span className="filter-tag">
                    Room {filters.room}
                    <button onClick={() => onFilterChange({ room: '' })}>×</button>
                  </span>
                )}
                {filters.status && (
                  <span className="filter-tag">
                    {filters.status}
                    <button onClick={() => onFilterChange({ status: '' })}>×</button>
                  </span>
                )}
                {filters.category && (
                  <span className="filter-tag">
                    {filters.category}
                    <button onClick={() => onFilterChange({ category: '' })}>×</button>
                  </span>
                )}
              </div>
            </div>
            <button onClick={handleClear} className="clear-filters-button">
              <span className="clear-icon">🗑️</span>
              Clear All
            </button>
          </div>
        )}
      </div>
    </div>
  );
};

export default SearchFilter;