import { useState } from 'react'
import { btnCls, cardCls, formatCost, ghostCls, inputCls, smallBtnCls } from '../styles.js'
import ChannelChip from './ChannelChip.jsx'

const SAMPLES = {
  Short: {
    title: 'Order #1042 shipped',
    body: 'Your order is on its way and should arrive on Thursday. Track it at nilemart.example/t/1042',
  },
  Long: {
    title: 'Order #1042 shipped',
    body: 'Good news! Your order of 3 items has left our Cairo warehouse and is on its way with Nile Express. It should arrive on Thursday between 10:00 and 14:00. Track it any time at nilemart.example/t/1042 or reply HELP for support.',
  },
  Arabic: {
    title: 'تم شحن طلبك رقم 1042',
    body: 'طلبك في الطريق ومن المتوقع أن يصل يوم الخميس بين الساعة 10 و 14. تابع الشحنة على nilemart.example/t/1042',
  },
}

function ChannelPicker({ channels, selected, onSelect }) {
  return (
    <div className="grid grid-cols-2 gap-2">
      {channels.map((c) => (
        <button key={c.channel} type="button" onClick={() => onSelect(c)}
          className={`rounded-lg p-2 text-left ring-1 transition ${selected === c.channel
            ? 'bg-indigo-50 ring-2 ring-indigo-500'
            : 'bg-slate-50 ring-slate-200 hover:bg-slate-100'}`}>
          <div className="flex items-center justify-between gap-1">
            <ChannelChip channel={c.channel} />
            <span className="text-xs tabular-nums text-slate-500">{formatCost(c.costPerPart)}</span>
          </div>
          <div className="mt-1 truncate font-mono text-[11px] text-slate-500">{c.creator}</div>
        </button>
      ))}
    </div>
  )
}

export default function SendForm({ channels, onSend, onPreview, busy }) {
  const [channel, setChannel] = useState('Sms')
  const [recipient, setRecipient] = useState('+201012345678')
  const [title, setTitle] = useState(SAMPLES.Short.title)
  const [body, setBody] = useState(SAMPLES.Short.body)

  const current = channels.find((c) => c.channel === channel)
  const examples = channels.map((c) => c.exampleRecipient)

  // Switch the recipient to the new channel's example, unless the user typed their own.
  const select = (c) => {
    setChannel(c.channel)
    if (!recipient || examples.includes(recipient)) setRecipient(c.exampleRecipient)
  }

  const submit = (e) => {
    e.preventDefault()
    onSend({ channel, recipient, title, body })
  }

  return (
    <form className={cardCls} onSubmit={submit}>
      <h2 className="mb-3 font-semibold text-slate-800">Send a notification</h2>

      <span className="text-sm font-medium text-slate-700">Channel</span>
      <div className="mt-1">
        <ChannelPicker channels={channels} selected={channel} onSelect={select} />
      </div>

      {current && (
        <>
          <code className="mt-2 block whitespace-pre-wrap break-words rounded-lg bg-slate-900 px-3 py-2 text-xs text-slate-100">
            <span className="text-slate-400">NotificationDispatcher d = </span>new {current.creator}();<br />
            <span className="text-slate-400">// d.DispatchAsync() calls </span>CreateChannel()<span className="text-slate-400"> → </span>new {current.product}()
          </code>
          <p className="mt-2 text-xs text-slate-500">{current.description}</p>
        </>
      )}

      <label className="mt-3 block text-sm font-medium text-slate-700">
        Recipient
        <input className={inputCls + ' mt-1 font-normal'} value={recipient} onChange={(e) => setRecipient(e.target.value)} required />
      </label>
      {current && (
        <p className="mt-1 text-xs text-slate-500">
          {current.recipientFormat}.{' '}
          {recipient !== current.exampleRecipient && (
            <button type="button" className="font-medium text-indigo-600 hover:text-indigo-800" onClick={() => setRecipient(current.exampleRecipient)}>
              Use {current.exampleRecipient}
            </button>
          )}
        </p>
      )}

      <div className="mt-4 flex flex-wrap items-center justify-between gap-2">
        <span className="text-sm font-medium text-slate-700">Message</span>
        <div className="flex flex-wrap gap-1">
          {Object.entries(SAMPLES).map(([name, sample]) => (
            <button key={name} type="button" className={smallBtnCls} onClick={() => { setTitle(sample.title); setBody(sample.body) }}>{name}</button>
          ))}
        </div>
      </div>

      <label className="mt-2 block text-sm font-medium text-slate-700">
        Title <span className="font-normal text-slate-400">({title.length} chars)</span>
        <input dir="auto" className={inputCls + ' mt-1 font-normal'} value={title} onChange={(e) => setTitle(e.target.value)} required />
      </label>

      <label className="mt-3 block text-sm font-medium text-slate-700">
        Body <span className="font-normal text-slate-400">({body.length} chars)</span>
        <textarea dir="auto" className={inputCls + ' mt-1 h-28 font-normal'} value={body} onChange={(e) => setBody(e.target.value)} required />
      </label>

      <div className="mt-4 flex flex-col gap-2 sm:flex-row">
        <button className={btnCls + ' flex-1'} disabled={busy || !current}>Send via {channel}</button>
        <button type="button" className={ghostCls + ' flex-1'} disabled={busy || !title || !body} onClick={() => onPreview({ title, body, channels: [] })}>
          Preview on all channels
        </button>
      </div>
      <p className="mt-2 text-xs text-slate-500">
        “Preview” asks every dispatcher to render this message with its own channel, without sending or billing anything.
      </p>
    </form>
  )
}
