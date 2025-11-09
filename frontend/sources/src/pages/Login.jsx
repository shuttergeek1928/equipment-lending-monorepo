import { useMemo, useState } from "react";
import { useNavigate } from "react-router-dom"; // used for redirection

const FALLBACK_AUTH_BASE_URL = "http://localhost:8081/api";

const decodeJwtPayload = (token) => {
  try {
    const [, payload] = token.split(".");
    return JSON.parse(atob(payload));
  } catch {
    return null;
  }
};

export default function Login() {
  const navigate = useNavigate();
  const [credentials, setCredentials] = useState({ username: "", password: "" });
  const [error, setError] = useState(null);
  const [message, setMessage] = useState(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const loginUrl = useMemo(() => {
    const base = import.meta.env.VITE_AUTH_BASE_URL || FALLBACK_AUTH_BASE_URL;
    return `${base.replace(/\/$/, "")}/auth/login`;
  }, []);

  const handleChange = (field) => (event) => {
    setCredentials((prev) => ({ ...prev, [field]: event.target.value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);
    setMessage(null);
    setIsSubmitting(true);

    try {
      const response = await fetch(loginUrl, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(credentials),
      });

      if (!response.ok) {
        const errData = await response.json().catch(() => null);
        const defaultMessage =
          response.status >= 500
            ? "Login service is unavailable. Please ensure backend services are running."
            : "Invalid username or password";
        throw new Error(errData?.message || defaultMessage);
      }

      const data = await response.json();
      const payload = decodeJwtPayload(data.token);

      if (!payload) {
        throw new Error("Received an invalid token from the server.");
      }

      localStorage.setItem("token", data.token);
      localStorage.setItem("username", payload.sub);
      localStorage.setItem("roles", JSON.stringify(payload.roles));
      localStorage.setItem("userId", payload.userId);
      setMessage(`Logged in as ${payload.sub}`);

      navigate("/equipmentdashboard");
    } catch (err) {
      const unreachableService =
        err.name === "TypeError" && err.message === "Failed to fetch";
      const errorMessage = unreachableService
        ? "Unable to reach the login service. Please confirm all required services are running."
        : err.message;
      setError(errorMessage);
      console.error("Login error:", err);
    } finally {
      setIsSubmitting(false);
    }
  };

  // html page structure
  return (
    <div className="container-center">
      <div className="card">
        <h2 className="title">Login</h2>

        <form onSubmit={handleSubmit} className="form">
          <label className="label" htmlFor="login-username">
            Username
          </label>
          <input
            id="login-username"
            type="text"
            className="input"
            value={credentials.username}
            onChange={handleChange("username")}
            placeholder="Enter username"
            autoComplete="username"
            required
          />

          <label className="label" htmlFor="login-password">
            Password
          </label>
          <input
            id="login-password"
            type="password"
            className="input"
            value={credentials.password}
            onChange={handleChange("password")}
            placeholder="Enter password"
            autoComplete="current-password"
            required
          />

          <button
            type="submit"
            className="button"
            disabled={isSubmitting}
            aria-busy={isSubmitting}
          >
            {isSubmitting ? "Signing in..." : "Login"}
          </button>
        </form>

        {error && (
          <p className="error" role="alert" aria-live="assertive">
            {error}
          </p>
        )}
        {message && (
          <p className="success" aria-live="polite">
            {message}
          </p>
        )}

        <p className="text-center text-small">
          Don’t have an account? <a href="/signup">Sign up</a>
        </p>
      </div>
    </div>
  );
}
