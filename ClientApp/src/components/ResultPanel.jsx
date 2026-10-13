import { cardCls, formatCost, plural } from '../styles.js'
import ChannelChip from './ChannelChip.jsx'
import MessageView from './MessageView.jsx'
import StepList from './StepList.jsx'

function Header({ title, subtitle, tone = 'text-slate-800' }) {
  return (
    <div className="mb-3">
      <h2 className={`font-semibold ${tone}`}>{title}</h2>
      {subtitle && <div className="mt-0.5 text-xs text-slate-500">{subtitle}</div>}
    </div>
  )
}

function SendResult({ data }) {
  const sent = data.status === 'Sent'
  return (
    <>
      <Header
        title={sent ? `Sent ${data.id}` : `${data.product} rejected the recipient`}
        tone={sent ? 'text-slate-800' : 'text-rose-700'}
        subtitle={
          <span className="inline-flex flex-wrap items-center gap-1.5">
            <ChannelChip channel={data.channel} />
            <span>to {data.recipient}</span>
            {sent && <span>· {plural(data.parts.length, 'part')} · {formatCost(data.cost)}</span>}
          </span>
        }
      />
      {!sent && (
        <div className="mb-3 rounded-lg bg-rose-50 p-3 text-sm text-rose-800 ring-1 ring-rose-200">
          {data.error}
        </div>
      )}

      <div className="grid gap-4 md:grid-cols-2">
        <div>
          <h3 className="mb-2 text-xs font-semibold uppercase tracking-wide text-slate-500">Steps</h3>
          <StepList steps={data.steps} />
        </div>
        {sent && (
          <div className="min-w-0">
            <h3 className="mb-2 text-xs font-semibold uppercase tracking-wide text-slate-500">What the recipient gets</h3>
            <MessageView channel={data.channel} parts={data.parts.map((p) => ({ content: p.content, meta: p.providerMessageId }))} />
          </div>
        )}
      </div>

      <p className="mt-4 text-xs text-slate-500">
        {data.creator} only ran the first step. Everything after it is the same <code>NotificationDispatcher</code> code for
        every channel, calling the <code>INotificationChannel</code> that <code>CreateChannel()</code> returned.
      </p>
    </>
  )
}

function PreviewResult({ data }) {
  const cheapest = Math.min(...data.channels.map((c) => c.cost))
  return (
    <>
      <Header
        title="One message, every channel"
        subtitle={`Title ${data.titleLength} chars, body ${data.bodyLength} chars. Each dispatcher rendered it with the channel its factory method created.`}
      />
      <div className="grid gap-3 md:grid-cols-2">
        {data.channels.map((c) => (
          <div key={c.channel} className="min-w-0 rounded-lg bg-slate-50 p-3 ring-1 ring-slate-200">
            <div className="mb-1 flex items-center justify-between gap-2">
              <span className="inline-flex items-center gap-1.5">
                <ChannelChip channel={c.channel} />
                <span className="font-mono text-[11px] text-slate-500">{c.product}</span>
              </span>
              <span className={`text-sm font-semibold tabular-nums ${c.cost === cheapest ? 'text-emerald-700' : 'text-slate-700'}`}>
                {formatCost(c.cost)}
              </span>
            </div>
            <p className="mb-2 text-xs text-slate-500">{plural(c.parts.length, 'part')}. {c.note}</p>
            <MessageView channel={c.channel} parts={c.parts.map((content) => ({ content }))} />
          </div>
        ))}
      </div>
    </>
  )
}

export default function ResultPanel({ result }) {
  if (!result) {
    return (
      <section className={cardCls + ' flex items-center justify-center text-center text-sm text-slate-500'}>
        Send a notification to see the factory method create its channel, or preview the message on every channel.
      </section>
    )
  }

  return (
    <section className={cardCls + ' self-start'}>
      {result.kind === 'send' && <SendResult data={result.data} />}
      {result.kind === 'preview' && <PreviewResult data={result.data} />}
    </section>
  )
}
