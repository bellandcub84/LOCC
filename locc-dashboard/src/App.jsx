import { useEffect, useState } from 'react'
import TaskLifecycleUpdate from './components/TaskLifecycleUpdate'

const getPriorityStyle = (priority) => {
  switch (priority) {
    case 'Critical':
      return { label: '🔴 Critical', background: '#ffe5e5', border: '#ff4d4d' }
    case 'High':
      return { label: '🟠 High', background: '#fff3e0', border: '#ff9800' }
    case 'Moderate':
      return { label: '🟡 Moderate', background: '#fffde7', border: '#fbc02d' }
    default:
      return { label: '🔵 Low', background: '#e3f2fd', border: '#64b5f6' }
  }
}

const groupTasksByOperationalArea = (tasks) => {
  return tasks.reduce((groups, task) => {
    const area = task.operationalArea || 'Unassigned'

    if (!groups[area]) {
      groups[area] = []
    }

    groups[area].push(task)
    return groups
  }, {})
}

const getPriorityCounts = (tasks) => {
  return tasks.reduce(
    (counts, task) => {
      const priority = task.priority || 'Low'
      counts[priority] = (counts[priority] || 0) + 1
      return counts
    },
    {
      Critical: 0,
      High: 0,
      Moderate: 0,
      Low: 0,
    }
  )
}

const getRiskStyle = (riskLevel) => {
  switch ((riskLevel || '').toLowerCase()) {
    case 'high':
    case 'severe':
      return { label: '🔴 High', background: '#ffe5e5', border: '#ff4d4d' }
    case 'moderate':
      return { label: '🟠 Moderate', background: '#fff3e0', border: '#ff9800' }
    case 'low':
      return { label: '🟢 Low', background: '#e8f5e9', border: '#4caf50' }
    default:
      return { label: riskLevel || 'Unknown', background: '#f5f5f5', border: '#ccc' }
  }
}

const getBauStyle = (score) => {
  if (score >= 80) {
    return { label: '🟢 Ready', background: '#e8f5e9', border: '#4caf50' }
  }

  if (score >= 60) {
    return { label: '🟡 Caution', background: '#fffde7', border: '#fbc02d' }
  }

  return { label: '🔴 Not ready', background: '#ffe5e5', border: '#ff4d4d' }
}

const getPpeWarningStyle = (warnings) => {
  if (warnings >= 2) {
    return { label: '🔴 Action required', background: '#ffe5e5', border: '#ff4d4d' }
  }

  if (warnings === 1) {
    return { label: '🟠 Monitor', background: '#fff3e0', border: '#ff9800' }
  }

  return { label: '🟢 Stable', background: '#e8f5e9', border: '#4caf50' }
}

const getStatusStyle = (status) => {
  switch (status) {
    case 'Completed':
      return { label: '🟢 Completed', background: '#e8f5e9', border: '#4caf50' }
    case 'In Progress':
      return { label: '🔵 In Progress', background: '#e3f2fd', border: '#2196f3' }
    case 'Escalated':
      return { label: '🔴 Escalated', background: '#ffe5e5', border: '#ff4d4d' }
    case 'Blocked':
      return { label: '🟠 Blocked', background: '#fff3e0', border: '#ff9800' }
    case 'Cancelled':
      return { label: '⚪ Cancelled', background: '#f5f5f5', border: '#999' }
    default:
      return { label: '🟡 Pending', background: '#fffde7', border: '#fbc02d' }
  }
}

const getStatusCounts = (tasks) => {
  return tasks.reduce(
    (counts, task) => {
      const status = task.status || 'Pending'
      counts[status] = (counts[status] || 0) + 1
      return counts
    },
    {
      Pending: 0,
      'In Progress': 0,
      Escalated: 0,
      Blocked: 0,
      Completed: 0,
      Cancelled: 0,
    }
  )
}

const getRoomRiskStyle = (riskLevel) => {
  switch ((riskLevel || '').toLowerCase()) {
    case 'critical':
      return { label: '🔴 Critical', background: '#ffe5e5', border: '#ff4d4d' }
    case 'high':
      return { label: '🟠 High', background: '#fff3e0', border: '#ff9800' }
    case 'moderate':
      return { label: '🟡 Moderate', background: '#fffde7', border: '#fbc02d' }
    case 'low':
      return { label: '🟢 Low', background: '#e8f5e9', border: '#4caf50' }
    default:
      return { label: 'Unknown', background: '#f5f5f5', border: '#ccc' }
  }
}

const groupRoomsByZone = (rooms) => {
  return rooms.reduce((groups, room) => {
    const zone = room.zone || 'Unassigned Zone'

    if (!groups[zone]) {
      groups[zone] = []
    }

    groups[zone].push(room)
    return groups
  }, {})
}

function App() {
  const [tasks, setTasks] = useState([])
  const [summary, setSummary] = useState(null)
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(true)
  const [collapsedAreas, setCollapsedAreas] = useState({})
  const [rooms, setRooms] = useState([])

  const handleStatusUpdated = (updatedTask) => {
  setTasks((prevTasks) =>
    prevTasks.map((task) =>
      task.taskId === updatedTask.taskId ? updatedTask : task
    )
  )
}

  useEffect(() => {
    fetch('http://localhost:5000/api/tasks')
      .then((res) => {
        if (!res.ok) {
          throw new Error(`Tasks API returned ${res.status}`)
        }

        return res.json()
      })
      .then((data) => {
        setTasks(data)
        setLoading(false)
      })
      .catch((err) => {
        setError(err.message)
        setLoading(false)
      })

    fetch('http://localhost:5000/api/outbreak-summary')
      .then((res) => {
        if (!res.ok) {
          throw new Error(`Summary API returned ${res.status}`)
        }

        return res.json()
      })
      .then((data) => setSummary(data))
      .catch((err) => console.error('Summary fetch error:', err))

      fetch('http://localhost:5000/api/rooms')
        .then((res) => {
          if (!res.ok) {
            throw new Error(`Rooms API returned ${res.status}`)
          }
          return res.json()
        })
        .then((data) => setRooms(data))
        .catch((err) => console.error('Rooms fetch error:', err))
  }, [])

  const groupedTasks = groupTasksByOperationalArea(tasks)
  const priorityCounts = getPriorityCounts(tasks)
  const statusCounts = getStatusCounts(tasks)
  const groupedRooms = groupRoomsByZone(rooms)

  const toggleArea = (area) => {
    setCollapsedAreas((prev) => ({
      ...prev,
      [area]: !prev[area],
    }))
  }

  return (
    <div style={{ padding: '24px', fontFamily: 'Arial' }}>
      <h1>LOCC Incident Controller Dashboard</h1>

      {summary && (
        <div
          style={{
            display: 'grid',
            gridTemplateColumns: 'repeat(auto-fit, minmax(180px, 1fr))',
            gap: '12px',
            marginBottom: '24px',
          }}
        >
          <div style={{ padding: '12px', border: '2px solid #ccc', borderRadius: '10px' }}>
            <strong>Pathogen</strong>
            <div>{summary.pathogen}</div>
          </div>

          <div style={{ padding: '12px', border: '2px solid #ccc', borderRadius: '10px' }}>
            <strong>Outbreak Phase</strong>
            <div>{summary.outbreakPhase}</div>
          </div>

          {(() => {
            const style = getRiskStyle(summary.riskLevel)

            return (
              <div
                style={{
                  padding: '12px',
                  border: `2px solid ${style.border}`,
                  backgroundColor: style.background,
                  borderRadius: '10px',
                }}
              >
                <strong>Risk Level</strong>
                <div>{style.label}</div>
              </div>
            )
          })()}

          <div style={{ padding: '12px', border: '2px solid #ccc', borderRadius: '10px' }}>
            <strong>Active Cases</strong>
            <div>{summary.activeCases}</div>
          </div>

          {(() => {
            const style = getBauStyle(summary.bauScore)

            return (
              <div
                style={{
                  padding: '12px',
                  border: `2px solid ${style.border}`,
                  backgroundColor: style.background,
                  borderRadius: '10px',
                }}
              >
                <strong>BAU Score</strong>
                <div>{summary.bauScore}</div>
                <small>{style.label}</small>
              </div>
            )
          })()}

          {(() => {
            const style = getPpeWarningStyle(summary.ppeWarnings)

            return (
              <div
                style={{
                  padding: '12px',
                  border: `2px solid ${style.border}`,
                  backgroundColor: style.background,
                  borderRadius: '10px',
                }}
              >
                <strong>PPE Warnings</strong>
                <div>{summary.ppeWarnings}</div>
                <small>{style.label}</small>
              </div>
            )
          })()}
        </div>
      )}

      <div
        style={{
          display: 'grid',
          gridTemplateColumns: 'repeat(auto-fit, minmax(140px, 1fr))',
          gap: '12px',
          marginBottom: '24px',
        }}
      >
        {Object.entries(priorityCounts).map(([priority, count]) => {
          const style = getPriorityStyle(priority)

          return (
            <div
              key={priority}
              style={{
                border: `2px solid ${style.border}`,
                backgroundColor: style.background,
                borderRadius: '10px',
                padding: '12px',
                textAlign: 'center',
              }}
            >
              <strong>{style.label}</strong>
              <div style={{ fontSize: '24px', marginTop: '6px' }}>{count}</div>
            </div>
          )
        })}
      </div>

      <h2>Task Lifecycle Summary</h2>
      <div
        style={{
          display: 'grid',
          gridTemplateColumns: 'repeat(auto-fit, minmax(140px, 1fr))',
          gap: '12px',
          marginBottom: '24px',
        }}
      >
        {Object.entries(statusCounts).map(([status, count]) => {
          const style = getStatusStyle(status)

          return (
            <div
              key={status}
              style={{
                border: `2px solid ${style.border}`,
                backgroundColor: style.background,
                borderRadius: '10px',
                padding: '12px',
                textAlign: 'center',
              }}
            >
              <strong>{style.label}</strong>
              <div style={{ fontSize: '24px', marginTop: '6px' }}>{count}</div>
            </div>
          )
        })}
      </div>

      <section
  style={{
    marginBottom: '24px',
    padding: '16px',
    border: '2px solid #ddd',
    borderRadius: '12px',
    backgroundColor: '#fafafa',
  }}
>
  <h2>Environmental Risk Zones</h2>
  <p style={{ marginTop: 0 }}>
    Room-level outbreak zoning and IPC risk visibility.
  </p>

  {Object.entries(groupedRooms).map(([zone, zoneRooms]) => (
    <div key={zone} style={{ marginBottom: '20px' }}>
      <h3>{zone}</h3>

      <div
        style={{
          display: 'grid',
          gridTemplateColumns: 'repeat(auto-fit, minmax(180px, 1fr))',
          gap: '12px',
        }}
      >
        {zoneRooms.map((room) => {
          const style = getRoomRiskStyle(room.riskLevel)

          return (
            <div
              key={room.facilityRoomId}
              style={{
                padding: '12px',
                border: `2px solid ${style.border}`,
                backgroundColor: style.background,
                borderRadius: '10px',
              }}
            >
              <strong>{room.roomName}</strong>
              <p><strong>Risk:</strong> {style.label}</p>

              <div style={{ display: 'flex', flexWrap: 'wrap', gap: '6px' }}>
                {room.hasConfirmedCase && <small>Confirmed</small>}
                {room.hasSuspectedCase && <small>Suspected</small>}
                {room.isIsolationRoom && <small>Isolation</small>}
                {room.isClosed && <small>Closed</small>}
              </div>

              {room.notes && (
                <p style={{ fontSize: '13px', marginBottom: 0 }}>
                  {room.notes}
                </p>
              )}
            </div>
          )
        })}
      </div>
    </div>
  ))}
</section>

      {loading && <p>Loading tasks from LOCC API...</p>}

      {error && (
        <div style={{ padding: '12px', border: '1px solid red', color: 'red' }}>
          API connection error: {error}
        </div>
      )}

      {!loading && !error && (
        <>
          <p>Connected to API. Tasks loaded: {tasks.length}</p>

          {Object.entries(groupedTasks).map(([area, areaTasks]) => (
            <section
              key={area}
              style={{
                marginBottom: '24px',
                padding: '16px',
                border: '2px solid #ddd',
                borderRadius: '12px',
                backgroundColor: '#fafafa',
              }}
            >
              <div
                onClick={() => toggleArea(area)}
                style={{
                  cursor: 'pointer',
                  display: 'flex',
                  justifyContent: 'space-between',
                  alignItems: 'center',
                  gap: '12px',
                }}
              >
                <div>
                  <h2 style={{ marginBottom: '4px' }}>{area}</h2>
                  <p style={{ marginTop: 0 }}>{areaTasks.length} task(s)</p>
                </div>

                <strong style={{ fontSize: '24px' }}>
                  {collapsedAreas[area] ? '＋' : '−'}
                </strong>
              </div>

              {!collapsedAreas[area] && (
                <>
                  {areaTasks.map((task) => {
                    const style = getPriorityStyle(task.priority)

                    return (
                      <div
                        key={task.taskId}
                        style={{
                          border: `2px solid ${style.border}`,
                          backgroundColor: style.background,
                          padding: '12px',
                          marginBottom: '10px',
                          borderRadius: '8px',
                        }}
                      >
                        <strong>{task.taskDescription}</strong>
                        <p><strong>Priority:</strong> {style.label}</p>
                        <p><strong>Operational Area:</strong> {task.operationalArea}</p>
                        <TaskLifecycleUpdate
                          task={task}
                          onStatusUpdated={handleStatusUpdated}
                        />
                      </div>
                    )
                  })}
                </>
              )}
            </section>
          ))}
        </>
      )}
    </div>
  )
}

export default App