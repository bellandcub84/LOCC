const getSituationStyle = (severity) => {
  switch ((severity || '').toLowerCase()) {
    case 'critical':
      return {
        label: '🔴 Critical',
        background: '#ffe5e5',
        border: '#ff4d4d',
      }
    case 'actionrequired':
    case 'action required':
      return {
        label: '🟠 Action required',
        background: '#fff3e0',
        border: '#ff9800',
      }
    case 'monitor':
      return {
        label: '🟡 Monitor',
        background: '#fffde7',
        border: '#fbc02d',
      }
    case 'information':
      return {
        label: '🔵 Information',
        background: '#e3f2fd',
        border: '#64b5f6',
      }
    default:
      return {
        label: severity || 'Update',
        background: '#f5f5f5',
        border: '#ccc',
      }
  }
}

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
        const style = getSituationStyle(item.severity)

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