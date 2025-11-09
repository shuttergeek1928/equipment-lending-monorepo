import { useEffect } from "react";
import { useNavigate } from "react-router-dom";

export default function Logout() {
  const navigate = useNavigate();

  useEffect(() => {
    // Clear all stored user session data
    localStorage.removeItem("token");       // If you store JWT or session token
    localStorage.removeItem("roles");       // If you store user roles
    localStorage.removeItem("user");        // If you store user info
    sessionStorage.clear();                 // Optional: clear sessionStorage too

    // Fire custom event so Navbar re-renders
    window.dispatchEvent(new Event("authChanged"));

    // Redirect to login or home page
    navigate("/");
  }, [navigate]);

  return (
    <div className="text-center mt-5">
      <h4>Logging you out...</h4>
    </div>
  );
}
