import StatusBadge from "./components/StatusBadge";
import PaymentBadge from "./components/PaymentBadge";
import { SAMPLE_REGISTRATIONS } from "./api/sample-data";

export default function App() {
  return (
    <div className="min-h-screen bg-slate-50 p-6 text-slate-900">
      <div className="mx-auto max-w-3xl">
        <h1 className="mb-1 text-2xl font-bold">
          Car Show 2027 — Badge Preview
        </h1>
        <p className="mb-4 text-sm text-slate-600">
          Testing StatusBadge and PaymentBadge with sample registrations
        </p>

        <div className="divide-y divide-slate-200 rounded-lg border border-slate-200 bg-white shadow-sm">
          {SAMPLE_REGISTRATIONS.map((reg) => (
            <div key={reg.id} className="flex items-center justify-between p-4">
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
  );
}
