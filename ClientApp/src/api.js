async function request(path, options) {
  const res = await fetch(path, {
    headers: { 'Content-Type': 'application/json' },
    ...options,
  })
  const text = await res.text()
  let body = text
  try { body = text ? JSON.parse(text) : null } catch { /* plain-text error */ }
  if (!res.ok) {
    const message = typeof body === 'string' ? body : body?.detail ?? body?.title ?? `Request failed (${res.status})`
    const error = new Error(message)
    error.status = res.status
    error.body = body
    throw error
  }
  return body
}

const post = (path, body) => request(path, { method: 'POST', body: body ? JSON.stringify(body) : undefined })
const del = (path) => request(path, { method: 'DELETE' })

export const getChannels = () => request('/api/channels')
export const previewNotification = (message) => post('/api/notifications/preview', message)
export const listOutbox = () => request('/api/notifications')
export const getNotification = (id) => request(`/api/notifications/${encodeURIComponent(id)}`)
export const clearOutbox = () => del('/api/notifications')

// 422 means the channel rejected the recipient. The body is still a full result with its steps, so show it.
export const sendNotification = (notification) =>
  post('/api/notifications', notification).catch((err) => {
    if (err.status === 422) return err.body
    throw err
  })
