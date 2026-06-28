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

const EnvironmentalZonesPanel = ({ rooms = [], updateZone }) => {
  const handleRoomUpdate = async (roomId, updates) => {
    try {
      const response = await fetch(`http://localhost:5000/api/zones/${roomId}`, {
        method: 'PATCH',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(updates),
      })

      if (!response.ok) {
        throw new Error(`Room update failed: ${response.status}`)
      }

      const updatedRoom = await response.json()

      updateZone(updatedRoom.facilityRoomId, updatedRoom)
    } catch (err) {
      console.error('Environmental zone update failed:', err)
    }
  }

  const affectedRooms = rooms.filter(
  (room) => room.hasConfirmedCase || room.hasSuspectedCase
).length

const isolationRooms = rooms.filter(
  (room) => room.isIsolationRoom
).length

const closedRooms = rooms.filter(
  (room) => room.isClosed
).length

const highRiskRooms = rooms.filter((room) =>
  ['high', 'critical'].includes(
    (room.riskLevel || '').toLowerCase()
  )
).length

  return (
    <section style={panelStyle}>
      <h2 style={{ marginTop: 0, color: typography.headingColor }}>
        Environmental Risk Zones
      </h2>

      <div
        style={{
          display: 'grid',
          gridTemplateColumns: 'repeat(auto-fit, minmax(140px, 1fr))',
          gap: '12px',
          marginBottom: '20px',
        }}
      >
        <EnvironmentMetricCard
          title="Affected Rooms"
          value={affectedRooms}
          tone="critical"
        />

        <EnvironmentMetricCard
          title="Isolation Rooms"
          value={isolationRooms}
          tone="high"
        />

        <EnvironmentMetricCard
          title="Closed Rooms"
          value={closedRooms}
          tone="moderate"
        />

        <EnvironmentMetricCard
          title="High Risk"
          value={highRiskRooms}
          tone="low"
        />
      </div>

      {rooms.length === 0 && <p>No environmental zone data available.</p>}

      <div
        style={{
          display: 'grid',
          gridTemplateColumns: 'repeat(auto-fit, minmax(240px, 1fr))',
          gap: '12px',
        }}
      >
        {rooms.map((room) => {
          const style = getZoneStyle(room.riskLevel)

          return (
            <div
              key={room.facilityRoomId}
              style={{
                backgroundColor: '#ffffff',
                border: `2px solid ${style.border}`,
                borderLeft: `8px solid ${style.border}`,
                borderRadius: '14px',
                padding: '14px',
              }}
            >
              <div
                style={{
                  fontSize: '18px',
                  fontWeight: 'bold',
                  color: typography.headingColor,
                }}
              >
                {room.roomName}
              </div>

              <div style={{ fontSize: '12px', opacity: 0.75, marginTop: '4px' }}>
                {room.zone || 'Unassigned Zone'}
              </div>

              <div style={{ marginTop: '10px', fontWeight: 'bold', color: style.text }}>
                {room.riskLevel || 'Unknown'} Risk
              </div>

              {room.cohortStatus && (
                <div
                  style={{
                    marginTop: '8px',
                    fontWeight: 'bold',
                    color: '#2E4057',
                  }}
                >
                  Cohort: {room.cohortStatus}
                </div>
              )}

              {room.cohortRationale && (
                <div
                  style={{
                    marginTop: '4px',
                    fontSize: '12px',
                    opacity: 0.8,
                  }}
                >
                  {room.cohortRationale}
                </div>
              )}

              <div style={{ marginTop: '8px', fontSize: '13px' }}>
                {room.hasConfirmedCase && <div>🔴 Confirmed case linked</div>}
                {room.hasSuspectedCase && <div>🟠 Suspected case linked</div>}
                {room.isIsolationRoom && <div>🛏 Isolation active</div>}
                {room.isClosed && <div>⛔ Room closed</div>}
              </div>

              <div
                style={{
                  display: 'flex',
                  flexWrap: 'wrap',
                  gap: '8px',
                  marginTop: '14px',
                }}
              >
                <button
                  onClick={() =>
                    handleRoomUpdate(room.facilityRoomId, {
                      riskLevel: 'High',
                    })
                  }
                >
                  Escalate
                </button>

                <button
                  onClick={() =>
                    handleRoomUpdate(room.facilityRoomId, {
                      riskLevel: 'Low',
                    })
                  }
                >
                  De-escalate
                </button>

                <button
                  onClick={() =>
                    handleRoomUpdate(room.facilityRoomId, {
                      cohortStatus: 'Recovery',
                      cohortRationale:
                        'Resident is post-infection and requires monitoring for secondary infection risk.',
                    })
                  }
                >
                  Recovery Cohort
                </button>

                <button
                  onClick={() =>
                    handleRoomUpdate(room.facilityRoomId, {
                      isClosed: !room.isClosed,
                    })
                  }
                >
                  {room.isClosed ? 'Open Room' : 'Close Room'}
                </button>

                <button
                  onClick={() =>
                    handleRoomUpdate(room.facilityRoomId, {
                      isIsolationRoom: !room.isIsolationRoom,
                    })
                  }
                >
                  {room.isIsolationRoom ? 'Remove Isolation' : 'Assign Isolation'}
                </button>

                <button
                  onClick={() =>
                    handleRoomUpdate(room.facilityRoomId, {
                      cohortStatus: 'Green',
                      cohortRationale: 'Room assigned to green cohort / standard precautions.',
                    })
                  }
                >
                  Green Cohort
                </button>

                <button
                  onClick={() =>
                    handleRoomUpdate(room.facilityRoomId, {
                      cohortStatus: 'Amber',
                      cohortRationale: 'Room assigned to amber cohort / exposure or suspected risk.',
                    })
                  }
                >
                  Amber Cohort
                </button>

                <button
                  onClick={() =>
                    handleRoomUpdate(room.facilityRoomId, {
                      cohortStatus: 'Red',
                      cohortRationale: 'Room assigned to red cohort / confirmed outbreak risk.',
                    })
                  }
                >
                  Red Cohort
                </button>

                <button
                  onClick={() =>
                    handleRoomUpdate(room.facilityRoomId, {
                      cohortStatus: 'Isolation',
                      cohortRationale: 'Room assigned for individual isolation precautions.',
                      isIsolationRoom: true,
                    })
                  }
                >
                  Isolation Cohort
                </button>

                <button
                  onClick={() =>
                    handleRoomUpdate(room.facilityRoomId, {
                      terminalCleanCompleted: true,
                    })
                  }
                >
                  Terminal Clean Complete
                </button>
              </div>

              {room.notes && (
                <div
                  style={{
                    marginTop: '10px',
                    padding: '8px',
                    backgroundColor: '#F7F5F2',
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

const EnvironmentMetricCard = ({ title, value, tone }) => {
  const toneStyles = {
    critical: {
      background: '#ffe5e5',
      border: '#ff4d4d',
      text: '#8a1f1f',
    },
    high: {
      background: '#fff3e0',
      border: '#ff9800',
      text: '#8a4b00',
    },
    moderate: {
      background: '#fffde7',
      border: '#fbc02d',
      text: '#6b5b00',
    },
    low: {
      background: '#e8f5e9',
      border: '#4caf50',
      text: '#1b5e20',
    },
  }

  const style = toneStyles[tone]

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
          color: style.text,
          marginBottom: '6px',
        }}
      >
        {title}
      </div>

      <div
        style={{
          fontSize: '28px',
          fontWeight: 'bold',
          color: style.text,
        }}
      >
        {value}
      </div>
    </div>
  )
}

export default EnvironmentalZonesPanel