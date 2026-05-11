import { useEffect, useState } from 'react'

const getPriorityStyle = (priority) => {
  switch (priority) {

    case 4:
      return {
        label: '🟢 Complete',
        background: '#e8f5e9',
        border: '#4caf50'
      }

    case 3:
      return {
        label: '🔴 Critical',
        background: '#ffe5e5',
        border: '#ff4d4d'
      }

    case 2:
      return {
        label: '🟠 High',
        background: '#fff3e0',
        border: '#ff9800'
      }

    case 1:
      return {
        label: '🟡 Moderate',
        background: '#fffde7',
        border: '#fbc02d'
      }

    default:
      return {
        label: '🔵 Low',
        background: '#e3f2fd',
        border: '#64b5f6'
      }
  }
}

function App() {
  const [tasks, setTasks] = useState([])
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(true)

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

          {tasks.map((task) => (
            <div
              key={task.taskId}
              style={{
                border: `2px solid ${getPriorityStyle(task.priority).border}`,
                backgroundColor: getPriorityStyle(task.priority).background,
                padding: '12px',
                marginBottom: '10px',
                borderRadius: '8px',
              }}
            >
              <strong>{task.taskDescription}</strong>
              <p>
                <strong>Priority:</strong>{' '}
                {getPriorityStyle(task.priority).label}
              </p>
              <p><strong>Domain:</strong> {task.aiimsFunctionLabel}</p>
            </div>
          ))}
        </>
      )}
    </div>
  )
}

export default App