import { useEffect, useState } from 'react'

const getPriorityStyle = (priority) => {
  switch (priority) {
    case 'Complete':
      return { label: '🟢 Complete', background: '#e8f5e9', border: '#4caf50' }
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

function App() {
  const [tasks, setTasks] = useState([])
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(true)
  const [collapsedAreas, setCollapsedAreas] = useState({})

  useEffect(() => {
    fetch('http://localhost:5000/api/tasks')
      .then((res) => {
        if (!res.ok) {
          throw new Error(`API returned ${res.status}`)
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
  }, [])

  const groupedTasks = groupTasksByOperationalArea(tasks)

  const toggleArea = (area) => {
    setCollapsedAreas((prev) => ({
      ...prev,
      [area]: !prev[area],
    }))
  }

  return (
    <div style={{ padding: '24px', fontFamily: 'Arial' }}>
      <h1>LOCC Incident Controller Dashboard</h1>

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
                        <p><strong>Status:</strong> {task.status}</p>
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