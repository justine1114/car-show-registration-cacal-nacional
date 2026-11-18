import StatusBadge from "./components/StatusBadge";
import PaymentBadge from "./components/PaymentBadge";
import KpiCards from "./components/KpiCards";
import { SAMPLE_REGISTRATIONS } from "./api/sample-data";

// Sample summary object matching GET /api/registrations/summary
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
  return (
    <div className="min-h-screen bg-slate-50 p-6 text-slate-900">
      <div className="mx-auto max-w-7xl">
        <h1 className="mb-1 text-2xl font-bold">Car Show 2027 — Dashboard</h1>
        <p className="mb-6 text-sm text-slate-600">
          Event Staff Registration & Analytics Portal
        </p>

        {/* KPI Cards Row */}
        <KpiCards summary={sampleSummary} />

        {/* Existing Badge Preview Section */}
        <div className="rounded-xl border border-slate-200 bg-white shadow-sm overflow-hidden">
          <div className="border-b border-slate-200 bg-slate-50 px-4 py-3">
            <h2 className="font-semibold text-slate-800">
              Registrations Preview
            </h2>
          </div>
          <div className="divide-y divide-slate-200">
            {SAMPLE_REGISTRATIONS.map((reg) => (
              <div
                key={reg.id}
                className="flex items-center justify-between p-4"
              >
                <div>
                  <p className="font-medium text-slate-900">
                    {reg.participantName}{" "}
                    <span className="text-xs font-normal text-slate-500">
                      ({reg.registrationNumber})
                    </span>
                  </p>
                  <p className="text-xs text-slate-500">
                    {reg.carMakeModel} • {reg.plateNumber} •{" "}
                    {reg.registrationType}
                  </p>
                </div>
                <div className="flex items-center gap-2">
                  <StatusBadge status={reg.status} />
                  <PaymentBadge paymentStatus={reg.paymentStatus} />
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
}
