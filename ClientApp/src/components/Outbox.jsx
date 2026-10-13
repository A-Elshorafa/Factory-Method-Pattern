import { cardCls, formatCost, plural, smallBtnCls } from '../styles.js'
import ChannelChip from './ChannelChip.jsx'

export default function Outbox({ entries, onView, onClear, busy }) {
  const sent = entries.filter((e) => e.status === 'Sent')
  const total = sent.reduce((sum, e) => sum + e.cost, 0)

  return (
    <section className={cardCls}>
      <div className="mb-2 flex flex-wrap items-center justify-between gap-2">
        <div>
          <h2 className="font-semibold text-slate-800">Outbox</h2>
          <p className="text-xs text-slate-500">
            {entries.length === 0 ? 'Every dispatch, sent or rejected, newest first.' : `${sent.length} sent, ${entries.length - sent.length} rejected, ${formatCost(total)} in total.`}
          </p>
        </div>
        <button className={smallBtnCls} disabled={busy || entries.length === 0} onClick={onClear}>Clear</button>
      </div>

      {entries.length === 0 ? (
        <p className="text-sm text-slate-500">Nothing sent yet.</p>
      ) : (
        <ul className="max-h-96 divide-y divide-slate-100 overflow-auto text-sm">
          {entries.map((e) => (
            <li key={e.id} className="flex flex-col gap-1 py-2 sm:flex-row sm:items-center sm:gap-3">
              <span className="flex min-w-0 flex-1 items-center gap-2">
                <span className="shrink-0 text-xs tabular-nums text-slate-400">{new Date(e.sentAt).toLocaleTimeString()}</span>
                <ChannelChip channel={e.channel} />
                <span className="min-w-0 truncate text-slate-700">{e.recipient}</span>
              </span>
              <span className="flex shrink-0 items-center gap-3">
                {e.status === 'Sent' ? (
                  <span className="text-xs tabular-nums text-slate-500">{plural(e.parts.length, 'part')} · {formatCost(e.cost)}</span>
                ) : (
                  <span className="text-xs font-medium text-rose-600">Rejected</span>
                )}
                <button className={smallBtnCls} disabled={busy} onClick={() => onView(e.id)}>View</button>
              </span>
            </li>
          ))}
        </ul>
      )}
    </section>
  )
}
