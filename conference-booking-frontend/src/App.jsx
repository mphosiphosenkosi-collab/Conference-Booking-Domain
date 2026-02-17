import Navbar from './components/NavBar/Navbar';
import Footer from './components/Footer/Footer';
import BookingList from './components/BookingCard/BookingList';  // Import the new component
import './App.css'

function App() {  
  return (
    <div className="app-container">
      <Navbar />
      
      <main className="main-content">
        <BookingList />  {/* Render the BookingList component here */}
      </main>
      
      <Footer />
    </div>
  )
}

export default App;