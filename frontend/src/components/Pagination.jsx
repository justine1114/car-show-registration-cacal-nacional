export default function Pagination({
  page,
  pageSize,
  totalCount,
  totalPages,
  onPageChange,
  onPageSizeChange,
}) {
  if (totalCount === 0) return null;

  const startItem = (page - 1) * pageSize + 1;
  const endItem = Math.min(page * pageSize, totalCount);

  const pages = Array.from({ length: totalPages }, (_, i) => i + 1);

  return (
    <div className="flex flex-col items-center justify-between gap-4 border-t border-slate-200 bg-slate-50/50 px-4 py-3 sm:flex-row">
      {/* Page navigation buttons */}
      <div className="flex items-center gap-1">
        <button
          onClick={() => onPageChange(page - 1)}
          disabled={page <= 1}
          className="rounded-lg border border-slate-300 bg-white px-2.5 py-1.5 text-xs font-medium text-slate-700 hover:bg-slate-50 disabled:cursor-not-allowed disabled:opacity-40"
        >
          &lt;
        </button>

        {pages.map((p) => (
          <button
            key={p}
            onClick={() => onPageChange(p)}
            className={`rounded-lg border px-3 py-1.5 text-xs font-medium transition-colors ${
              p === page
                ? "border-blue-600 bg-blue-600 text-white"
                : "border-slate-300 bg-white text-slate-700 hover:bg-slate-50"
            }`}
          >
            {p}
          </button>
        ))}

        <button
          onClick={() => onPageChange(page + 1)}
          disabled={page >= totalPages}
          className="rounded-lg border border-slate-300 bg-white px-2.5 py-1.5 text-xs font-medium text-slate-700 hover:bg-slate-50 disabled:cursor-not-allowed disabled:opacity-40"
        >
          &gt;
        </button>
      </div>

      {/* Showing x-y of N + Per page selector */}
      <div className="flex items-center gap-4 text-xs text-slate-600">
        <span>
          Showing{" "}
          <span className="font-medium text-slate-900">{startItem}</span>–
          <span className="font-medium text-slate-900">{endItem}</span> of{" "}
          <span className="font-medium text-slate-900">{totalCount}</span>
        </span>

        <select
          value={pageSize}
          onChange={(e) => onPageSizeChange(Number(e.target.value))}
          className="rounded-lg border border-slate-300 bg-white px-2.5 py-1.5 text-xs text-slate-700 focus:border-blue-500 focus:outline-none"
        >
          <option value={5}>5 / page</option>
          <option value={10}>10 / page</option>
          <option value={25}>25 / page</option>
        </select>
      </div>
    </div>
  );
}
