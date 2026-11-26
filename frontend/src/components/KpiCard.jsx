export default function KpiCard({ title, value, type = "number", subtext }) {
  const formattedValue =
    type === "currency"
      ? Number(value || 0).toLocaleString("en-PH", {
          style: "currency",
          currency: "PHP",
          minimumFractionDigits: 2,
        })
      : Number(value || 0).toLocaleString();

  return (
    <div className="rounded-xl border border-slate-200 bg-white p-5 shadow-sm transition-all hover:shadow-md">
      <p className="text-xs font-semibold tracking-wider text-slate-500 uppercase">
        {title}
      </p>
      <p className="mt-2 text-2xl font-bold tracking-tight text-slate-900">
        {formattedValue}
      </p>
      {subtext && <p className="mt-1 text-xs text-slate-500">{subtext}</p>}
    </div>
  );
}
