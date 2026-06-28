import { getOperationalStatusStyle } from '../../styles/operationalStatus'

function SituationAwarenessPanel({ situationAwareness }) {
  if (!Array.isArray(situationAwareness) || situationAwareness.length === 0) {
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
      <h2 style={{ marginTop: 0 }}>What has changed?</h2>

      {situationAwareness.map((item) => {
        const style = getOperationalStatusStyle(item.severity)

        return (
          <div
            key={item.situationAwarenessItemId}
            style={{
              border: `1px solid ${style.border}`,
              borderLeft: `6px solid ${style.border}`,
              borderRadius: '10px',
              padding: '12px',
              marginBottom: '10px',
              backgroundColor: style.background,
            }}
          >
            <div style={{ display: 'flex', justifyContent: 'space-between', gap: '12px' }}>
              <strong>{item.title}</strong>
              <span>{style.label}</span>
            </div>

            <p style={{ marginBottom: '8px' }}>
              {item.operationalInterpretation || item.summary}
            </p>

            {item.recommendedAction && (
              <p style={{ marginBottom: 0 }}>
                <strong>Suggested action:</strong> {item.recommendedAction}
              </p>
            )}
          </div>
        )
      })}
    </section>
  )
}

export default SituationAwarenessPanel