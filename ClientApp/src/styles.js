// No width here, so callers can size the field. inputCls is the usual full-width version.
export const fieldCls =
  'min-w-0 rounded-lg border border-slate-300 bg-white px-3 py-2 text-base focus:border-indigo-500 focus:outline-none focus:ring-2 focus:ring-indigo-200'
export const inputCls = fieldCls + ' w-full'
export const btnCls =
  'rounded-lg bg-indigo-600 px-3 py-2 text-sm font-medium text-white transition hover:bg-indigo-700 disabled:opacity-50'
export const ghostCls =
  'rounded-lg border border-slate-300 px-3 py-2 text-sm font-medium text-slate-700 transition hover:bg-slate-50 disabled:opacity-50'
export const smallBtnCls =
  'rounded-md border border-slate-300 px-2 py-1 text-xs font-medium text-slate-700 transition hover:bg-slate-50 disabled:opacity-50'
export const cardCls = 'rounded-xl bg-white p-4 shadow-sm ring-1 ring-slate-200'

// One colour per channel, used everywhere a channel is shown.
export const CHANNEL_TONES = {
  Email: 'bg-sky-100 text-sky-800 ring-sky-200',
  Sms: 'bg-amber-100 text-amber-800 ring-amber-200',
  Push: 'bg-violet-100 text-violet-800 ring-violet-200',
  WhatsApp: 'bg-emerald-100 text-emerald-800 ring-emerald-200',
}

// One colour per role in the pattern.
export const ROLE_TONES = {
  Creator: 'bg-indigo-600 text-white',
  'Concrete Creator': 'bg-indigo-100 text-indigo-800',
  Product: 'bg-slate-200 text-slate-700',
}

export const formatCost = (n) => (n === 0 ? 'free' : `${n.toFixed(2)} EGP`)
export const plural = (n, noun) => `${n} ${noun}${n === 1 ? '' : 's'}`
