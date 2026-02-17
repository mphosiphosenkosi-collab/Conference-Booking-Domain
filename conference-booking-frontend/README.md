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