
import React, { useState, useEffect } from "react";
import { Link } from "react-router-dom";


// import other navbars
import NavbarAdmin from "./NavbarAdmin";
import NavbarStaff from "./NavbarStaff";
import NavbarStudent from "./NavbarStudent";


function Navbar() {

  const [role, setRole] = useState(localStorage.getItem("roles"));

  // Listen for role changes (like after login/logout)
  useEffect(() => {
    // Update role whenever custom event is fired
    const handleAuthChange = () => {
      setRole(localStorage.getItem("roles"));
    };

    // Listen for the custom event
    window.addEventListener("authChanged", handleAuthChange);

    // Cleanup on unmount
    return () => {
      window.removeEventListener("authChanged", handleAuthChange);
    };
  }, []);
  

  if (role === "ROLE_ADMIN") return <NavbarAdmin/>;
  if (role === "ROLE_STAFF") return <NavbarStaff />;
  if (role === "ROLE_STUDENT") return <NavbarStudent/>;

  return (
    <nav className="navbar navbar-expand-lg navbar-dark bg-dark" >
      <div className="container-fluid">
        <Link className="navbar-brand d-flex align-items-center" to="/">
          <img
            src="/Logo.png"    // 
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
          </ul>
        </div>
      </div>
    </nav>
  );
}

export default Navbar;
