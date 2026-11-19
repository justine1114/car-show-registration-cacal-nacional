import { SAMPLE_REGISTRATIONS } from "./sample-data";

const API_BASE_URL =
  import.meta.env.VITE_API_BASE_URL || "http://localhost:5205";
const USE_MOCK = import.meta.env.VITE_USE_MOCK_DATA === "true";

const SAMPLE_SUMMARY = {
  totalRegistrations: 25,
  confirmed: 10,
  checkedIn: 6,
  pending: 6,
  cancelled: 3,
  totalFees: 61500.0,
  totalPayments: 42000.0,
  uncollectedPayments: 19500.0,
};

export async function fetchRegistrations({
  search = "",
  type = "",
  status = "",
  page = 1,
  pageSize = 10,
} = {}) {
  if (USE_MOCK) {
    // Simulate network delay
    await new Promise((r) => setTimeout(r, 250));

    const filtered = SAMPLE_REGISTRATIONS.filter((reg) => {
      const matchesName = reg.participantName
        .toLowerCase()
        .includes(search.toLowerCase());
      const matchesType = type ? reg.registrationType === type : true;
      const matchesStatus = status ? reg.status === status : true;
      return matchesName && matchesType && matchesStatus;
    });

    const totalCount = filtered.length;
    const totalPages = Math.max(1, Math.ceil(totalCount / pageSize));
    const safePage = Math.min(page, totalPages);
    const startIndex = (safePage - 1) * pageSize;
    const items = filtered.slice(startIndex, startIndex + pageSize);

    return {
      items,
      page: safePage,
      pageSize,
      totalCount,
      totalPages,
    };
  }

  const params = new URLSearchParams();
  if (search) params.append("search", search);
  if (type) params.append("type", type);
  if (status) params.append("status", status);
  params.append("page", String(page));
  params.append("pageSize", String(pageSize));

  const response = await fetch(
    `${API_BASE_URL}/api/registrations?${params.toString()}`,
  );
  if (!response.ok) {
    throw new Error(`API error (${response.status})`);
  }
  return await response.json();
}

export async function fetchRegistrationSummary() {
  if (USE_MOCK) {
    await new Promise((r) => setTimeout(r, 200));
    return SAMPLE_SUMMARY;
  }

  const response = await fetch(`${API_BASE_URL}/api/registrations/summary`);
  if (!response.ok) {
    throw new Error(`Summary API error (${response.status})`);
  }
  return await response.json();
}
