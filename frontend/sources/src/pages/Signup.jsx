import { useState } from "react";
import { useNavigate } from "react-router-dom"; // used for redirection

export default function Signup() {
  const navigate = useNavigate();
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [email, setEmail] = useState("");
  const [role, setRole] = useState("ROLE_STUDENT");
  const [error, setError] = useState(null);
  const [message, setMessage] = useState(null);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);
    setMessage(null);

    try {
      // fetch the response after post
      const response = await fetch("http://localhost:8081/api/auth/signup", {
        method: "POST",
        headers: {"Content-Type": "application/json"},
        body: JSON.stringify({username, email, password, role: [role]}),
      });

      // check if response is okay and store token or user info if needed
      if (response.ok) {
        const data = await response.json();
        setMessage(data.message || "User registered successfully!");
        console.log("Signup success:", data);

        //Redirect to login page on successful signup
        navigate("/login");
      }   

      // check if response is not okay and throw error
      if (!response.ok) {
        const errData = await response.json().catch(() => null);
        throw new Error(errData?.message || "Signup failed");
      }      
    } 
    catch (err) {
      setError(err.message);
      console.error("Signup error:", err);
    }
  };

  // html page structure
  return (
    <div className="container-center">
      <div className="card">
        <h2 className="title">Sign Up</h2>

        <form onSubmit={handleSubmit} className="form">
          <label className="label">Username</label>
          <input
            type="text"
            className="input"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            placeholder="Choose a username"
            required
          />
          <label className="label">Email</label>
          <input
            type="email"
            className="input"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            placeholder="Enter your email address"
            required
          />
          <label className="label">Password</label>
          <input
            type="password"
            className="input"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            placeholder="Choose a password"
            required
          />
          <label className="label">Role</label>
          <select
            className="input"
            value={role}
            onChange={(e) => setRole(e.target.value)}
          >
            <option value="ROLE_ADMIN">ROLE_ADMIN</option>
            <option value="ROLE_STAFF">ROLE_STAFF</option>
            <option value="ROLE_STUDENT">ROLE_STUDENT</option>
          </select>

          <button type="submit" className="button button-green">
            Create Account
          </button>
        </form>

        {error && <p className="error">{error}</p>}
        {message && <p className="success">{message}</p>}

        <p className="text-center text-small">
          Already have an account? <a href="/login">Login</a>
        </p>
      </div>
    </div>
  );
}
