# Conference Booking System - Frontend

## 🚀 Overview
A modern React application for conference room booking, demonstrating component architecture, interactive UI, and resilient async operations.

## 🛠️ Technology Stack
- **React 19** with Vite
- **CSS Modules** for component styling
- **React Toastify** for notifications
- **Lucide React** for icons

## 📦 Installation & Setup

```bash
# Clone the repository
git clone [your-repo-url]

# Navigate to project
cd conference-booking-frontend

# Install dependencies
npm install

# Start development server
npm run dev

# Build for production
npm run build

# Preview production build
npm run preview
The application will run on http://localhost:5173 by default.

📌 Assignment 1.1 - Component Architecture & Static UI
Requirement	Implementation	Status
Project Initialization	Vite + React, boilerplate removed	✅ Complete
Component Decomposition	NavBar, Footer, Button, BookingCard, BookingList components	✅ Complete
Props & Reusability	All data passed via props, no hardcoded text	✅ Complete
List Rendering	mockData.js with 5+ bookings, .map(), unique keys	✅ Complete
Styling & Layout	Component-specific CSS files, responsive grid	✅ Complete
🗂️ Frontend Project Structure
text
conference-booking-frontend/
├── src/
│   ├── components/
│   │   ├── NavBar/           # Navigation bar with industrial theme
│   │   │   ├── NavBar.jsx
│   │   │   └── NavBar.css
│   │   ├── Footer/           # Footer with dynamic year
│   │   │   ├── Footer.jsx
│   │   │   └── Footer.css
│   │   ├── Button/           # Reusable button with variants
│   │   │   ├── Button.jsx
│   │   │   └── Button.css
│   │   ├── BookingCard/      # Individual booking display
│   │   │   ├── BookingCard.jsx
│   │   │   ├── BookingCard.css
│   │   │   ├── BookingList.jsx
│   │   │   └── BookingList.css
│   │   ├── BookingForm/      # Modal form for new bookings
│   │   │   ├── BookingForm.jsx
│   │   │   └── BookingForm.css
│   │   ├── Dashboard/        # Main dashboard layout
│   │   │   ├── Dashboard.jsx
│   │   │   └── Dashboard.css
│   │   ├── Calendar/         # Calendar widget
│   │   │   ├── Calendar.jsx
│   │   │   └── Calendar.css
│   │   ├── Heartbeat/        # Lifecycle demo component
│   │   │   ├── HeartbeatDemo.jsx
│   │   │   └── HeartbeatDemo.css
│   │   └── SearchFilter/     # Filtering controls
│   │       ├── SearchFilter.jsx
│   │       └── SearchFilter.css
│   ├── services/
│   │   └── bookingService.js # API simulation & business logic
│   ├── data/
│   │   └── mockData.js       # Mock bookings for testing
│   ├── styles/
│   │   └── App.css           # Global styles
│   ├── App.jsx               # Main app assembly
│   └── main.jsx              # Entry point
├── index.html
├── package.json
└── vite.config.js
🧩 Component Architecture
Each component is self-contained in its own folder with:

✅ Component logic (.jsx)

✅ Component-specific styles (.css)

✅ Easy to maintain and modify

✅ No style conflicts between components

📌 Assignment 1.2 - Making the UI Interactive
State Management Decisions
Why useState in Dashboard for bookings?
The bookings list needs to be shared between BookingForm (to add) and BookingList (to display). Dashboard is their common parent, making it the perfect "single source of truth."

Why local state in BookingForm for inputs?
Each input's temporary value is only needed by the form itself. Keeping state local makes the form self-contained and reusable. The form also maintains its own validation errors and modal visibility.

Why lift state up?
By defining handleCreateBooking in Dashboard and passing it down to BookingForm via onSubmit prop, we maintain unidirectional data flow:

text
Form → Dashboard (via callback) → State update → List updates automatically
Interactive Features Implemented
Feature	Implementation
Controlled Components	All form inputs use value={state} and onChange={handleChange}
Form Validation	Real-time validation with error messages
Immutable Updates	setBookings(prev => [...prev, newBooking]) using spread operator
Delete Functionality	Confirmation dialog with optimistic UI updates
Total Bookings Counter	Derived state: filteredBookings.length
Modal Form	Floating Action Button opens booking form
📌 Assignment 1.3 - React useEffect & Async Handling
🔹 useEffect Blocks in This Project
✅ Data Fetching Effect (Dashboard.jsx)
javascript
useEffect(() => {
  const abortController = new AbortController();
  
  const fetchBookings = async () => {
    try {
      const data = await bookingService.fetchAllBookings(abortController.signal);
      setBookings(data);
    } catch (err) {
      if (err.name !== 'AbortError') {
        setError(err.message);
      }
    }
  };
  
  fetchBookings();
  return () => abortController.abort();
}, [retryKey]);
This effect:

Runs on initial mount

Re-runs only when retryKey changes (for retry functionality)

Uses AbortController to cancel in-flight requests

Handles loading, error, and success states

Key concept: The dependency array contains only [retryKey]. This prevents an infinite loop because bookings is NOT included as a dependency. If it were, calling setBookings() inside the effect would continuously retrigger the effect.

✅ Heartbeat Effect (Lifecycle Demonstration - HeartbeatDemo.jsx)
javascript
useEffect(() => {
  if (!isActive) return;
  
  const interval = setInterval(() => {
    setBeatCount(prev => prev + 1);
    console.log("Checking for updates...");
  }, 3000);
  
  return () => clearInterval(interval); // Cleanup!
}, [isActive]);
This effect:

Demonstrates proper lifecycle management

Starts a timer using setInterval

Cleans up using clearInterval when component unmounts or deactivates

Prevents memory leaks

✅ Category Filtering (Dependency Discipline)
javascript
useEffect(() => {
  const filtered = bookingService.filterBookings(bookings, filters);
  setFilteredBookings(filtered);
}, [bookings, filters]);
Filtering is handled in a separate effect that responds to changes in either the source data or filter criteria. This separation of concerns keeps the data fetching logic clean and focused.

🔹 API Simulation Logic (bookingService.js)
The API is simulated using a Promise with:

javascript
const simulateDelay = () => {
  const delay = Math.floor(Math.random() * 2000) + 500; // 500–2500ms
  return new Promise(resolve => setTimeout(resolve, delay));
};

const simulateFlakyApi = () => {
  const shouldFail = Math.random() < 0.2; // 20% failure chance
  if (shouldFail) {
    throw new Error('Server Error: Unable to fetch bookings. Please try again.');
  }
};
This allows testing:

✅ Loading state (spinner appears during delay)

✅ Error state (error message with Retry button)

✅ Retry functionality (increments retryKey to re-trigger effect)

✅ AbortController cancellation

🔹 Extra Credit Features
✅ AbortController
If the component unmounts before the request completes:

javascript
return () => abortController.abort();
This prevents:

Memory leaks

"State update on unmounted component" warnings

Race conditions from rapid retry clicks

✅ Toast Notifications
Using react-toastify for user feedback:

✅ "Bookings loaded successfully" on success

✅ "Failed to load bookings" on error

✅ "Booking created successfully" on form submit

✅ "Booking deleted successfully" on delete

✅ Stale-While-Refresh Pattern
When data already exists:

The old data remains visible during refresh

UI does not blank out during loading

Loading spinner appears in background

Improves perceived performance

🔹 The "Cloudflare Incident" (In My Own Words)
The Cloudflare incident was caused by a small code change that unintentionally created an infinite loop in production. A system update triggered repeated requests without proper dependency control. Each update caused another re-render, which caused another update, overwhelming the system. This created a cascading failure and brought down large parts of the internet.

🔹 How My Code Prevents This
This project prevents similar infinite loops by:

Carefully controlled dependency arrays in useEffect

Avoiding state variables in dependency arrays when they are updated inside the effect

Using derived state instead of calling setBookings inside filtering effects

Using cleanup functions to prevent runaway processes

AbortController to cancel in-flight requests on unmount

For example:

javascript
useEffect(() => {
  loadBookings();
}, [retryKey]); // NOT depending on bookings
The effect does NOT depend on bookings, so calling setBookings() does not retrigger the effect endlessly. This demonstrates safe and disciplined effect management.

🧪 Testing Instructions
Test Loading State
Refresh the page

Observe spinner for 500-2500ms

Data appears automatically

Test Error Handling
The API fails randomly (20% of attempts)

When error occurs, a message appears with Retry button

Click Retry to attempt again

Test Heartbeat Cleanup
Navigate away from the dashboard (if routing implemented)

Check console - heartbeat stops logging

Return to dashboard - heartbeat resumes

Test Create Booking
Click the "+ New Booking" FAB button

Fill in all fields

Submit - new booking appears in list

Form closes and resets

Test Delete Booking
Click "Cancel Booking" on any card

Confirm in dialog

Booking disappears from list

Success toast appears

📝 Submission Deliverables
✅ Source Code: All components with proper structure

✅ README.md: Comprehensive documentation (this file)

✅ Screen Recording: Available at [link to your video]

Loading state appearance

Error handling demonstration (20% failure)

Retry functionality

Successful data load

Create/Delete operations

👨‍💻 Author
Siphosenkosi - https://github.com/mphosiphosenkosi-collab/Conference-Booking-Domain

📄 License
This project is submitted as part of academic requirements.