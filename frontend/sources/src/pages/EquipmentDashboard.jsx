import { useEffect, useState } from "react";
import axios from "axios";

export default function DashboardPage() {
  const [dashboard, setDashboard] = useState(null);
  const [equipmentList, setEquipmentList] = useState([]);
  const [filteredList, setFilteredList] = useState([]);
  const [search, setSearch] = useState("");
  const [category, setCategory] = useState("All");
  const [availability, setAvailability] = useState("All");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  // Backend URL
  const BASE_URL = "https://localhost:7124/api";
  const FULL_ENDPOINT = "https://localhost:7124/api/dashboard";

  // Fetch dashboard summary + equipment list
  useEffect(() => {
    async function fetchDashboardData() {
      try {
        setLoading(true);

        // Fetch dashboard summary
        const dashboardRes = await axios.get(`${BASE_URL}/dashboard`, {
          headers: { Accept: "application/json" },
        });

        // Fetch all equipment (or only available if needed)
        const equipmentRes = await axios.get(`${BASE_URL}/dashboard/available`, {
          params: {
            isAvailable: true
          },
          headers: {
            Accept: "application/json"
          }
        });


        setDashboard(dashboardRes.data);
        setEquipmentList(equipmentRes.data);
        setFilteredList(equipmentRes.data);
      } catch (err) {
        console.error("Error fetching dashboard:", err);
        setError("Failed to load dashboard data. Please try again later.");
      } finally {
        setLoading(false);
      }
    }

    fetchDashboardData();
  }, []);

  // Filter logic for search, category, availability
  useEffect(() => {
    let results = equipmentList;

    if (search.trim()) {
      results = results.filter((item) =>
        item.name?.toLowerCase().includes(search.toLowerCase())
      );
    }

    if (category !== "All") {
      results = results.filter((item) => item.category === category);
    }

    if (availability !== "All") {
      const isAvailable = availability === "Available";
      results = results.filter((item) => item.available === isAvailable);
    }

    setFilteredList(results);
  }, [search, category, availability, equipmentList]);

  if (loading) return <div className="text-center">Loading dashboard...</div>;
  if (error) return <div className="text-center text-red">{error}</div>;
  if (!dashboard) return <div>No dashboard data available.</div>;

  return (
    <div className="dashboard-container">
      {/* LEFT PANEL — Dashboard Summary */}
      <div className="left-panel card">
        <h2 className="title">Equipment Dashboard</h2>

        <div className="summary-row">
          <div className="summary-box total">
            <h4>Total</h4>
            <p className="count">{dashboard.totalEquipment}</p>
          </div>
          <div className="summary-box available">
            <h4>Available</h4>
            <p className="count">{dashboard.availableCount}</p>
          </div>
          <div className="summary-box lent">
            <h4>Lent Out</h4>
            <p className="count">{dashboard.lentOutCount}</p>
          </div>
        </div>

        <h3 className="subtitle">By Category</h3>
        <ul className="category-list">
          {dashboard.byCategory?.map((item, i) => (
            <li key={i} className="list-item">
              <span>{item.category}</span>
              <span className="badge">{item.count}</span>
            </li>
          ))}
        </ul>
      </div>

      {/* RIGHT PANEL — Equipment Listing */}
      <div className="right-panel card">
        <h2 className="title">Equipment Listing & Search</h2>

        <div className="filter-section">
          <input
            type="text"
            placeholder="🔍 Search equipment..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            className="input"
          />

          <select
            value={category}
            onChange={(e) => setCategory(e.target.value)}
            className="input"
          >
            <option value="All">All Categories</option>
            {dashboard.byCategory?.map((cat, i) => (
              <option key={i} value={cat.category}>
                {cat.category}
              </option>
            ))}
          </select>

          <select
            value={availability}
            onChange={(e) => setAvailability(e.target.value)}
            className="input"
          >
            <option value="All">All</option>
            <option value="Available">Available</option>
            <option value="Unavailable">Unavailable</option>
          </select>
        </div>

        <div className="table-wrapper">
          <table className="equipment-table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Name</th>
                <th>Category</th>
                <th>Availability</th>
              </tr>
            </thead>
            <tbody>
              {filteredList.length > 0 ? (
                filteredList.map((eq) => (
                  <tr key={eq.id}>
                    <td>{eq.id}</td>
                    <td>{eq.name}</td>
                    <td>{eq.category}</td>
                    <td>
                      <span
                        className={`status-badge ${
                          eq.available
                            ? "available-badge"
                            : "unavailable-badge"
                        }`}
                      >
                        {eq.available ? "Available" : "Unavailable"}
                      </span>
                    </td>
                  </tr>
                ))
              ) : (
                <tr>
                  <td colSpan="4" className="text-center text-small">
                    No equipment found
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}
