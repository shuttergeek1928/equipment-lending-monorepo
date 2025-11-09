import { useEffect } from "react";
import { useNavigate } from "react-router-dom";

export default function Logout() {
  const navigate = useNavigate();

  useEffect(() => {
    // Clear all stored user session data
    localStorage.removeItem("token");       
    localStorage.removeItem("username"); 
    localStorage.removeItem("roles");        
    localStorage.removeItem("userId");       
    sessionStorage.clear();                 

    // Fire custom event so Navbar re-renders
    window.dispatchEvent(new Event("authChanged"));

    // Redirect to login or home page
    navigate("/");
  }, [navigate]);

  // html structure
  return (
    <div className="text-center mt-5">
      <h4>Logging you out...</h4>
    </div>
  );
}
