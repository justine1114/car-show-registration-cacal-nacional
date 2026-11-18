import KpiCard from "./KpiCard";

export default function KpiCards({ summary }) {
  // Fallback defaults if summary hasn't loaded yet
  const data = summary || {
    totalRegistrations: 0,
    totalPayments: 0,
    uncollectedPayments: 0,
    confirmed: 0,
    checkedIn: 0,
  };

  return (
    <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-5 mb-6">
      <KpiCard
        title="Total Registrations"
        value={data.totalRegistrations}
        type="number"
      />
      <KpiCard
        title="Total Payments"
        value={data.totalPayments}
        type="currency"
      />
      <KpiCard
        title="Uncollected Payments"
        value={data.uncollectedPayments}
        type="currency"
      />
      <KpiCard title="Confirmed" value={data.confirmed} type="number" />
      <KpiCard title="Checked In" value={data.checkedIn} type="number" />
    </div>
  );
}
