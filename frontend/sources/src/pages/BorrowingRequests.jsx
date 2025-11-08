import { useEffect, useState } from "react";
import api from "axios"; // axios instance

export default function BorrowingRequests() {
  // Logged-in role
  const [role] = useState(localStorage.getItem("roles") || "ROLE_STUDENT");

  // State management
  const [equipmentList, setEquipmentList] = useState([]);
  const [requests, setRequests] = useState([]);
  const [newRequest, setNewRequest] = useState({ equipmentId: "", quantity: 1 });
  const [newEquipment, setNewEquipment] = useState({ name: "", availableQty: 1 });
  const [loading, setLoading] = useState(false);

  const currentUser = { id: 101, name: "Alice" }; // mock current user

  // Load borrowings and equipment
  useEffect(() => {
    fetchBorrowings();
    fetchEquipment();
  }, []);

  const fetchBorrowings = async () => {
    try {
      setLoading(true);
      const res = await api.get("/borrowings");
      setRequests(res.data);
    } catch (err) {
      console.error("Error fetching borrowings:", err);
      alert("Failed to load borrowings.");
    } finally {
      setLoading(false);
    }
  };

  const fetchEquipment = async () => {
    try {
      const res = await api.get("/dashboard/available?isAvailable=true");
      setEquipmentList(res.data || []);
    } catch (err) {
      console.warn("Equipment fetch fallback:", err);
      setEquipmentList([]);
    }
  };

  // STUDENT — Submit borrow request
  const handleRequest = async () => {
    if (!newRequest.equipmentId || newRequest.quantity < 1) {
      alert("Please select valid equipment and quantity!");
      return;
    }

    const payload = {
      equipmentId: Number(newRequest.equipmentId),
      requesterId: currentUser.id,
      quantity: Number(newRequest.quantity),
      requestedOn: new Date().toISOString().split("T")[0],
    };

    try {
      await api.post("/request", payload);
      alert("Request submitted successfully!");
      setNewRequest({ equipmentId: "", quantity: 1 });
      fetchBorrowings();
    } catch (err) {
      const msg =
        err?.response?.data?.error?.message || err.message || "Unknown error";
      alert("Error submitting request: " + msg);
    }
  };

  // STAFF / ADMIN — Approve
  const handleApprove = async (id) => {
    try {
      await api.put(`/approve/${id}`);
      alert("Request approved!");
      fetchBorrowings();
    } catch (err) {
      alert("Error approving request: " + err.message);
    }
  };

  // STAFF / ADMIN — Mark returned
  const handleReturn = async (id) => {
    try {
      await api.put(`/return/${id}`);
      alert("Marked as returned!");
      fetchBorrowings();
    } catch (err) {
      alert("Error marking as returned: " + err.message);
    }
  };

  // ADMIN — Add Equipment
  const handleAddEquipment = async () => {
    if (!newEquipment.name || newEquipment.availableQty < 1) {
      alert("Enter valid equipment name and quantity!");
      return;
    }

    const payload = {
      name: newEquipment.name,
      availableQty: newEquipment.availableQty,
    };

    try {
      // Replace with real endpoint if available
      await api.post("/equipment", payload);
      alert("Equipment added successfully!");
      fetchEquipment();
    } catch (err) {
      console.warn("Add equipment failed:", err);
      alert("Backend endpoint for adding equipment not available yet.");
    }

    setNewEquipment({ name: "", availableQty: 1 });
  };

  // ADMIN — Delete Equipment
  const handleDeleteEquipment = async (id) => {
    try {
      // Replace with real endpoint if available
      await api.delete(`/equipment/${id}`);
      alert("Equipment deleted successfully!");
      fetchEquipment();
    } catch (err) {
      console.warn("Delete equipment failed:", err);
      alert("Backend endpoint for deleting equipment not available yet.");
    }
  };

  // Visible requests based on role
  const visibleRequests = 
    role === "ROLE_STUDENT"
      ? requests.filter((r) => r.requesterId === currentUser.id)
      : requests;

  return (
    <div className="container-center">
      <div className="card" style={{ width: "90%", maxWidth: "1000px" }}>
        <h2 className="title">Borrowing & Return Management</h2>

        {/* STUDENT — Request Form */}
        {role === "ROLE_STUDENT" && (
          <div className="form-section">
            <h3 className="subtitle">Request Equipment</h3>
            <select
              value={newRequest.equipmentId}
              onChange={(e) =>
                setNewRequest({ ...newRequest, equipmentId: e.target.value })
              }
              className="input"
              style={{ marginRight: "10px" }}
            >
              <option value="">Select Equipment</option>
              {equipmentList.map((eq) => (
                <option key={eq.id} value={eq.id}>
                  {eq.name} (Available: {eq.availableQty})
                </option>
              ))}
            </select>

            <input
              type="number"
              placeholder="Quantity"
              min="1"
              value={newRequest.quantity}
              onChange={(e) =>
                setNewRequest({
                  ...newRequest,
                  quantity: Number(e.target.value),
                })
              }
              className="input"
              style={{ marginRight: "10px" }}
            />

            <button className="button" onClick={handleRequest}>
              Submit Request
            </button>
          </div>
        )}

        {/* ADMIN — Manage Equipment */}
        {role === "ROLE_ADMIN" && (
          <div className="form-section">
            <h3 className="subtitle">Manage Equipment</h3>
            <input
              type="text"
              placeholder="Equipment Name"
              value={newEquipment.name}
              onChange={(e) =>
                setNewEquipment({ ...newEquipment, name: e.target.value })
              }
              className="input"
            />
            <input
              type="number"
              placeholder="Available Quantity"
              min="1"
              value={newEquipment.availableQty}
              onChange={(e) =>
                setNewEquipment({
                  ...newEquipment,
                  availableQty: Number(e.target.value),
                })
              }
              className="input"
            />
            <button className="button" onClick={handleAddEquipment}>
              Add Equipment
            </button>

            <ul className="list">
              {equipmentList.map((eq) => (
                <li key={eq.id} className="list-item">
                  <span>
                    {eq.name} (Qty: {eq.availableQty})
                  </span>
                  <button
                    className="button button-red"
                    onClick={() => handleDeleteEquipment(eq.id)}
                  >
                    Delete
                  </button>
                </li>
              ))}
            </ul>
          </div>
        )}

        {/* REQUEST TABLE */}
        <h3 className="subtitle">Requests</h3>
        <div className="table-wrapper">
          {loading ? (
            <p>Loading requests...</p>
          ) : (
            <table className="equipment-table">
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Equipment</th>
                  <th>Qty</th>
                  <th>Status</th>
                  {role !== "ROLE_STUDENT" && <th>Requester</th>}
                  {(role === "ROLE_STAFF" || role === "ROLE_ADMIN") && (
                    <th>Actions</th>
                  )}
                </tr>
              </thead>
              <tbody>
                {visibleRequests.map((req) => (
                  <tr key={req.id}>
                    <td>{req.id}</td>
                    <td>{req.equipmentName}</td>
                    <td>{req.quantity}</td>
                    <td>{req.status}</td>
                    {role !== "ROLE_STUDENT" && <td>{req.requesterName}</td>}

                    {(role === "ROLE_STAFF" || role === "ROLE_ADMIN") && (
                      <td>
                        {req.status === "Pending" && (
                          <button
                            className="button button-green"
                            onClick={() => handleApprove(req.id)}
                          >
                            Approve
                          </button>
                        )}

                        {req.status === "Approved" && (
                          <button
                            className="button button-blue"
                            onClick={() => handleReturn(req.id)}
                          >
                            Mark Returned
                          </button>
                        )}
                      </td>
                    )}
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
