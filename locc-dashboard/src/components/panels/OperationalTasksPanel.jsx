import TaskLifecycleUpdate from '../TaskLifecycleUpdate'
import {
  panelStyle,
  typography,
  statusColors,
} from '../../styles/theme'

const getPriorityStyle = (priority) => {
  switch (priority) {
    case 'Critical':
      return statusColors.critical
    case 'High':
      return statusColors.high
    case 'Moderate':
      return statusColors.moderate
    default:
      return statusColors.info
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

const OperationalTasksPanel = ({
  tasks,
  collapsedAreas,
  toggleArea,
  expandedRationale,
  toggleRationale,
  onStatusUpdated,
}) => {
  const groupedTasks = groupTasksByOperationalArea(tasks)

  return (
    <section style={panelStyle}>
      <h2
        style={{
          marginTop: 0,
          color: typography.headingColor,
        }}
      >
        Operational Actions
      </h2>

      {tasks.length === 0 && <p>No operational tasks currently loaded.</p>}

      {Object.entries(groupedTasks).map(([area, areaTasks]) => (
        <section
          key={area}
          style={{
            marginBottom: '16px',
            border: '1px solid #ddd',
            borderRadius: '14px',
            overflow: 'hidden',
          }}
        >
          <div
            onClick={() => toggleArea(area)}
            style={{
              cursor: 'pointer',
              padding: '14px',
              backgroundColor: '#F7F5F2',
              display: 'flex',
              justifyContent: 'space-between',
              alignItems: 'center',
            }}
          >
            <div>
              <strong>{area}</strong>
              <div style={{ fontSize: '12px', opacity: 0.7 }}>
                {areaTasks.length} action(s)
              </div>
            </div>

            <strong>{collapsedAreas[area] ? '+' : '-'}</strong>
          </div>

          {!collapsedAreas[area] &&
            areaTasks.map((task) => {
              const style = getPriorityStyle(task.priority)

              return (
                <div
                  key={task.taskId}
                  style={{
                    borderTop: '1px solid #eee',
                    backgroundColor: style.background,
                    padding: '14px',
                  }}
                >
                  <strong>{task.taskDescription}</strong>

                  <p>
                    <strong>Priority:</strong> {task.priority}
                  </p>

                  <p>
                    <strong>Status:</strong> {task.status}
                  </p>

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
                      <span style={badgeStyleWarning}>⚠ Risk Linked</span>
                    )}

                    {task.decisionRationale && (
                      <span style={badgeStyleSuccess}>
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
                        style={linkButtonStyle}
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
                            backgroundColor: 'rgba(255,255,255,0.7)',
                            borderLeft: '4px solid #ccc',
                            borderRadius: '6px',
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
                    onStatusUpdated={onStatusUpdated}
                  />
                </div>
              )
            })}
        </section>
      ))}
    </section>
  )
}

const badgeStyleWarning = {
  backgroundColor: '#FFF3E0',
  color: '#8A4B00',
  padding: '4px 8px',
  borderRadius: '999px',
  fontSize: '12px',
  fontWeight: 'bold',
}

const badgeStyleSuccess = {
  backgroundColor: '#E8F5E9',
  color: '#1B5E20',
  padding: '4px 8px',
  borderRadius: '999px',
  fontSize: '12px',
  fontWeight: 'bold',
}

const linkButtonStyle = {
  border: 'none',
  background: 'none',
  color: '#2E4057',
  cursor: 'pointer',
  padding: 0,
  fontSize: '13px',
  fontWeight: 'bold',
}

export default OperationalTasksPanel