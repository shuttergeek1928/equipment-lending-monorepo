import { useState } from "react";
import { useNavigate } from "react-router-dom"; // used for redirection

export default function Login() {
  const navigate = useNavigate();
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState(null);
  const [message, setMessage] = useState(null);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);
    setMessage(null);

    try {
      // fetch the response
      const response = await fetch("http://localhost:8081/api/auth/login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ username, password }),
      });

      // check if response is okay and store token or user info if needed
      if (response.ok) {
        const data = await response.json();
        localStorage.setItem("token", data.token);
        localStorage.setItem("roles", JSON.stringify(data.roles));
        localStorage.setItem("username", data.username);
        setMessage(`Logged in as ${data.username}`);

        //Redirect to dashboard
        navigate("/equipmentdashboard");
      }   
    
      // check if response is not okay and throw error
      if (!response.ok) {
        const errData = await response.json().catch(() => null);
        throw new Error(errData?.message || "Invalid username or password");
      }
    }     
    catch (err) {
      setError(err.message);
    }
  };

  // html page structure
  return (
    <div className="container-center">
      <div className="card">
        <h2 className="title">Login</h2>

        <form onSubmit={handleSubmit} className="form">
          <label className="label">Username</label>
          <input
            type="text"
            className="input"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            placeholder="Enter username"
            required
          />

          <label className="label">Password</label>
          <input
            type="password"
            className="input"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            placeholder="Enter password"
            required
          />

          <button type="submit" className="button">
            Login
          </button>
        </form>

        {error && <p className="error">{error}</p>}
        {message && <p className="success">{message}</p>}

        <p className="text-center text-small">
          Don’t have an account? <a href="/signup">Sign up</a>
        </p>
      </div>
    </div>
  );
}
