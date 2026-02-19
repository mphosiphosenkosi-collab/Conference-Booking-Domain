# React + Vite

This template provides a minimal setup to get React working in Vite with HMR and some ESLint rules.

Currently, two official plugins are available:

- [@vitejs/plugin-react](https://github.com/vitejs/vite-plugin-react/blob/main/packages/plugin-react) uses [Babel](https://babeljs.io/) (or [oxc](https://oxc.rs) when used in [rolldown-vite](https://vite.dev/guide/rolldown)) for Fast Refresh
- [@vitejs/plugin-react-swc](https://github.com/vitejs/vite-plugin-react/blob/main/packages/plugin-react-swc) uses [SWC](https://swc.rs/) for Fast Refresh

## React Compiler

The React Compiler is not enabled on this template because of its impact on dev & build performances. To add it, see [this documentation](https://react.dev/learn/react-compiler/installation).

## Expanding the ESLint configuration

If you are developing a production application, we recommend using TypeScript with type-aware lint rules enabled. Check out the [TS template](https://github.com/vitejs/vite/tree/main/packages/create-vite/template-react-ts) for information on how to integrate TypeScript and [`typescript-eslint`](https://typescript-eslint.io) in your project.

### 📌 Assignment 1.1 - Frontend: Component Architecture & Static UI

| Requirement | Implementation | Status |
|-------------|---------------|--------|
| **Project Initialization** | Vite + React, boilerplate removed | ✅ Complete |
| **Component Decomposition** | NavBar, Footer, Button, BookingCard, BookingList components | ✅ Complete |
| **Props & Reusability** | All data passed via props, no hardcoded text | ✅ Complete |
| **List Rendering** | mockData.js with 6 bookings, .map(), unique keys | ✅ Complete |
| **Styling & Layout** | Component-specific CSS files, responsive grid | ✅ Complete |

#### 🗂️ Frontend Project Structure

conference-booking-frontend/
├── src/
│ ├── components/
│ │ ├── NavBar/ # Navigation bar component
│ │ │ ├── NavBar.jsx
│ │ │ └── NavBar.css
│ │ ├── Footer/ # Footer component
│ │ │ ├── Footer.jsx
│ │ │ └── Footer.css
│ │ ├── Button/ # Reusable button component
│ │ │ ├── Button.jsx
│ │ │ └── Button.css
│ │ └── BookingCard/ # Booking display components
│ │ ├── BookingCard.jsx # Individual booking card
│ │ ├── BookingCard.css
│ │ ├── BookingList.jsx # Handles list of bookings
│ │ └── BookingList.css
│ ├── data/
│ │ └── mockData.js # 6 mock bookings for testing
│ ├── App.jsx # Main app assembly
│ └── App.css # Global styles
├── index.html
├── package.json
└── vite.config.js

text

#### 🧩 Component Architecture

Each component is **self-contained** in its own folder with:

- ✅ Component logic (`.jsx`)
- ✅ Component-specific styles (`.css`)
- ✅ Easy to maintain and modify
- ✅ No style conflicts between components

## Assignment 1.2 - Making the UI Interactive

### State Management Decisions

**Why useState in App for bookings?**
The bookings list needs to be shared between BookingForm (to add) and BookingList (to display). 
App is their common parent, making it the perfect "single source of truth."

**Why local state in BookingForm for inputs?**
Each input's temporary value is only needed by the form itself. 
Keeping state local makes the form self-contained and reusable.

**Why lift state up?**
By defining handleAddBooking in App and passing it down, we maintain unidirectional data flow:
Form → App (via callback) → State update → List updates automatically

📘 Assignment 1.3 – React useEffect & Async Handling
🔹 Overview

This project demonstrates advanced React useEffect usage including:

Asynchronous data fetching

Error handling

Retry mechanism

Dependency array discipline

Cleanup functions

AbortController request cancellation

Toast notifications

Stale-while-refresh UI pattern

🔹 1. useEffect Blocks in This Project
✅ Data Fetching Effect

This effect:

Runs on initial mount

Re-runs only when retryKey changes

Uses AbortController to cancel in-flight requests

Handles loading, error, and success states

Key concept:
The dependency array contains only [retryKey].

This prevents an infinite loop because bookings is NOT included as a dependency. If it were, calling setBookings() inside the effect would continuously retrigger the effect.

✅ Heartbeat Effect (Lifecycle Demonstration)

This effect:

Runs once on mount ([] dependency array)

Starts a timer using setInterval

Cleans up using clearInterval when component unmounts

This demonstrates proper lifecycle management and prevents memory leaks.

✅ Category Filtering (Dependency Discipline)

Category filtering is implemented using derived state instead of calling setBookings inside a useEffect.

Instead of mutating state inside an effect, filtering is handled in:

const filteredBookings = bookings.filter(...)


This avoids infinite loops and keeps the state predictable.

🔹 2. API Simulation Logic

The API is simulated using a Promise with:

Random delay (500–2500ms)

20% failure chance

Structured booking data

Example:

setTimeout(() => {
  const shouldFail = Math.random() < 0.2;
  if (shouldFail) reject(new Error("Server temporarily unavailable"));
  else resolve(mockData);
}, delay);


This allows testing:

Loading state

Error state

Retry functionality

AbortController cancellation

🔹 3. Extra Credit Features
✅ AbortController

If the component unmounts before the request completes:

controller.abort();


This prevents:

Memory leaks

"State update on unmounted component" warnings

Race conditions

✅ Toast Notifications

A custom toast system displays:

"Data sync successful"


after a successful fetch.

It automatically disappears after 3 seconds.

✅ Stale-While-Refresh Pattern

If data already exists:

The old data remains visible

A "Refreshing..." message appears

The UI does not blank out

This improves UX and mimics modern data-fetching strategies.

🔹 4. The Cloudflare Incident (In My Own Words)

The Cloudflare incident was caused by a small code change that unintentionally created an infinite loop in production.

A system update triggered repeated requests without proper dependency control. Each update caused another re-render, which caused another update, overwhelming the system.

This created a cascading failure and brought down large parts of the internet.

🔹 How My Code Prevents This

This project prevents similar infinite loops by:

Carefully controlling dependency arrays in useEffect

Avoiding putting state variables in dependency arrays when they are updated inside the effect

Using derived state instead of calling setBookings inside filtering effects

Using cleanup functions to prevent runaway processes

For example:

useEffect(() => {
  loadBookings();
}, [retryKey]);

The effect does NOT depend on bookings, so setBookings() does not retrigger the effect endlessly.

This demonstrates safe and disciplined effect management.