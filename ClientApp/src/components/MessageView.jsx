// Shows the parts the way each channel would display them. parts: [{ content, meta }]

const textCls = 'whitespace-pre-wrap break-words'

function Email({ content }) {
  const split = content.indexOf('\n\n')
  const headers = split < 0 ? [] : content.slice(0, split).split('\n')
  const body = split < 0 ? content : content.slice(split + 2)
  return (
    <div className="overflow-hidden rounded-lg ring-1 ring-slate-200">
      <div className="space-y-0.5 bg-slate-50 px-3 py-2 text-xs text-slate-600">
        {headers.map((h, i) => {
          const at = h.indexOf(': ')
          return <div key={i} dir="auto"><span className="font-medium text-slate-800">{h.slice(0, at)}:</span> {h.slice(at + 2)}</div>
        })}
      </div>
      <div dir="auto" className={`${textCls} bg-white px-3 py-2 text-sm text-slate-800`}>{body}</div>
    </div>
  )
}

function Push({ content }) {
  const [title, ...rest] = content.split('\n')
  return (
    <div className="flex gap-3 rounded-2xl bg-slate-100 p-3 shadow-sm ring-1 ring-slate-200">
      <span className="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg bg-indigo-600 text-sm font-bold text-white">N</span>
      <div className="min-w-0 flex-1">
        <div className="text-[11px] text-slate-500">NileMart · now</div>
        <div dir="auto" className="break-words text-sm font-semibold text-slate-900">{title}</div>
        <div dir="auto" className={`${textCls} text-sm text-slate-700`}>{rest.join('\n')}</div>
      </div>
    </div>
  )
}

// WhatsApp shows *text* in bold.
function WhatsAppText({ text }) {
  return text.split(/(\*[^*\n]+\*)/).map((piece, i) =>
    /^\*[^*\n]+\*$/.test(piece) ? <strong key={i}>{piece.slice(1, -1)}</strong> : piece)
}

const bubble = {
  Sms: 'rounded-2xl rounded-bl-sm bg-slate-200 text-slate-900',
  WhatsApp: 'rounded-xl rounded-tr-sm bg-emerald-100 text-slate-900',
}

export default function MessageView({ channel, parts }) {
  return (
    <div className="space-y-2">
      {parts.map((p, i) => (
        <div key={i}>
          {channel === 'Email' && <Email content={p.content} />}
          {channel === 'Push' && <Push content={p.content} />}
          {bubble[channel] && (
            <div className={channel === 'WhatsApp' ? 'flex justify-end' : ''}>
              <div dir="auto" className={`${textCls} max-w-[90%] px-3 py-2 text-sm ${bubble[channel]}`}>
                {channel === 'WhatsApp' ? <WhatsAppText text={p.content} /> : p.content}
              </div>
            </div>
          )}
          <div className={`mt-0.5 text-[11px] tabular-nums text-slate-400 ${channel === 'WhatsApp' ? 'text-right' : ''}`}>
            {parts.length > 1 && `${i + 1}/${parts.length} · `}{p.content.length} chars{p.meta && ` · ${p.meta}`}
          </div>
        </div>
      ))}
    </div>
  )
}
