export const colors = {
  navy: '#2E4057',
  sage: '#7C9A92',
  forest: '#5E756B',
  lavender: '#B7A7D9',
  amber: '#D9B26F',
  coral: '#C97C73',
  offWhite: '#F7F5F2',
  white: '#FFFFFF',
  border: '#DDDDDD',
  mutedText: '#6B7280',
}

export const spacing = {
  xs: '4px',
  sm: '8px',
  md: '12px',
  lg: '16px',
  xl: '24px',
  xxl: '32px',
}

export const radius = {
  sm: '6px',
  md: '10px',
  lg: '12px',
  xl: '16px',
  pill: '999px',
}

export const shadows = {
  soft: '0 2px 6px rgba(0,0,0,0.05)',
  medium: '0 4px 12px rgba(0,0,0,0.08)',
}

export const typography = {
  fontFamily: 'Arial, sans-serif',
  headingColor: colors.navy,
  bodyColor: colors.navy,
}

export const statusColors = {
  critical: {
    background: '#FFE5E5',
    border: '#FF4D4D',
    text: '#8A1F1F',
  },
  high: {
    background: '#FFF3E0',
    border: '#FF9800',
    text: '#8A4B00',
  },
  moderate: {
    background: '#FFFDE7',
    border: '#FBC02D',
    text: '#6B5B00',
  },
  low: {
    background: '#E8F5E9',
    border: '#4CAF50',
    text: '#1B5E20',
  },
  info: {
    background: '#E3F2FD',
    border: '#2196F3',
    text: '#0D47A1',
  },
  neutral: {
    background: '#F5F5F5',
    border: '#CCCCCC',
    text: '#555555',
  },
}

export const panelStyle = {
  padding: spacing.xl,
  border: `1px solid ${colors.border}`,
  borderRadius: radius.xl,
  backgroundColor: colors.white,
  boxShadow: shadows.soft,
  marginBottom: spacing.xl,
}

export const metricCardStyle = {
  backgroundColor: colors.offWhite,
  borderRadius: radius.lg,
  padding: spacing.lg,
}