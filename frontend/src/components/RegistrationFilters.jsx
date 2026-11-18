export default function RegistrationFilters({
  searchQuery,
  setSearchQuery,
  selectedType,
  setSelectedType,
  selectedStatus,
  setSelectedStatus,
  onReset,
}) {
  const hasActiveFilters = searchQuery || selectedType || selectedStatus;

  return (
    <div className="mb-6 flex flex-col gap-3 rounded-xl border border-slate-200 bg-white p-4 shadow-sm sm:flex-row sm:items-center sm:justify-between">
      <div className="flex flex-1 flex-col gap-3 sm:flex-row sm:items-center">
        {/* Search Input */}
        <div className="relative flex-1">
          <span className="absolute inset-y-0 left-0 flex items-center pl-3 text-slate-400">
            <svg
              className="h-4 w-4"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"
              />
            </svg>
          </span>
          <input
            type="text"
            placeholder="Search participant name..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            className="w-full rounded-lg border border-slate-300 bg-white py-2 pr-4 pl-9 text-sm text-slate-900 placeholder-slate-400 focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
          />
        </div>

        {/* Type Filter */}
        <select
          value={selectedType}
          onChange={(e) => setSelectedType(e.target.value)}
          className="rounded-lg border border-slate-300 bg-white py-2 px-3 text-sm text-slate-700 focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
        >
          <option value="">All Types</option>
          <option value="Exhibitor">Exhibitor</option>
          <option value="Competitor">Competitor</option>
          <option value="VIP">VIP</option>
        </select>

        {/* Status Filter */}
        <select
          value={selectedStatus}
          onChange={(e) => setSelectedStatus(e.target.value)}
          className="rounded-lg border border-slate-300 bg-white py-2 px-3 text-sm text-slate-700 focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
        >
          <option value="">All Statuses</option>
          <option value="Pending">Pending</option>
          <option value="Confirmed">Confirmed</option>
          <option value="CheckedIn">Checked In</option>
          <option value="Cancelled">Cancelled</option>
        </select>
      </div>

      {/* Clear Filters Button */}
      {hasActiveFilters && (
        <button
          onClick={onReset}
          className="inline-flex items-center justify-center rounded-lg border border-slate-300 bg-slate-50 px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-100 transition-colors"
        >
          Clear filters
        </button>
      )}
    </div>
  );
}
