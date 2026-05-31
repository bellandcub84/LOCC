import {
  panelStyle,
  metricCardStyle,
  typography,
} from '../../styles/theme'

const OperationalEpidemiologyPanel = ({ epidemiology }) => {
  if (!epidemiology) {
    return null
  }

  return (
    <section style={panelStyle}>
      <h2
        style={{
          marginTop: 0,
          color: typography.headingColor,
        }}
      >
        Operational Epidemiology
      </h2>

      <div
        style={{
          display: 'grid',
          gridTemplateColumns: 'repeat(auto-fit, minmax(140px, 1fr))',
          gap: '12px',
          marginBottom: '20px',
        }}
      >
        <MetricCard title="Total Cases" value={epidemiology.totalCases} />
        <MetricCard title="Confirmed" value={epidemiology.confirmedCases} />
        <MetricCard title="Suspected" value={epidemiology.suspectedCases} />
        <MetricCard title="Residents" value={epidemiology.residentCases} />
        <MetricCard title="Staff" value={epidemiology.staffCases} />
        <MetricCard title="Hospitalisations" value={epidemiology.hospitalisations} />
        <MetricCard title="Deaths" value={epidemiology.deaths} />
      </div>

      <h3>Epi Curve</h3>

      {(!epidemiology.casesByDate ||
        epidemiology.casesByDate.length === 0) && (
        <p>No symptom onset data available.</p>
      )}

      {(epidemiology.casesByDate || []).map((day) => (
        <div key={day.date} style={{ marginBottom: '10px' }}>
          <strong>
            {new Date(day.date).toLocaleDateString()}
          </strong>

          <div
            style={{
              height: '22px',
              width: `${Math.max(day.count * 40, 40)}px`,
              backgroundColor: colors.sage,
              borderRadius: '6px',
              color: 'white',
              paddingLeft: '8px',
              marginTop: '4px',
              display: 'flex',
              alignItems: 'center',
            }}
          >
            {day.count}
          </div>
        </div>
      ))}

      <h3 style={{ marginTop: '24px' }}>Cases by Zone</h3>

      {(epidemiology.casesByZone || []).map((zone) => (
        <div
          key={zone.zone}
          style={{
            padding: '10px',
            borderBottom: '1px solid #eee',
          }}
        >
          <strong>{zone.zone}</strong>: {zone.count} case(s)

          <div
            style={{
              fontSize: '12px',
              opacity: 0.7,
              marginTop: '4px',
            }}
          >
            Confirmed: {zone.confirmed} | Suspected: {zone.suspected}
          </div>
        </div>
      ))}
    </section>
  )
}

const MetricCard = ({ title, value }) => {
  return (
    <div
      style={{
        ...metricCardStyle,
      }}
    >
      <div
        style={{
          fontSize: '12px',
          opacity: 0.7,
          marginBottom: '6px',
          textTransform: 'uppercase',
        }}
      >
        {title}
      </div>

      <div
        style={{
          fontSize: '24px',
          fontWeight: 'bold',
          color: typography.headingColor,
        }}
      >
        {value}
      </div>
    </div>
  )
}

export default OperationalEpidemiologyPanel