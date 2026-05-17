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
    groups[area] = groups[area] || []
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
    { Critical: 0, High: 0, Moderate: 0, Low: 0 }
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
  if (score >= 80) return { label: '🟢 Ready', background: '#e8f5e9', border: '#4caf50' }
  if (score >= 60) return { label: '🟡 Caution', background: '#fffde7', border: '#fbc02d' }
  return { label: '🔴 Not ready', background: '#ffe5e5', border: '#ff4d4d' }
}

const getPpeWarningStyle = (warnings) => {
  if (warnings >= 2) return { label: '🔴 Action required', background: '#ffe5e5', border: '#ff4d4d' }
  if (warnings === 1) return { label: '🟠 Monitor', background: '#fff3e0', border: '#ff9800' }
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
    groups[zone] = groups[zone] || []
    groups[zone].push(room)
    return groups
  }, {})
}

function App() {
  const [tasks, setTasks] = useState([])
  const [summary, setSummary] = useState(null)
  const [resources, setResources] = useState([])
  const [zones, setZones] = useState([])
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(true)
  const [collapsedAreas, setCollapsedAreas] = useState({})
  const [expandedRationale, setExpandedRationale] = useState({})
  const [rooms, setRooms] = useState([])
  const [ppeResult, setPpeResult] = useState(null)
  const [ppeLoading, setPpeLoading] = useState(false)

  useEffect(() => {
    fetch('http://localhost:5000/api/tasks')
      .then((res) => {
        if (!res.ok) throw new Error(`Tasks API returned ${res.status}`)
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
        if (!res.ok) throw new Error(`Summary API returned ${res.status}`)
        return res.json()
      })
      .then((data) => setSummary(data))
      .catch((err) => console.error('Summary fetch error:', err))

    fetch('http://localhost:5000/api/rooms')
      .then((res) => {
        if (!res.ok) throw new Error(`Rooms API returned ${res.status}`)
        return res.json()
      })
      .then((data) => setRooms(data))
      .catch((err) => console.error('Rooms fetch error:', err))

      fetch('http://localhost:5000/api/resources')
      .then((res) => res.json())
      .then((data) => setResources(data))
      .catch((err) => console.error('Resources fetch error:', err))

    fetch('http://localhost:5000/api/zones')
      .then((res) => res.json())
      .then((data) => setZones(data))
      .catch((err) => console.error('Zones fetch error:', err))
  }, [])

  const handleStatusUpdated = (updatedTask) => {
    setTasks((prevTasks) =>
      prevTasks.map((task) =>
        task.taskId === updatedTask.taskId ? updatedTask : task
      )
    )
  }

  const toggleArea = (area) => {
    setCollapsedAreas((prev) => ({
      ...prev,
      [area]: !prev[area],
    }))
  }

  const toggleRationale = (taskId) => {
  setExpandedRationale((prev) => ({
    ...prev,
    [taskId]: !prev[taskId],
  }))
}

const updateZone = async (roomId, updates) => {
  try {
    const response = await fetch(
      `http://localhost:5000/api/zones/${roomId}`,
      {
        method: 'PATCH',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(updates),
      }
    )

    if (!response.ok) {
      throw new Error('Failed to update room')
    }

    const updatedZone = await response.json()

    setZones((prev) =>
      prev.map((zone) =>
        zone.facilityRoomId === updatedZone.facilityRoomId
          ? updatedZone
          : zone
      )
    )
  } catch (err) {
    console.error('Zone update error:', err)
  }
}

  const calculatePpeForecast = () => {
    setPpeLoading(true)

    const request = {
      facilityName: 'LOCC Manor',
      totalResidents: 100,
      singleAssistResidents: 75,
      twoPersonAssistResidents: 20,
      threePlusPersonAssistResidents: 5,
      contactDropletResidents: 10,
      contactOnlyResidents: 5,
      staffOnShift: 18,
      visitorsPerDay: 12,
      outbreakType: 'COVID-19',
      contactDropletInterventions: {
        regularNebulisers: 2,
        prnNebulisers: 1,
        regularOxygen: 3,
        prnOxygen: 1,
        cpapBipap: 2,
        oralSuctioning: 1,
        assistedFeeding: 6,
      },
      contactOnlyInterventions: {
        regularNebulisers: 0,
        prnNebulisers: 0,
        regularOxygen: 0,
        prnOxygen: 0,
        cpapBipap: 0,
        oralSuctioning: 0,
        assistedFeeding: 4,
      },
    }

    fetch('http://localhost:5000/api/ppe/calculate', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(request),
    })
      .then((res) => {
        if (!res.ok) throw new Error(`PPE API returned ${res.status}`)
        return res.json()
      })
      .then((data) => setPpeResult(data))
      .catch((err) => console.error('PPE calculation error:', err))
      .finally(() => setPpeLoading(false))
  }

  const groupedTasks = groupTasksByOperationalArea(tasks)
  const priorityCounts = getPriorityCounts(tasks)
  const statusCounts = getStatusCounts(tasks)
  const groupedRooms = groupRoomsByZone(rooms)

  return (
    <div style={{ padding: '24px', fontFamily: 'Arial' }}>
      <h1>LOCC Incident Controller Dashboard</h1>

      <section style={{ marginBottom: '24px', padding: '16px', border: '2px solid #ddd', borderRadius: '12px', backgroundColor: '#f8fbff' }}>
        <h2>PPE Forecast Calculator</h2>
        <p>Estimate PPE consumption across outbreak growth scenarios.</p>

        <button onClick={calculatePpeForecast} disabled={ppeLoading}>
          {ppeLoading ? 'Calculating...' : 'Calculate PPE Forecast'}
        </button>

        {ppeResult && (
          <div style={{ marginTop: '16px', overflowX: 'auto' }}>
            <strong>Facility:</strong> {ppeResult.facilityName} |{' '}
            <strong>Total Residents:</strong> {ppeResult.totalResidents} |{' '}
            <strong>Single Assist:</strong> {ppeResult.singleAssistResidents}

            <table style={{ width: '100%', borderCollapse: 'collapse', marginTop: '12px' }}>
              <thead>
                <tr>
                  {['Affected %', 'Residents', 'Gloves', 'Gowns', 'Aprons', 'Surgical Masks', 'N95/P2', 'Eye Protection'].map((header) => (
                    <th key={header} style={{ border: '1px solid #ddd', padding: '8px', backgroundColor: '#e3f2fd' }}>
                      {header}
                    </th>
                  ))}
                </tr>
              </thead>
              <tbody>
                {ppeResult.scenarios.map((row) => (
                  <tr key={row.percentAffected}>
                    <td>{row.percentAffected}%</td>
                    <td>{row.estimatedAffectedResidents}</td>
                    <td>{row.gloves}</td>
                    <td>{row.gowns}</td>
                    <td>{row.aprons}</td>
                    <td>{row.surgicalMasks}</td>
                    <td>{row.n95Respirators}</td>
                    <td>{row.eyeProtection}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </section>

      {summary && (
        <section style={{ marginBottom: '24px' }}>
          <h2>Outbreak Summary</h2>
          <p><strong>Pathogen:</strong> {summary.pathogen}</p>
          <p><strong>Outbreak Phase:</strong> {summary.outbreakPhase}</p>
          <p><strong>Active Cases:</strong> {summary.activeCases}</p>
        </section>
      )}

<div
  style={{
    display: 'grid',
    gridTemplateColumns: 'repeat(auto-fit, minmax(280px, 1fr))',
    gap: '16px',
    marginBottom: '24px',
  }}
>
  <section
    style={{
      padding: '16px',
      border: '2px solid #ddd',
      borderRadius: '12px',
      backgroundColor: '#fafafa',
    }}
  >
    <h2>PPE Sustainability</h2>

    {resources.length === 0 && <p>No PPE resource data available.</p>}

    {resources.map((resource) => (
      <div
        key={resource.resourceId}
        style={{
          padding: '10px',
          marginBottom: '8px',
          borderRadius: '8px',
          border: '1px solid #ddd',
          backgroundColor:
            resource.projectedDaysRemaining <= resource.reorderThreshold
              ? '#ffe5e5'
              : '#e8f5e9',
        }}
      >
        <strong>{resource.itemName}</strong>
        <p>Current stock: {resource.currentStockLevel}</p>
        <p>Daily usage: {resource.dailyUsageRate}</p>
        <p>Projected days remaining: {resource.projectedDaysRemaining}</p>
      </div>
    ))}
  </section>

    <section
      style={{
        padding: '16px',
        border: '2px solid #ddd',
        borderRadius: '12px',
        backgroundColor: '#fafafa',
      }}
    >
      <h2>Environmental Risk Zones</h2>

      {zones.length === 0 && <p>No zoning data available.</p>}

      {zones.map((zone) => (
        <div
          key={zone.roomId}
          style={{
            padding: '10px',
            marginBottom: '8px',
            borderRadius: '8px',
            border: '1px solid #ddd',
            backgroundColor:
              zone.riskZoneStatus === 'Red'
                ? '#ffe5e5'
                : zone.riskZoneStatus === 'Amber'
                ? '#fff3e0'
                : '#e8f5e9',
          }}
        >
          <strong>{zone.roomName}</strong>
          <p>Risk zone: {zone.riskZoneStatus}</p>
          {zone.enhancedPrecautionsRequired && <p>⚠ Enhanced precautions required</p>}
          {zone.terminalCleanRequired && <p>🧼 Terminal clean required</p>}
        </div>
      ))}
    </section>
  </div>

      <section>
        <h2>Priority Summary</h2>
        {Object.entries(priorityCounts).map(([priority, count]) => {
          const style = getPriorityStyle(priority)

          return (
            <div key={priority} style={{ border: `2px solid ${style.border}`, backgroundColor: style.background, padding: '12px', marginBottom: '8px' }}>
              <strong>{style.label}</strong>: {count}
            </div>
          )
        })}
      </section>

      <section>
        <h2>Task Lifecycle Summary</h2>
        {Object.entries(statusCounts).map(([status, count]) => {
          const style = getStatusStyle(status)

          return (
            <div key={status} style={{ border: `2px solid ${style.border}`, backgroundColor: style.background, padding: '12px', marginBottom: '8px' }}>
              <strong>{style.label}</strong>: {count}
            </div>
          )
        })}
      </section>

      <section
        style={{
          padding: '16px',
          border: '2px solid #ddd',
          borderRadius: '12px',
          backgroundColor: '#fafafa',
        }}
      >
        <h2>Environmental Risk Zones</h2>

        {zones.length === 0 && <p>No zoning data available.</p>}

        {zones.map((zone) => (
          <div
            key={zone.facilityRoomId}
            style={{
              padding: '10px',
              marginBottom: '8px',
              borderRadius: '8px',
              border: '1px solid #ddd',
              backgroundColor:
                zone.riskLevel === 'High'
                  ? '#ffe5e5'
                  : zone.riskLevel === 'Moderate'
                  ? '#fff3e0'
                  : '#e8f5e9',
            }}
          >
            <strong>{zone.roomName}</strong>
            <div
              style={{
                display: 'flex',
                flexWrap: 'wrap',
                gap: '8px',
                marginTop: '10px',
                marginBottom: '10px',
              }}
            >
              <select
                value={zone.riskLevel}
                onChange={(e) =>
                  updateZone(zone.facilityRoomId, {
                    riskLevel: e.target.value,
                  })
                }
              >
                <option value="Low">Low</option>
                <option value="Moderate">Moderate</option>
                <option value="High">High</option>
              </select>

              <button
                onClick={() =>
                  updateZone(zone.facilityRoomId, {
                    isClosed: !zone.isClosed,
                  })
                }
              >
                {zone.isClosed ? 'Reopen Room' : 'Close Room'}
              </button>

              <button
                onClick={() =>
                  updateZone(zone.facilityRoomId, {
                    isIsolationRoom: !zone.isIsolationRoom,
                  })
                }
              >
                {zone.isIsolationRoom ? 'Remove Isolation' : 'Assign Isolation'}
              </button>

              <button
                onClick={() =>
                  updateZone(zone.facilityRoomId, {
                    terminalCleanCompleted: true,
                  })
                }
              >
                Acknowledge Cleaning
              </button>
            </div>

            <p>
              <strong>Zone:</strong> {zone.zone}
            </p>

            <p>
              <strong>Risk Level:</strong> {zone.riskLevel}
            </p>

            {zone.hasConfirmedCase && (
              <p>🔴 Confirmed outbreak-associated case</p>
            )}

            {zone.hasSuspectedCase && (
              <p>🟠 Suspected outbreak-associated case</p>
            )}

            {zone.isIsolationRoom && (
              <p>🛏 Isolation-capable room</p>
            )}

            {zone.isClosed && (
              <p>⛔ Area closed</p>
            )}

            {zone.notes && (
              <div
                style={{
                  marginTop: '8px',
                  padding: '8px',
                  backgroundColor: '#f5f5f5',
                  borderLeft: '4px solid #ccc',
                  borderRadius: '4px',
                  fontSize: '13px',
                }}
              >
                {zone.notes}
              </div>
            )}
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
            <section key={area} style={{ marginBottom: '24px', padding: '16px', border: '2px solid #ddd', borderRadius: '12px' }}>
              <div onClick={() => toggleArea(area)} style={{ cursor: 'pointer' }}>
                <h2>{area}</h2>
                <p>{areaTasks.length} task(s)</p>
                <strong>{collapsedAreas[area] ? '+' : '-'}</strong>
              </div>

              {!collapsedAreas[area] &&
                areaTasks.map((task) => {
                  const style = getPriorityStyle(task.priority)

                  return (
                    <div key={task.taskId} style={{ border: `2px solid ${style.border}`, backgroundColor: style.background, padding: '12px', marginBottom: '10px' }}>
                      <strong>{task.taskDescription}</strong>
                      <p><strong>Priority:</strong> {style.label}</p>
                      <p><strong>Operational Area:</strong> {task.operationalArea}</p>
                      <p><strong>Status:</strong> {task.status}</p>
                    <div

                    style={{
                      display: 'flex',
                      gap: '8px',
                      flexWrap: 'wrap',
                      marginTop: '8px',
                      marginBottom: '8px',
                    }}
                  >
                    {task.generatedFrom && (
                      <span
                        style={{
                          backgroundColor: '#fff3e0',
                          color: '#e65100',
                          padding: '4px 8px',
                          borderRadius: '999px',
                          fontSize: '12px',
                          fontWeight: 'bold',
                        }}
                      >
                        ⚠ Risk Linked
                      </span>
                    )}

                    {task.decisionRationale && (
                      <span
                        style={{
                          backgroundColor: '#e8f5e9',
                          color: '#1b5e20',
                          padding: '4px 8px',
                          borderRadius: '999px',
                          fontSize: '12px',
                          fontWeight: 'bold',
                        }}
                      >
                        💡 Recommendation Generated
                      </span>
                    )}
                  </div>

                      {task.generatedFrom && (
                        <p style={{ fontSize: '12px', opacity: 0.75 }}>
                          <strong>Generated From:</strong> {task.generatedFrom}
                        </p>
                      )}

                      {task.decisionRationale && (
                        <div style={{ marginTop: '8px' }}>
                          <button
                            onClick={() => toggleRationale(task.taskId)}
                            style={{
                              border: 'none',
                              background: 'none',
                              color: '#1976d2',
                              cursor: 'pointer',
                              padding: 0,
                              fontSize: '13px',
                              fontWeight: 'bold',
                            }}
                          >
                            {expandedRationale[task.taskId]
                              ? 'Hide operational rationale'
                              : 'View operational rationale'}
                          </button>

                          {expandedRationale[task.taskId] && (
                            <div
                              style={{
                                marginTop: '8px',
                                padding: '8px',
                                backgroundColor: '#f5f5f5',
                                borderLeft: '4px solid #ccc',
                                borderRadius: '4px',
                                fontSize: '13px',
                              }}
                            >
                              <strong>Why this matters:</strong>
                              <div>{task.decisionRationale}</div>
                            </div>
                          )}
                        </div>
                      )}

                      <TaskLifecycleUpdate
                        task={task}
                        onStatusUpdated={handleStatusUpdated}
                      />
                    </div>
                  )
                })}
            </section>
          ))}
        </>
      )}
    </div>
  )
}

export default App