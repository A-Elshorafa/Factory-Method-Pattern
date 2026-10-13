import { CHANNEL_TONES } from '../styles.js'

export default function ChannelChip({ channel }) {
  return <span className={`rounded-full px-2 py-0.5 text-xs font-medium ring-1 ${CHANNEL_TONES[channel]}`}>{channel}</span>
}
