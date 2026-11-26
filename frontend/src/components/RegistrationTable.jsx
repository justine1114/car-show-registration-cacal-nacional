import StatusBadge from "./StatusBadge";
import PaymentBadge from "./PaymentBadge";

function formatDate(dateString) {
  if (!dateString) return "—";
  const date = new Date(dateString);
  return date.toLocaleDateString("en-PH", {
    month: "short",
    day: "numeric",
    year: "numeric",
  });
}

function formatCurrency(amount) {
  return Number(amount || 0).toLocaleString("en-PH", {
    style: "currency",
    currency: "PHP",
    minimumFractionDigits: 2,
  });
}

export default function RegistrationTable({ registrations }) {
  return (
    <div className="overflow-x-auto">
      <table className="min-w-full divide-y divide-slate-200 text-left text-sm">
        <thead className="bg-slate-50 text-xs font-semibold uppercase tracking-wider text-slate-600">
          <tr>
            <th className="px-4 py-3.5">Reg. No.</th>
            <th className="px-4 py-3.5">Date</th>
            <th className="px-4 py-3.5">Participant</th>
            <th className="px-4 py-3.5">Car</th>
            <th className="px-4 py-3.5">Type</th>
            <th className="px-4 py-3.5">Status</th>
            <th className="px-4 py-3.5">Fee / Paid</th>
            <th className="px-4 py-3.5">Payment</th>
          </tr>
        </thead>
        <tbody className="divide-y divide-slate-200 bg-white">
          {registrations.map((reg) => (
            <tr key={reg.id} className="hover:bg-slate-50/80 transition-colors">
              <td className="whitespace-nowrap px-4 py-3.5 font-mono text-xs font-medium text-slate-900">
                {reg.registrationNumber}
              </td>
              <td className="whitespace-nowrap px-4 py-3.5 text-xs text-slate-600">
                {formatDate(reg.registrationDate)}
              </td>
              <td className="px-4 py-3.5">
                <div className="font-medium text-slate-900">
                  {reg.participantName}
                </div>
                <div className="text-xs text-slate-500">
                  {reg.contactNumber}
                </div>
              </td>
              <td className="px-4 py-3.5">
                <div className="font-medium text-slate-800">
                  {reg.carMakeModel}
                </div>
                <div className="text-xs font-mono text-slate-500">
                  {reg.plateNumber}
                </div>
              </td>
              <td className="whitespace-nowrap px-4 py-3.5 text-xs font-medium text-slate-700">
                {reg.registrationType}
              </td>
              <td className="whitespace-nowrap px-4 py-3.5">
                <StatusBadge status={reg.status} />
              </td>
              <td className="whitespace-nowrap px-4 py-3.5 text-xs">
                <div className="font-medium text-slate-900">
                  {formatCurrency(reg.amountPaid)}
                </div>
                <div className="text-slate-500">
                  of {formatCurrency(reg.registrationFee)}
                </div>
              </td>
              <td className="whitespace-nowrap px-4 py-3.5">
                <PaymentBadge paymentStatus={reg.paymentStatus} />
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
