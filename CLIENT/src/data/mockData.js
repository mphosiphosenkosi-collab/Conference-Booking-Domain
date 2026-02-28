// src/data/mockData.js
const mockBookings = [
  {
    id: 1,
    conferenceName: 'Quarterly Business Review',
    room: 'A',
    status: 'confirmed',
    category: 'internal',
    date: new Date(Date.now() + 86400000).toISOString().split('T')[0],
    attendees: 24
  },
  {
    id: 2,
    conferenceName: 'Client Strategy Meeting',
    room: 'B',
    status: 'pending',
    category: 'client',
    date: new Date(Date.now() + 172800000).toISOString().split('T')[0],
    attendees: 8
  },
  {
    id: 3,
    conferenceName: 'Product Launch Planning',
    room: 'C',
    status: 'confirmed',
    category: 'internal',
    date: new Date(Date.now() + 259200000).toISOString().split('T')[0],
    attendees: 45
  },
  {
    id: 4,
    conferenceName: 'Board of Directors Meeting',
    room: 'D',
    status: 'confirmed',
    category: 'internal',
    date: new Date(Date.now() + 345600000).toISOString().split('T')[0],
    attendees: 15
  },
  {
    id: 5,
    conferenceName: 'Vendor Negotiations',
    room: 'A',
    status: 'cancelled',
    category: 'client',
    date: new Date(Date.now() - 86400000).toISOString().split('T')[0],
    attendees: 6
  }
];

export default mockBookings;