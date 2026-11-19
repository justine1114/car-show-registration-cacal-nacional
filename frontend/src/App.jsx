import { useState } from "react";
import KpiCards from "./components/KpiCards";
import RegistrationFilters from "./components/RegistrationFilters";
import RegistrationTable from "./components/RegistrationTable";
import { useDebounce } from "./hooks/useDebounce";
import { SAMPLE_REGISTRATIONS } from "./api/sample-data";

const sampleSummary = {
  totalRegistrations: 25,
  confirmed: 10,
  checkedIn: 6,
  pending: 6,
  cancelled: 3,
  totalFees: 61500.0,
  totalPayments: 42000.0,
  uncollectedPayments: 19500.0,
};

export default function App() {
  const [searchQuery, setSearchQuery] = useState("");
  const [selectedType, setSelectedType] = useState("");
  const [selectedStatus, setSelectedStatus] = useState("");

  const debouncedSearch = useDebounce(searchQuery, 400);

  const filteredRegistrations = SAMPLE_REGISTRATIONS.filter((reg) => {
    const matchesName = reg.participantName
      .toLowerCase()
      .includes(debouncedSearch.toLowerCase());
    const matchesType = selectedType
      ? reg.registrationType === selectedType
      : true;
    const matchesStatus = selectedStatus ? reg.status === selectedStatus : true;
    return matchesName && matchesType && matchesStatus;
  });

  const handleReset = () => {
    setSearchQuery("");
    setSelectedType("");
    setSelectedStatus("");
  };

  return (
    <div className="min-h-screen bg-slate-50 p-6 text-slate-900">
      <div className="mx-auto max-w-7xl">
        <h1 className="mb-1 text-2xl font-bold">Car Show 2027 — Dashboard</h1>
        <p className="mb-6 text-sm text-slate-600">
          Event Staff Registration & Analytics Portal
        </p>

        <KpiCards summary={sampleSummary} />

        <RegistrationFilters
          searchQuery={searchQuery}
          setSearchQuery={setSearchQuery}
          selectedType={selectedType}
          setSelectedType={setSelectedType}
          selectedStatus={selectedStatus}
          setSelectedStatus={setSelectedStatus}
          onReset={handleReset}
        />

        <div className="overflow-hidden rounded-xl border border-slate-200 bg-white shadow-sm">
          <div className="flex items-center justify-between border-b border-slate-200 bg-slate-50 px-4 py-3">
            <h2 className="font-semibold text-slate-800">Registrations</h2>
            <span className="text-xs text-slate-500">
              Showing {filteredRegistrations.length} results
            </span>
          </div>
          {filteredRegistrations.length > 0 ? (
            <RegistrationTable registrations={filteredRegistrations} />
          ) : (
            <div className="p-8 text-center text-sm text-slate-500">
              No registrations match your filters.
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
