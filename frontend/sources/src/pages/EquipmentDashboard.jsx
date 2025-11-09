import { useEffect, useState } from "react";
import api from "axios"; // axios instance

export default function BorrowingRequests() {
  // fetch the logged in role and user
  const [role] = useState(localStorage.getItem("roles") || "ROLE_STUDENT");
  const [user] = useState(localStorage.getItem("username"));

  // State management
  const [equipmentList, setEquipmentList] = useState([]);
  const [requests, setRequests] = useState([]);
  const [newRequest, setNewRequest] = useState({ EquipmentId: "", RequestedQuantity: 1 });
  const [newEquipment, setNewEquipment] = useState({ EquipmentName: "", AvailableQuantity: 1 });
  const [loading, setLoading] = useState(false);

  const currentUser = { id: 101, name: "Alice" }; // mock current user

  // Backend URL
  //const BASE_URL = "https://localhost:7124/api";
  //const FULL_ENDPOINT = "https://localhost:7124/api/dashboard";

  // Load borrowings and equipment
  useEffect(() => {
    fetchBorrowings();
    fetchEquipment();
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
      setRequests(data);
    } 
    catch (err) {
      console.error("Error fetching borrowings:", err);
      alert("Failed to load borrowings.");
    } 
    finally {
      setLoading(false);
    }
  };

  // fetch equipments
  const fetchEquipment = async () => {
    try {
      const res = await api.get("/dashboard/available?isAvailable=true");
      const data = Array.isArray(res.data)
        ? res.data
        : Array.isArray(res.data.data)
        ? res.data.data
        : [];
      setEquipmentList(data|| []);
    } 
    catch (err) {
      console.warn("Equipment fetch fallback:", err);
      setEquipmentList([]);
    }
  };

  // STUDENT — Submit borrow request
  const handleRequest = async () => {
    // check if new request has proper data
    if (!newRequest.EquipmentId || newRequest.RequestedQuantity < 1) {
      alert("Please select valid equipment and quantity!");
      return;
    }

    // set the payload data
    const payload = {
      EquipmentId: Number(newRequest.EquipmentId),
      RequesterId: currentUser.id, // need to check this and see how is request id set?
      RequestedQuantity: Number(newRequest.RequestedQuantity),
      RequestedOn: new Date().toISOString().split("T")[0],
    };

    try {
      // sending payload data in post
      await api.post("/request", payload);
      alert("Request submitted successfully!");
      setNewRequest({ EquipmentId: "", RequestedQuantity: 1 });
      fetchBorrowings();
    } 
    catch (err) {
      const msg =
        err?.response?.data?.error?.message || err.message || "Unknown error";
      alert("Error submitting request: " + msg);
    }
  };

  // For approving and handling logic, RequestId is used - check
  // STAFF / ADMIN — Approve request
  const handleApprove = async (RequestId) => {
    try {
      await api.put(`/approve/${RequestId}`);
      alert("Request approved!");
      fetchBorrowings();
    } 
    catch (err) {
      alert("Error approving request: " + err.message);
    }
  };

  // STAFF / ADMIN - Reject request - need separate ?

  // STAFF / ADMIN — Mark returned
  const handleReturn = async (RequestId) => {
    try {
      await api.put(`/return/${RequestId}`);
      alert("Marked as returned!");
      fetchBorrowings();
    } 
    catch (err) {
      alert("Error marking as returned: " + err.message);
    }
  };

  // Visible requests based on role  
  const visibleRequests = Array.isArray(requests)
    ? role === "ROLE_STUDENT"
      ? requests.filter((r) => r.RequesterId === currentUser.id)
      : requests
    : [];

  return (
    <div className="container-center">
      <div className="card" style={{ width: "90%", maxWidth: "1000px" }}>
        <h2 className="title">Borrowing & Return Management</h2>

        {/* STUDENT — Request Form */}
        {role === "ROLE_STUDENT" && (
          <div className="form-section">
            <h3 className="subtitle">Request Equipment</h3>

            {/* on changing the value, it will set the equipment id in the new request */}
            <select
              value={newRequest.EquipmentId}              
              onChange={(e) =>
                setNewRequest({ ...newRequest, EquipmentId: e.target.value })
              }
              className="input"
              style={{ marginRight: "10px" }}
            >
              {/* this will display the equipment name and available quantity in dropdown*/}
              <option value="">Select Equipment</option>
              {equipmentList.map((eq) => (
                <option key={eq.id} value={eq.id}>
                  {eq.EquipmentName} (Available: {eq.AvailableQuantity})
                </option>
              ))}
            </select>

            {/* on changing the value, it will set the requested quantity in the new request */}
            <input
              type="number"
              placeholder="Quantity"
              min="1"
              value={newRequest.RequestedQuantity}              
              onChange={(e) =>
                setNewRequest({
                  ...newRequest,
                  RequestedQuantity: Number(e.target.value),
                })
              }
              className="input"
              style={{ marginRight: "10px" }}
            />

            {/* on clicking button, handle request function will be called */}
            <button className="button" onClick={handleRequest}>
              Submit Request
            </button>
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
                  <th>Quantity</th>
                  <th>Status</th>
                  {role !== "ROLE_STUDENT" && <th>Requester</th>}
                  {(role === "ROLE_STAFF" || role === "ROLE_ADMIN") && (
                    <th>Actions</th>
                  )}
                </tr>
              </thead>
              <tbody>
                {visibleRequests.map((req) => (
                  <tr key={req.RequestId}>
                    <td>{req.RequestId}</td>
                    <td>{req.EquipmentName}</td>
                    <td>{req.RequestedQuantity}</td>
                    <td>{req.isApproved ? "Approved" : "Pending"}</td>

                    {/* Check logic for getting requester name */}
                    {role !== "ROLE_STUDENT" && <td>{req.RequesterName}</td>}

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
