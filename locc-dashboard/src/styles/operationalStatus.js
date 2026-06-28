export const operationalStatus = {
  stable: {
    label: '🟢 Stable',
    background: '#E8F5E9',
    border: '#4CAF50',
  },
  monitor: {
    label: '🟡 Monitor',
    background: '#FFFDE7',
    border: '#FBC02D',
  },
  underPressure: {
    label: '🟠 Under pressure',
    background: '#FFF3E0',
    border: '#FF9800',
  },
  critical: {
    label: '🔴 Critical',
    background: '#FFE5E5',
    border: '#FF4D4D',
  },
  information: {
    label: '🔵 Information',
    background: '#E3F2FD',
    border: '#64B5F6',
  },
  unavailable: {
    label: 'Not yet available',
    background: '#F5F5F5',
    border: '#CCCCCC',
  },
}

export const getOperationalStatusStyle = (status) => {
  switch ((status || '').toLowerCase()) {
    case 'stable':
      return operationalStatus.stable
    case 'monitor':
      return operationalStatus.monitor
    case 'under pressure':
    case 'action required':
      return operationalStatus.underPressure
    case 'critical':
      return operationalStatus.critical
    case 'information':
      return operationalStatus.information
    case 'not yet available':
      return operationalStatus.unavailable
    case 'high':
      return operationalStatus.critical
    case 'medium':
      return operationalStatus.underPressure
    case 'low':
      return operationalStatus.monitor
    case 'warning':
      return operationalStatus.underPressure
        default:
        return {
        label: status || 'Not available',
        background: operationalStatus.unavailable.background,
        border: operationalStatus.unavailable.border,
      }
  }
}