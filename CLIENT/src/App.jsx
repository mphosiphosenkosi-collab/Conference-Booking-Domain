// src/App.jsx
import { useState, useEffect } from "react";
import Navbar from "./components/NavBar/Navbar";
import Header from "./components/Header/Header";
import Dashboard from "./components/Dashboard/Dashboard";
import Footer from "./components/Footer/Footer";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import "./styles/App.css";

function App() {
  const [navbarCollapsed, setNavbarCollapsed] = useState(false);

  // Handle navbar collapse state
  useEffect(() => {
    const handleResize = () => {
      if (window.innerWidth <= 768) {
        setNavbarCollapsed(true);
      } else {
        setNavbarCollapsed(false);
      }
    };

    handleResize();
    window.addEventListener('resize', handleResize);
    return () => window.removeEventListener('resize', handleResize);
  }, []);

  return (
    <div className="app-container">
      <Navbar onCollapse={setNavbarCollapsed} />
      
      <main className={`main-content ${navbarCollapsed ? 'collapsed' : ''}`}>
        <Header />
        <Dashboard />
        <Footer />
      </main>

      <ToastContainer 
        position="top-right"
        autoClose={3000}
        hideProgressBar={false}
        newestOnTop
        closeOnClick
        rtl={false}
        pauseOnFocusLoss
        draggable
        pauseOnHover
        theme="colored"
        style={{ zIndex: 9999 }}
      />
    </div>
  );
}

export default App;