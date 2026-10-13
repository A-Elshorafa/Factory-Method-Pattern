import { ROLE_TONES } from '../styles.js'

// What happened during one dispatch, labelled with the participant of the pattern that did it.
export default function StepList({ steps }) {
  return (
    <ol className="relative space-y-3 border-l-2 border-slate-200 pl-4 text-sm">
      {steps.map((s, i) => (
        <li key={i} className="relative">
          <span className="absolute -left-[23px] top-1 h-3 w-3 rounded-full bg-white ring-2 ring-slate-300" />
          <div className="flex flex-wrap items-center gap-2">
            <span className={`rounded px-1.5 py-0.5 text-[11px] font-semibold ${ROLE_TONES[s.role]}`}>{s.role}</span>
            <span className="font-mono text-xs text-slate-600">{s.actor}</span>
          </div>
          <p className="mt-0.5 break-words text-slate-800">{s.action}</p>
        </li>
      ))}
    </ol>
  )
}
