import { colors, panelStyle, metricCardStyle, typography } from '../../styles/theme'

const TopCommandBar = ({ summary }) => {
  return (
    <section style={panelStyle}>

      <div
        style={{
        ...metricCardStyle,
        borderLeft: `6px solid ${colors.sage}`,
        color: typography.headingColor,
        }}
      >

        <CommandCard
          title="Pathogen"
          value={summary?.pathogen || 'Unknown'}
        />

        <CommandCard
          title="Outbreak Phase"
          value={summary?.outbreakPhase || 'Unknown'}
        />

        <CommandCard
          title="Active Cases"
          value={summary?.activeCases ?? 0}
        />

        <CommandCard
          title="PPE Alerts"
          value={summary?.ppeWarnings ?? 0}
        />
      </div>
    </section>
  )
}

const CommandCard = ({ title, value }) => {
  return (
    <div
      style={{
        backgroundColor: '#F7F5F2',
        borderRadius: '12px',
        padding: '16px',
        borderLeft: '6px solid #7C9A92',
      }}
    >
      <div
        style={{
          fontSize: '12px',
          opacity: 0.7,
          marginBottom: '6px',
          textTransform: 'uppercase',
          letterSpacing: '0.5px',
        }}
      >
        {title}
      </div>

      <div
        style={{
          fontSize: '24px',
          fontWeight: 'bold',
          color: '#2E4057',
        }}
      >
        {value}
      </div>
    </div>
  )
}

export default TopCommandBar