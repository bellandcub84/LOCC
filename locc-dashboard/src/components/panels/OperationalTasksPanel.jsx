import TaskLifecycleUpdate from '../TaskLifecycleUpdate'
import { panelStyle, typography, statusColors } from '../../styles/theme'

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

const groupTasksByOperationalArea = (tasks = []) => {
  return tasks.reduce((groups, task) => {
    const area = task.operationalArea || 'Unassigned'
    groups[area] = groups[area] || []
    groups[area].push(task)
    return groups
  }, {})
}

const OperationalTasksPanel = ({
  tasks = [],
  collapsedAreas = {},
  toggleArea,
  expandedRationale = {},
  toggleRationale,
  onStatusUpdated,
}) => {
  const groupedTasks = groupTasksByOperationalArea(tasks)
  const now = new Date()

  const escalatedCount = tasks.filter((t) => t.status === 'Escalated').length
  const completedCount = tasks.filter((t) => t.status === 'Completed').length

  const overdueCount = tasks.filter((t) => {
    if (!t.dueDateTime) return false
    return new Date(t.dueDateTime) < now && t.status !== 'Completed'
  }).length

  const dueTodayCount = tasks.filter((t) => {
    if (!t.dueDateTime) return false
    const due = new Date(t.dueDateTime)
    return due.toDateString() === now.toDateString() && t.status !== 'Completed'
  }).length

  return (
    <section style={panelStyle}>
      <h2 style={{ marginTop: 0, color: typography.headingColor }}>
        Operational Actions
      </h2>

      <div style={metricGridStyle}>
        <TaskMetricCard title="Escalated" value={escalatedCount} tone="critical" />
        <TaskMetricCard title="Due Today" value={dueTodayCount} tone="high" />
        <TaskMetricCard title="Overdue" value={overdueCount} tone="moderate" />
        <TaskMetricCard title="Completed" value={completedCount} tone="low" />
      </div>

      {tasks.length === 0 && <p>No operational tasks currently loaded.</p>}

      {Object.entries(groupedTasks).map(([area, areaTasks]) => (
        <section key={area} style={areaSectionStyle}>
          <div onClick={() => toggleArea?.(area)} style={areaHeaderStyle}>
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
                  borderLeft: `6px solid ${style.border}`,
                  backgroundColor: '#ffffff',
                  padding: '16px',
                }}
              >
                <div
                  style={{
                    display: 'flex',
                    justifyContent: 'space-between',
                    gap: '12px',
                    alignItems: 'flex-start',
                    flexWrap: 'wrap',
                  }}
                >
                  <div>
                    <div
                      style={{
                        fontSize: '16px',
                        fontWeight: 'bold',
                        color: style.text,
                      }}
                    >
                      {task.taskDescription}
                    </div>

                    <div
                      style={{
                        marginTop: '6px',
                        fontSize: '13px',
                        opacity: 0.8,
                      }}
                    >
                      {task.operationalArea}
                    </div>
                  </div>

                  <div
                    style={{
                      display: 'flex',
                      gap: '8px',
                      flexWrap: 'wrap',
                    }}
                  >
                    <span style={priorityBadgeStyle(style)}>
                      {task.priority}
                    </span>

                    <span style={statusBadgeStyle}>
                      {task.status}
                    </span>
                  </div>
                </div>

                  <div style={badgeContainerStyle}>
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
                        type="button"
                        onClick={() => toggleRationale?.(task.taskId)}
                        style={linkButtonStyle}
                      >
                        {expandedRationale[task.taskId]
                          ? 'Hide operational rationale'
                          : 'View operational rationale'}
                      </button>

                      {expandedRationale[task.taskId] && (
                        <div style={rationaleBoxStyle}>
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

const TaskMetricCard = ({ title, value, tone }) => {
  const toneStyles = {
    critical: statusColors.critical,
    high: statusColors.high,
    moderate: statusColors.moderate,
    low: statusColors.low || {
      background: '#e8f5e9',
      border: '#4caf50',
      text: '#1b5e20',
    },
  }

  const style = toneStyles[tone] || statusColors.info

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
          marginBottom: '6px',
          color: style.text,
        }}
      >
        {title}
      </div>

      <div style={{ fontSize: '26px', fontWeight: 'bold', color: style.text }}>
        {value}
      </div>
    </div>
  )
}

const metricGridStyle = {
  display: 'grid',
  gridTemplateColumns: 'repeat(auto-fit, minmax(140px, 1fr))',
  gap: '12px',
  marginBottom: '20px',
}

const areaSectionStyle = {
  marginBottom: '16px',
  border: '1px solid #ddd',
  borderRadius: '14px',
  overflow: 'hidden',
}

const areaHeaderStyle = {
  cursor: 'pointer',
  padding: '14px',
  backgroundColor: '#F7F5F2',
  display: 'flex',
  justifyContent: 'space-between',
  alignItems: 'center',
}

const badgeContainerStyle = {
  display: 'flex',
  gap: '8px',
  flexWrap: 'wrap',
  marginTop: '8px',
  marginBottom: '8px',
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

const rationaleBoxStyle = {
  marginTop: '8px',
  padding: '8px',
  backgroundColor: 'rgba(255,255,255,0.7)',
  borderLeft: '4px solid #ccc',
  borderRadius: '6px',
  fontSize: '13px',
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

const priorityBadgeStyle = (style) => ({
  backgroundColor: 'rgba(255,255,255,0.75)',
  border: `1px solid ${style.border}`,
  color: style.text,
  padding: '4px 8px',
  borderRadius: '999px',
  fontSize: '12px',
  fontWeight: 'bold',
})

const statusBadgeStyle = {
  backgroundColor: 'rgba(255,255,255,0.75)',
  border: '1px solid #ccc',
  color: '#2E4057',
  padding: '4px 8px',
  borderRadius: '999px',
  fontSize: '12px',
  fontWeight: 'bold',
}

export default OperationalTasksPanel