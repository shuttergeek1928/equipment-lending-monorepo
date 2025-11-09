import { useEffect, useState } from "react";
import api from "axios"; // axios instance

export default function EquipmentManagement() {
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
    fetchEquipment();
  }, []);

  const fetchEquipment = async () => {
    try {
      setLoading(true);
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
    finally {
      setLoading(false);
    }
  };
  
  // ADMIN — Add Equipment
  const handleAddEquipment = async () => {
    if (!newEquipment.EquipmentName || newEquipment.AvailableQuantity < 1) {
      alert("Enter valid equipment name and quantity!");
      return;
    }

    const payload = {
      EquipmentName: newEquipment.EquipmentName,
      AvailableQuantity: newEquipment.AvailableQuantity,
    };

    try {
      // Replace with real endpoint 
      await api.post("/equipment", payload);
      alert("Equipment added successfully!");
      fetchEquipment();
    } 
    catch (err) {
      console.warn("Add equipment failed:", err);
      alert("Backend endpoint for adding equipment not available yet.");
    }

    setNewEquipment({ EquipmentName: "", AvailableQuantity: 1 });
  };

  // ADMIN — Delete Equipment
  const handleDeleteEquipment = async (EquipmentId) => {
    try {
      // Replace with real endpoint if available
      await api.delete(`/equipment/${EquipmentId}`);
      alert("Equipment deleted successfully!");
      fetchEquipment();
    } 
    catch (err) {
      console.warn("Delete equipment failed:", err);
      alert("Backend endpoint for deleting equipment not available yet.");
    }
  };
  
  // html structure
  return (
    <div className="container-center">
      <div className="card" style={{ width: "90%", maxWidth: "1000px" }}>
        <h2 className="title">Equipment Management</h2>
        
        {/* ADMIN — Manage Equipment */}
        {/* Need to check logic for this whether it will change the available quantity or total 
        or is it like adding a new equipment altogether*/}
        {role === "ROLE_ADMIN" && (
          <div className="form-section">
            <h3 className="subtitle">Manage Equipment</h3>
            <input
              type="text"
              placeholder="Equipment Name"
              value={newEquipment.EquipmentName}
              onChange={(e) =>
                setNewEquipment({ ...newEquipment, EquipmentName: e.target.value })
              }
              className="input"
            />
            <input
              type="number"
              placeholder="Available Quantity"
              min="1"
              value={newEquipment.AvailableQuantity}
              onChange={(e) =>
                setNewEquipment({
                  ...newEquipment,
                  AvailableQuantity: Number(e.target.value),
                })
              }
              className="input"
            />
            <button className="button" onClick={handleAddEquipment}>
              Add Equipment
            </button>

             {/* this deletes existing element completely!! */}
            <ul className="list">
              {equipmentList.map((eq) => (
                <li key={eq.id} className="list-item">
                  <span>
                    {eq.EquipmentName} (Qty: {eq.AvailableQuantity})
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
      </div>
    </div>
  );
}
