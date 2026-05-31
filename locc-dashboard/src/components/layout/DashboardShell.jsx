import { colors, spacing, radius, typography } from '../../styles/theme'

const DashboardShell = ({ children }) => {
  return (
    <div
      style={{
        display: 'grid',
        gridTemplateColumns: '220px 1fr',
        minHeight: '100vh',
        backgroundColor: colors.offWhite,
        color: colors.navy,
        fontFamily: typography.fontFamily,
      }}
    >
      <aside
        style={{
          backgroundColor: '#2E4057',
          color: '#F7F5F2',
          padding: '24px 16px',
          position: 'sticky',
          top: 0,
          height: '100vh',
        }}
      >
        <h2 style={{ marginTop: 0 }}>LOCC</h2>
        <p style={{ opacity: 0.8, fontSize: '13px' }}>IPC Command Centre</p>

        <nav style={{ display: 'grid', gap: '12px', marginTop: '32px' }}>
          <a style={navStyle}>🏠 Home</a>
          <a style={navStyle}>🦠 Outbreak Command</a>
          <a style={navStyle}>📋 Line Listing</a>
          <a style={navStyle}>🧪 PPE Monitoring</a>
          <a style={navStyle}>🗺️ Risk Zones</a>
          <a style={navStyle}>📤 Reporting</a>
        </nav>
      </aside>

      <main style={{ padding: '24px', overflowX: 'hidden' }}>
        {children}
      </main>
    </div>
  )
}

const navStyle = {
  color: '#F7F5F2',
  textDecoration: 'none',
  padding: '10px 12px',
  borderRadius: '10px',
  backgroundColor: 'rgba(255,255,255,0.08)',
  cursor: 'pointer',
}

export default DashboardShell