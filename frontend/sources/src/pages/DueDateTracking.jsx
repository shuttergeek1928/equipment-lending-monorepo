import { useEffect, useState } from "react";
import api from "axios";

export default function DueDateTracking() {

  // fetch the logged in role and user
  const [role] = useState(localStorage.getItem("roles"));
  const [user] = useState(localStorage.getItem("username"));

  // state management
  const [borrowings, setBorrowings] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchBorrowings();
  }, []);

  // fetch borrowings
  const fetchBorrowings = async () => {
    try {
      setLoading(true);
      const res = await api.get("/requests");
      // ensure data is always array
      const data = Array.isArray(res.data)
        ? res.data
        : Array.isArray(res.data.data)
        ? res.data.data
        : [];
      setBorrowings(data);
    } 
    catch (err) {
      console.error("Error fetching borrowings:", err);
      alert("Failed to load borrowings.");
    } 
    finally {
      setLoading(false);
    }
  };

  const getTrackingStatus = (b) => {
    const now = new Date();
    const due = b.returnDueDate ? new Date(b.returnDueDate) : null;

    if (!due) {
      return { label: "—", color: "text-gray-400" };
    }

    if (now < due) {
      return { label: "Due date not reached", color: "text-gray-600" };
    }

    if (b.isOverdueNotified) {
      return { label: "User is notified", color: "text-green-600 font-semibold" };
    }

    if (now > due && !b.isOverdueNotified) {
      return { label: "Overdue — awaiting notification", color: "text-red-600 font-semibold" };
    }

    return { label: "—", color: "text-gray-500" };
  };

  if (loading) {
    return <div className="p-6 text-gray-500">Loading overdue tracking...</div>;
  }

  return (
    <div className="container-center">
      <div className="card" style={{ width: "90%", maxWidth: "1000px" }}>
        <h2 className="title">Due Date Tracking</h2>

        {/* TRACKING TABLE */}
        <h3 className="subtitle">Borrowed Equipment</h3>
        <div className="table-wrapper">
          {loading ? (
            <p>Loading tracking data...</p>
          ) : (
            <table className="equipment-table">
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Equipment</th>
                  <th>Requester</th>
                  <th>Due Date</th>
                  <th>Status</th>
                  {(role === "ROLE_ADMIN" || role === "ROLE_STAFF") && <th>Actions</th>}
                </tr>
              </thead>
              <tbody>
                {borrowings.map((b) => {
                  const status = getTrackingStatus(b);
                  return (
                    <tr key={b.id}>
                      <td>{b.requestId}</td>
                      <td>{b.equipment?.name || "N/A"}</td>
                      <td>{b.requester?.userName || "N/A"}</td>
                      <td>
                        {b.returnDueDate
                          ? new Date(b.returnDueDate).toLocaleDateString()
                          : "—"}
                      </td>
                      <td className={status.className}>{status.label}</td>

                      {(role === "ROLE_ADMIN" || role === "ROLE_STAFF") && (
                        <td>
                          {status.label === "Overdue — awaiting notification" && (
                            <button
                              className="button button-blue"
                              onClick={() => alert(`Notify sent to ${b.requester?.userName}`)}
                            >
                              Notify User
                            </button>
                          )}
                        </td>
                      )}
                    </tr>
                  );
                })}
              </tbody>
            </table>
          )}
        </div>
      </div>
    </div>
  );
}