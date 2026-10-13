import { useEffect, useState } from 'react'
import { clearOutbox, getChannels, getNotification, listOutbox, previewNotification, sendNotification } from './api.js'
import Outbox from './components/Outbox.jsx'
import ResultPanel from './components/ResultPanel.jsx'
import SendForm from './components/SendForm.jsx'

export default function App() {
  const [channels, setChannels] = useState([])
  const [outbox, setOutbox] = useState([])
  const [result, setResult] = useState(null)
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState('')

  const refresh = async () => setOutbox(await listOutbox())

  // Runs one action, shows its error if any, then refreshes the outbox.
  const run = async (action) => {
    setBusy(true)
    setError('')
    try {
      await action()
    } catch (err) {
      setError(err.message)
    } finally {
      await refresh().catch(() => {})
      setBusy(false)
    }
  }

  useEffect(() => {
    run(async () => setChannels(await getChannels()))
  }, [])

  const actions = {
    send: (notification) => run(async () => setResult({ kind: 'send', data: await sendNotification(notification) })),
    preview: (message) => run(async () => setResult({ kind: 'preview', data: await previewNotification(message) })),
    view: (id) => run(async () => setResult({ kind: 'send', data: await getNotification(id) })),
    clearOutbox: () => run(clearOutbox),
  }

  return (
    <div className="mx-auto max-w-6xl space-y-6 px-4 py-6">
      <header>
        <h1 className="text-2xl font-bold text-slate-900">Notification Factory Method</h1>
        <p className="text-sm text-slate-600">
          Every channel goes through the same steps, written once in <code className="text-xs">NotificationDispatcher</code>: check the
          recipient, render, deliver each part, add up the cost. Only the channel object differs, and each dispatcher subclass creates its
          own in <code className="text-xs">CreateChannel()</code>, the factory method.
        </p>
      </header>

      {error && (
        <div className="flex items-start justify-between gap-2 rounded-lg bg-rose-50 p-3 text-sm text-rose-800 ring-1 ring-rose-200">
          <span>{error}</span>
          <button className="font-medium" onClick={() => setError('')}>Dismiss</button>
        </div>
      )}

      <div className="grid gap-6 lg:grid-cols-[minmax(0,2fr)_minmax(0,3fr)]">
        <SendForm channels={channels} onSend={actions.send} onPreview={actions.preview} busy={busy} />
        <ResultPanel result={result} />
      </div>

      <Outbox entries={outbox} onView={actions.view} onClear={actions.clearOutbox} busy={busy} />
    </div>
  )
}
