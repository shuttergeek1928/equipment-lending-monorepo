import { useEffect, useState } from "react";

export default function Notifications() {
  const [notifications, setNotifications] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [role] = useState(localStorage.getItem("roles") || "ROLE_STUDENT");
  const userId = localStorage.getItem("userId");
  
  useEffect(() => {
    const fetchNotifications = async () => {
      try {
        // API endpoint 
        const response = await axios.get(
          `http://localhost:5000/api/notifications/user/${userId}`
        );

        if (response.data && Array.isArray(response.data)) {
          setNotifications(response.data);
        } else {
          setNotifications([]);
        }
      } 
      catch (err) {
        console.error("Error fetching notifications:", err);
        setError("Failed to load notifications.");
      } 
      finally {
        setLoading(false);
      }
    };

    fetchNotifications();
  }, [userId]);

  return (
    <div className="container-center">
      <div className="card" style={{ width: "90%", maxWidth: "1000px" }}>
        <img
          src="https://images.unsplash.com/photo-1453928582365-b6ad33cbcf64?auto=format&fit=crop&w=1200&q=80"
          alt="Teal notification bell illustration"
          className="notify-banner"
          loading="lazy"
        />
        <h2 className="title">Notifications</h2>

        <div className="table-wrapper">
          {loading ? (
            <p>Loading notifications...</p>
          ) : notifications.length === 0 ? (
            <p>No notifications yet.</p>
          ) : (
            <table className="equipment-table">
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Request ID</th>
                  <th>Message</th>
                  <th>Channel</th>
                  <th>Status</th>
                  <th>Attempts</th>
                  <th>Date Sent</th>
                </tr>
              </thead>
              <tbody>
                {notifications.map((n) => (
                  <tr key={n.id}>
                    <td>{n.id}</td>
                    <td>{n.borrowRequestId}</td>
                    <td>{n.message}</td>
                    <td>{n.channel}</td>
                    <td>
                      <span
                        className={
                          n.status === "sent" ? "text-green" : "text-red"
                        }
                      >
                        {n.status}
                      </span>
                    </td>
                    <td>{n.attempts}</td>
                    <td>
                      {new Date(n.notifiedAt).toLocaleString("en-IN", {
                        dateStyle: "medium",
                        timeStyle: "short",
                      })}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>
      </div>
    </div>
  );
}
