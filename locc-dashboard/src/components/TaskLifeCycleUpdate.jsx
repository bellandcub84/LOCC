const TASK_STATUSES = [
  'Pending',
  'In Progress',
  'Escalated',
  'Blocked',
  'Completed',
  'Cancelled',
]

const getStatusStyle = (status) => {
  switch (status) {
    case 'Completed':
      return { background: '#e8f5e9', border: '#4caf50', label: '🟢 Completed' }
    case 'In Progress':
      return { background: '#e3f2fd', border: '#2196f3', label: '🔵 In Progress' }
    case 'Escalated':
      return { background: '#ffe5e5', border: '#ff4d4d', label: '🔴 Escalated' }
    case 'Blocked':
      return { background: '#fff3e0', border: '#ff9800', label: '🟠 Blocked' }
    case 'Cancelled':
      return { background: '#f5f5f5', border: '#999', label: '⚪ Cancelled' }
    default:
      return { background: '#fffde7', border: '#fbc02d', label: '🟡 Pending' }
  }
}

function TaskLifecycleUpdate({ task, onStatusUpdated }) {
  const currentStyle = getStatusStyle(task.status)

  const handleStatusChange = async (event) => {
    const newStatus = event.target.value

    try {
      const response = await fetch(
        `http://localhost:5000/api/tasks/${task.taskId}/status`,
        {
          method: 'PATCH',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({ status: newStatus }),
        }
      )

      if (!response.ok) {
        throw new Error(`Status update failed: ${response.status}`)
      }

      const updatedTask = await response.json()
      onStatusUpdated(updatedTask)
    } catch (error) {
      console.error('Task status update error:', error)
      alert('Unable to update task status. Check API connection.')
    }
  }

  return (
    <div
      style={{
        marginTop: '10px',
        padding: '10px',
        border: `2px solid ${currentStyle.border}`,
        backgroundColor: currentStyle.background,
        borderRadius: '8px',
      }}
    >
      <label>
        <strong>Status:</strong>
      </label>

      <select
        value={task.status || 'Pending'}
        onChange={handleStatusChange}
        style={{
          display: 'block',
          marginTop: '6px',
          padding: '8px',
          borderRadius: '6px',
          border: '1px solid #aaa',
          width: '100%',
          maxWidth: '260px',
          backgroundColor: 'white',
        }}
      >
        {TASK_STATUSES.map((status) => (
          <option key={status} value={status}>
            {getStatusStyle(status).label}
          </option>
        ))}
      </select>
    </div>
  )
}

export default TaskLifecycleUpdate