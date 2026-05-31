import {
  panelStyle,
  typography,
  statusColors,
} from '../../styles/theme'

const getZoneStyle = (riskLevel) => {
  switch ((riskLevel || '').toLowerCase()) {
    case 'critical':
      return statusColors.critical
    case 'high':
      return statusColors.high
    case 'moderate':
      return statusColors.moderate
    case 'low':
      return statusColors.low
    default:
      return statusColors.neutral
  }
}

const EnvironmentalZonesPanel = ({ rooms }) => {
  return (
    <section style={panelStyle}>
      <h2
        style={{
          marginTop: 0,
          color: typography.headingColor,
        }}
      >
        Environmental Risk Zones
      </h2>

      {rooms.length === 0 && <p>No environmental zone data available.</p>}

      <div
        style={{
          display: 'grid',
          gridTemplateColumns: 'repeat(auto-fit, minmax(220px, 1fr))',
          gap: '12px',
        }}
      >
        {rooms.map((room) => {
          const style = getZoneStyle(room.riskLevel)

          return (
            <div
              key={room.facilityRoomId}
              style={{
                backgroundColor: style.background,
                border: `2px solid ${style.border}`,
                borderRadius: '14px',
                padding: '14px',
              }}
            >
              <div
                style={{
                  fontSize: '18px',
                  fontWeight: 'bold',
                  color: style.text,
                }}
              >
                {room.roomName}
              </div>

              <div
                style={{
                  fontSize: '12px',
                  opacity: 0.8,
                  marginTop: '4px',
                }}
              >
                {room.zone || 'Unassigned Zone'}
              </div>

              <div
                style={{
                  marginTop: '12px',
                  fontWeight: 'bold',
                  color: style.text,
                }}
              >
                {room.riskLevel || 'Unknown'} Risk
              </div>

              <div style={{ marginTop: '10px', fontSize: '13px' }}>
                {room.hasConfirmedCase && <div>🔴 Confirmed case linked</div>}
                {room.hasSuspectedCase && <div>🟠 Suspected case linked</div>}
                {room.isIsolationRoom && <div>🛏 Isolation-capable</div>}
                {room.isClosed && <div>⛔ Room closed</div>}
              </div>

              {room.notes && (
                <div
                  style={{
                    marginTop: '10px',
                    padding: '8px',
                    backgroundColor: 'rgba(255,255,255,0.65)',
                    borderRadius: '8px',
                    fontSize: '12px',
                  }}
                >
                  {room.notes}
                </div>
              )}
            </div>
          )
        })}
      </div>
    </section>
  )
}

export default EnvironmentalZonesPanel