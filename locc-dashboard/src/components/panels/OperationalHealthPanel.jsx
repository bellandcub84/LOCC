import { getOperationalStatusStyle } from '../../styles/operationalStatus'

function OperationalHealthPanel({ operationalHealth }) {
  if (!operationalHealth) return null

  const style =getOperationalStatusStyle( operationalHealth.overallStatus)

  return (
    <section
      style={{
        border: `2px solid ${style.border}`,
        backgroundColor: style.background,
        borderRadius: '14px',
        padding: '18px',
        marginBottom: '24px',
      }}
    >
      <h2 style={{ marginTop: 0 }}>
        {operationalHealth.question || 'How are we coping?'}
      </h2>

      <div style={{ fontSize: '24px', fontWeight: 'bold', marginBottom: '8px' }}>
        {style.label}
      </div>

      <p>{operationalHealth.operationalInterpretation}</p>

      {Array.isArray(operationalHealth.suggestedFocus) &&
        operationalHealth.suggestedFocus.length > 0 && (
          <div style={{ marginTop: '12px' }}>
            <strong>Today&apos;s focus</strong>
            <ul>
              {operationalHealth.suggestedFocus.map((item) => (
                <li key={item}>{item}</li>
              ))}
            </ul>
          </div>
        )}

      {Array.isArray(operationalHealth.dimensions) &&
        operationalHealth.dimensions.length > 0 && (
          <div
            style={{
              display: 'grid',
              gridTemplateColumns: 'repeat(auto-fit, minmax(180px, 1fr))',
              gap: '12px',
              marginTop: '16px',
            }}
          >
            {operationalHealth.dimensions.map((dimension) => {
              const dimensionStyle = getOperationalStatusStyle(dimension.status)

              return (
                <div
                  key={dimension.name}
                  style={{
                    border: `1px solid ${dimensionStyle.border}`,
                    backgroundColor: '#fff',
                    borderRadius: '10px',
                    padding: '12px',
                  }}
                >
                  <strong>{dimension.name}</strong>
                  <div>{dimensionStyle.label}</div>
                  <small>{dimension.reason}</small>
                </div>
              )
            })}
          </div>
        )}
    </section>
  )
}

export default OperationalHealthPanel