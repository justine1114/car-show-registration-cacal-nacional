import { getRegistrations, getSummary } from "./sample-data";

const API_BASE_URL =
  import.meta.env.VITE_API_BASE_URL || "http://localhost:5205";
const USE_MOCK = import.meta.env.VITE_USE_MOCK_DATA === "true";

export async function fetchRegistrations(params = {}) {
  if (USE_MOCK) {
    await new Promise((r) => setTimeout(r, 200));
    return getRegistrations(params);
  }

  const query = new URLSearchParams();
  if (params.search) query.append("participantName", params.search);
  if (params.type) query.append("type", params.type);
  if (params.status) query.append("status", params.status);
  query.append("page", String(params.page || 1));
  query.append("pageSize", String(params.pageSize || 10));

  const response = await fetch(
    `${API_BASE_URL}/api/registrations?${query.toString()}`,
  );
  if (!response.ok) {
    throw new Error(`API error (${response.status})`);
  }
  return await response.json();
}

export async function fetchRegistrationSummary() {
  if (USE_MOCK) {
    await new Promise((r) => setTimeout(r, 150));
    return getSummary();
  }

  const response = await fetch(`${API_BASE_URL}/api/registrations/summary`);
  if (!response.ok) {
    throw new Error(`Summary API error (${response.status})`);
  }
  return await response.json();
}
