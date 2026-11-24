import { useState, useEffect, useCallback } from "react";
import KpiCards from "./components/KpiCards";
import RegistrationFilters from "./components/RegistrationFilters";
import RegistrationTable from "./components/RegistrationTable";
import Pagination from "./components/Pagination";
import { useDebounce } from "./hooks/useDebounce";
import {
  fetchRegistrations,
  fetchRegistrationSummary,
} from "./api/registrationApi";

export default function App() {
  const [registrations, setRegistrations] = useState([]);
  const [summary, setSummary] = useState(null);

  const [searchQuery, setSearchQuery] = useState("");
  const [selectedType, setSelectedType] = useState("");
  const [selectedStatus, setSelectedStatus] = useState("");

  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [totalCount, setTotalCount] = useState(0);
  const [totalPages, setTotalPages] = useState(1);

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const debouncedSearch = useDebounce(searchQuery, 400);

  // Reset to page 1 when filters change
  useEffect(() => {
    setPage(1);
  }, [debouncedSearch, selectedType, selectedStatus, pageSize]);

  const loadDashboardData = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);

      const [regResponse, summaryResponse] = await Promise.all([
        fetchRegistrations({
          search: debouncedSearch,
          type: selectedType,
          status: selectedStatus,
          page,
          pageSize,
        }),
        fetchRegistrationSummary(),
      ]);

      setRegistrations(regResponse.items || []);
      setTotalCount(regResponse.totalCount ?? 0);
      setTotalPages(regResponse.totalPages ?? 1);
      setSummary(summaryResponse);
    } catch {
      setError(
        "Cannot reach the API. Please check that the backend server is running.",
      );
    } finally {
      setLoading(false);
    }
  }, [debouncedSearch, selectedType, selectedStatus, page, pageSize]);

  useEffect(() => {
    loadDashboardData();
  }, [loadDashboardData]);

  const handleReset = () => {
    setSearchQuery("");
    setSelectedType("");
    setSelectedStatus("");
    setPage(1);
  };

  return (
    <div className="min-h-screen bg-slate-50 p-6 text-slate-900">
      <div className="mx-auto max-w-7xl">
        {/* Header */}
        <header className="mb-6 flex flex-col justify-between gap-2 sm:flex-row sm:items-center">
          <div>
            <h1 className="text-2xl font-bold tracking-tight text-slate-900">
              Car Show 2027
            </h1>
            <p className="text-sm text-slate-600">
              Registration & Payment Monitoring Dashboard
            </p>
          </div>
          <span className="inline-flex w-fit items-center rounded-full bg-slate-200/70 px-3 py-1 text-xs font-medium text-slate-700">
            Staff Portal
          </span>
        </header>

        {/* KPI Summary Row */}
        <KpiCards summary={summary} />

        {/* Main Card Container */}
        <div className="overflow-hidden rounded-xl border border-slate-200 bg-white shadow-sm">
          {/* Card Header + Inline Filters */}
          <div className="border-b border-slate-200 p-4">
            <div className="mb-3 flex items-center justify-between">
              <h2 className="text-base font-semibold text-slate-800">
                Registrations
              </h2>
            </div>
            <RegistrationFilters
              searchQuery={searchQuery}
              setSearchQuery={setSearchQuery}
              selectedType={selectedType}
              setSelectedType={setSelectedType}
              selectedStatus={selectedStatus}
              setSelectedStatus={setSelectedStatus}
              onReset={handleReset}
            />
          </div>

          {/* States: Error -> Loading -> Empty -> Table */}
          {error ? (
            <div className="p-10 text-center">
              <p className="mb-3 text-sm font-medium text-red-600">{error}</p>
              <button
                onClick={loadDashboardData}
                className="rounded-lg bg-red-600 px-4 py-2 text-xs font-medium text-white hover:bg-red-700 transition-colors"
              >
                Try again
              </button>
            </div>
          ) : loading ? (
            <div className="p-12 text-center text-sm text-slate-500">
              Loading registrations...
            </div>
          ) : registrations.length === 0 ? (
            <div className="p-12 text-center">
              <p className="mb-3 text-sm text-slate-600">
                No registrations match your filters.
              </p>
              <button
                onClick={handleReset}
                className="rounded-lg border border-slate-300 bg-white px-4 py-2 text-xs font-medium text-slate-700 hover:bg-slate-50 transition-colors"
              >
                Clear filters
              </button>
            </div>
          ) : (
            <>
              <RegistrationTable registrations={registrations} />
              <Pagination
                page={page}
                pageSize={pageSize}
                totalCount={totalCount}
                totalPages={totalPages}
                onPageChange={setPage}
                onPageSizeChange={setPageSize}
              />
            </>
          )}
        </div>
      </div>
    </div>
  );
}
