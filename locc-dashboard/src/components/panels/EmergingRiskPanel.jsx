import { getOperationalStatusStyle } from '../../styles/operationalStatus'

const getRiskStyle = (risk) => {
  if ((risk.likelihood || '').toLowerCase() === 'high') {
    return getOperationalStatusStyle('under pressure')
  }

  if ((risk.likelihood || '').toLowerCase() === 'moderate') {
    return getOperationalStatusStyle('monitor')
  }

  if ((risk.status || '').toLowerCase() === 'stable') {
    return getOperationalStatusStyle('stable')
  }

  return getOperationalStatusStyle('information')
}

function EmergingRisksPanel({ emergingRisks }) {
  if (!Array.isArray(emergingRisks) || emergingRisks.length === 0) {
    return null
  }

  return (
    <section
      style={{
        border: '2px solid #ddd',
        borderRadius: '14px',
        padding: '18px',
        marginBottom: '24px',
        backgroundColor: '#fafafa',
      }}
    >
      <h2 style={{ marginTop: 0 }}>What could affect us next?</h2>

      {emergingRisks.map((risk) => {
        const style = getRiskStyle(risk)

        return (
          <div
            key={`${risk.category}-${risk.title}`}
            style={{
              border: `1px solid ${style.border}`,
              borderLeft: `6px solid ${style.border}`,
              borderRadius: '10px',
              padding: '12px',
              marginBottom: '10px',
              backgroundColor: style.background,
            }}
          >
            <div
              style={{
                display: 'flex',
                justifyContent: 'space-between',
                gap: '12px',
                alignItems: 'flex-start',
              }}
            >
              <div>
                <strong>{risk.title}</strong>
                <div style={{ fontSize: '13px', marginTop: '4px' }}>
                  {risk.category} · {risk.timeHorizon}
                </div>
              </div>

              <span>{style.label}</span>
            </div>

            <p style={{ marginBottom: '8px' }}>
              {risk.operationalInterpretation}
            </p>

            <p style={{ marginBottom: '8px' }}>
              <strong>Why this matters:</strong> {risk.evidence}
            </p>

            <p style={{ marginBottom: 0 }}>
              <strong>Suggested preparation:</strong> {risk.suggestedPreparation}
            </p>

            <div
              style={{
                marginTop: '10px',
                fontSize: '13px',
                opacity: 0.8,
              }}
            >
              Likelihood: {risk.likelihood} · Confidence: {risk.confidence}
            </div>
          </div>
        )
      })}
    </section>
  )
}

export default EmergingRisksPanel