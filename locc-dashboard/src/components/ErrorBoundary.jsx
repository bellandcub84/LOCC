import React from 'react'

class ErrorBoundary extends React.Component {
  constructor(props) {
    super(props)

    this.state = {
      hasError: false,
      error: null,
    }
  }

  static getDerivedStateFromError(error) {
    return {
      hasError: true,
      error,
    }
  }

  componentDidCatch(error, errorInfo) {
    console.error('LOCC dashboard rendering error:', error, errorInfo)
  }

  render() {
    if (this.state.hasError) {
      return (
        <div
          style={{
            padding: '24px',
            fontFamily: 'Arial, sans-serif',
            color: '#2E4057',
            backgroundColor: '#F7F5F2',
            minHeight: '100vh',
          }}
        >
          <div
            style={{
              backgroundColor: '#ffffff',
              border: '1px solid #ddd',
              borderRadius: '16px',
              padding: '24px',
              maxWidth: '720px',
              margin: '48px auto',
            }}
          >
            <h1>LOCC Dashboard Recovery Mode</h1>

            <p>
              A dashboard panel encountered an unexpected rendering issue.
            </p>

            <p>
              Core outbreak data may still be available after refreshing.
            </p>

            {this.state.error && (
              <pre
                style={{
                  backgroundColor: '#f5f5f5',
                  padding: '12px',
                  borderRadius: '8px',
                  overflowX: 'auto',
                  fontSize: '12px',
                }}
              >
                {this.state.error.message}
              </pre>
            )}

            <button
              onClick={() => window.location.reload()}
              style={{
                marginTop: '16px',
                padding: '10px 14px',
                borderRadius: '10px',
                border: 'none',
                backgroundColor: '#2E4057',
                color: '#ffffff',
                cursor: 'pointer',
              }}
            >
              Reload Dashboard
            </button>
          </div>
        </div>
      )
    }

    return this.props.children
  }
}

export default ErrorBoundary