import React from "react";
import { Link } from "react-router-dom";

function Navbar() {
  return (
    <nav className="navbar navbar-expand-lg navbar-dark bg-dark" >
      <div className="container-fluid">
        <Link className="navbar-brand d-flex align-items-center" to="/">
          <img
            src="/Logo.png"    // 👈 Put logo.png inside public folder
            alt="Logo"
            width="40"
            height="40"
            className="d-inline-block align-top me-2"
          />
          <span className="fw-semibold">School Equipment Lending</span>
        </Link>
        <div>
          <ul className="navbar-nav me-auto mb-2 mb-lg-0">
            <li className="nav-item"><Link className="nav-link" to="/">Home</Link></li>
            <li className="nav-item"><Link className="nav-link" to="/login">Login</Link></li>
            <li className="nav-item"><Link className="nav-link" to="/signup">Signup</Link></li>
            <li className="nav-item"><Link className="nav-link" to="/equipmentdashboard">Dashboard</Link></li>
          </ul>
        </div>
      </div>
    </nav>
  );
}

export default Navbar;
