import Navbar from './components/layout/Navbar';
import Footer from './components/layout/Footer';
import BookingList from './components/features/BookingList';  // Import the new component
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