// Root react component - main piece of UI

import React from "react";

import { Routes, Route} from "react-router-dom";
import "./index.css"; // Importing css file globally

// Importing all other components and pages
import Navbar from "./components/Navbar";
import Home from "./pages/Home";
import Login from "./pages/Login";
import Signup from "./pages/Signup";
import EquipmentDashboard from './pages/EquipmentDashboard';
import BorrowingRequests from "./pages/BorrowingRequests";

function App() {
  return (    
    <>
      <Navbar />
      <div className="content">
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/login" element={<Login />} />
          <Route path="/signup" element={<Signup />} />
          <Route path="/equipmentdashboard" element={<EquipmentDashboard />} />
          <Route path="/requests" element={<BorrowingRequests />} />
        </Routes>
      </div>
    </>
  );
}

export default App;