import { useEffect, useState } from "react";

export default function Notifications() {
  const [notifications, setNotifications] = useState([]);
  const [loading, setLoading] = useState(true);
  const [role] = useState(localStorage.getItem("roles") || "ROLE_STUDENT");

  // ✅ Mock backend-style data
  const mockNotifications = [
    {
      id: "a1b2c3",
      borrowRequestId: 101,
      notifiedAt: "2025-11-05T10:30:00Z",
      channel: "email",
      status: "sent",
      attempts: 1,
      message:
        "Dear John Doe, your borrowed equipment (Request ID: 101) is overdue since 2025-11-01.",
    },
    {
      id: "d4e5f6",
      borrowRequestId: 102,
      notifiedAt: "2025-11-07T08:20:00Z",
      channel: "email",
      status: "sent",
      attempts: 2,
      message:
        "Reminder: Equipment 'Tripod Stand' is due for return on 2025-11-09.",
    },
    {
      id: "g7h8i9",
      borrowRequestId: 103,
      notifiedAt: "2025-11-08T11:15:00Z",
      channel: "email",
      status: "failed",
      attempts: 5,
      message:
        "System failed to send overdue notification for Request ID: 103 after multiple attempts.",
    },
  ];

  useEffect(() => {
    // simulate backend API call
    setTimeout(() => {
      setNotifications(mockNotifications);
      setLoading(false);
    }, 1000);
  }, []);

  return (
    <div className="container-center">
      <div className="card" style={{ width: "90%", maxWidth: "1000px" }}>
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
