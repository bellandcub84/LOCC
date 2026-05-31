import { panelStyle, typography } from '../../styles/theme'

const SurveillancePanel = ({
  surveillanceCases = [],
  surveillanceSearch = '',
  setSurveillanceSearch,
}) => {
  const search = surveillanceSearch.toLowerCase()

  const filteredCases = surveillanceCases.filter((c) => (
    c.displayName?.toLowerCase().includes(search) ||
    c.personType?.toLowerCase().includes(search) ||
    c.roomName?.toLowerCase().includes(search) ||
    c.caseStatus?.toLowerCase().includes(search)
  ))

  const confirmedCount = surveillanceCases.filter(
    (c) => c.caseStatus === 'Confirmed'
  ).length

  const suspectedCount = surveillanceCases.filter(
    (c) => c.caseStatus === 'Suspected'
  ).length

  const residentCount = surveillanceCases.filter(
    (c) => c.personType === 'Resident'
  ).length

  const staffCount = surveillanceCases.filter(
    (c) => c.personType === 'Staff'
  ).length

  const notificationOutstandingCount = surveillanceCases.filter(
    (c) =>
      !c.publicHealthNotificationStatus ||
      c.publicHealthNotificationStatus === 'Pending'
  ).length

  return (
    <section style={panelStyle}>
      <h2 style={{ marginTop: 0, color: typography.headingColor }}>
        Surveillance & Line Listing
      </h2>

      <div
        style={{
          display: 'grid',
          gridTemplateColumns: 'repeat(auto-fit, minmax(140px, 1fr))',
          gap: '12px',
          marginBottom: '16px',
        }}
      >
        <SurveillanceMetricCard title="Confirmed" value={confirmedCount} tone="critical" />
        <SurveillanceMetricCard title="Suspected" value={suspectedCount} tone="moderate" />
        <SurveillanceMetricCard title="Residents" value={residentCount} tone="info" />
        <SurveillanceMetricCard title="Staff" value={staffCount} tone="info" />
        <SurveillanceMetricCard
          title="Notifications Outstanding"
          value={notificationOutstandingCount}
          tone={notificationOutstandingCount > 0 ? 'high' : 'low'}
        />
      </div>

      <input
        type="text"
        placeholder="Search resident, staff, room, or case status..."
        value={surveillanceSearch}
        onChange={(e) => setSurveillanceSearch(e.target.value)}
        style={{
          width: '100%',
          padding: '12px',
          marginBottom: '16px',
          borderRadius: '8px',
          border: '1px solid #ddd',
        }}
      />

      <table style={{ width: '100%', borderCollapse: 'collapse' }}>
        <thead>
          <tr>
            <th>Name</th>
            <th>Type</th>
            <th>Room</th>
            <th>Status</th>
            <th>Test</th>
            <th>Onset</th>
            <th>Notification</th>
          </tr>
        </thead>

        <tbody>
          {filteredCases.map((c) => (
            <tr key={c.surveillanceCaseId}>
              <td>{c.displayName || '-'}</td>
              <td>{c.personType || '-'}</td>
              <td>{c.roomName || '-'}</td>
              <td>{c.caseStatus || '-'}</td>
              <td>{c.testResult || '-'}</td>
              <td>
                {c.symptomOnsetDate
                  ? new Date(c.symptomOnsetDate).toLocaleDateString()
                  : '-'}
              </td>
              <td>{c.publicHealthNotificationStatus || '-'}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </section>
  )
}

const SurveillanceMetricCard = ({ title, value, tone }) => {
  const toneStyles = {
    critical: { background: '#ffe5e5', border: '#ff4d4d', text: '#8a1f1f' },
    high: { background: '#fff3e0', border: '#ff9800', text: '#8a4b00' },
    moderate: { background: '#fffde7', border: '#fbc02d', text: '#6b5b00' },
    low: { background: '#e8f5e9', border: '#4caf50', text: '#1b5e20' },
    info: { background: '#e3f2fd', border: '#2196f3', text: '#0d47a1' },
  }

  const style = toneStyles[tone] || toneStyles.info

  return (
    <div
      style={{
        backgroundColor: style.background,
        border: `2px solid ${style.border}`,
        borderRadius: '12px',
        padding: '12px',
      }}
    >
      <div
        style={{
          fontSize: '12px',
          textTransform: 'uppercase',
          opacity: 0.8,
          marginBottom: '6px',
          color: style.text,
        }}
      >
        {title}
      </div>

      <div style={{ fontSize: '26px', fontWeight: 'bold', color: style.text }}>
        {value}
      </div>
    </div>
  )
}

export default SurveillancePanel