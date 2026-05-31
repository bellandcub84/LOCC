import {
  panelStyle,
  typography,
  statusColors,
} from '../../styles/theme'

const SurveillancePanel = ({
  surveillanceCases,
  surveillanceSearch,
  setSurveillanceSearch,
}) => {
  const filteredCases = surveillanceCases.filter((c) => {
    const search = surveillanceSearch.toLowerCase()

    return (
      c.displayName?.toLowerCase().includes(search) ||
      c.personType?.toLowerCase().includes(search) ||
      c.roomName?.toLowerCase().includes(search) ||
      c.caseStatus?.toLowerCase().includes(search)
    )
  })

  return (
    <section style={panelStyle}>
      <h2
        style={{
          marginTop: 0,
          color: typography.headingColor,
        }}
      >
        Surveillance & Line Listing
      </h2>

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

      <table
        style={{
          width: '100%',
          borderCollapse: 'collapse',
        }}
      >
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
              <td>{c.displayName}</td>
              <td>{c.personType}</td>
              <td>{c.roomName}</td>
              <td>{c.caseStatus}</td>
              <td>{c.testResult}</td>
              <td>
                {c.symptomOnsetDate
                  ? new Date(c.symptomOnsetDate).toLocaleDateString()
                  : '-'}
              </td>
              <td>{c.publicHealthNotificationStatus}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </section>
  )
}

export default SurveillancePanel