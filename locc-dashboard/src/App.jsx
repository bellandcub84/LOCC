import { useEffect, useState } from 'react'

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
                border: '1px solid #ccc',
                padding: '12px',
                marginBottom: '10px',
                borderRadius: '8px',
              }}
            >
              <strong>{task.taskDescription}</strong>
              <p>Priority: {task.priority}</p>
              <p>AIIMS Function: {task.aiimsFunction}</p>
            </div>
          ))}
        </>
      )}
    </div>
  )
}

export default App