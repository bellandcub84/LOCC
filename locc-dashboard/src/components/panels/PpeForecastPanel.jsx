import { useState } from 'react'
import {
  panelStyle,
  typography,
  statusColors,
} from '../../styles/theme'

const PpeForecastPanel = () => {
  const [ppeResult, setPpeResult] = useState(null)
  const [ppeLoading, setPpeLoading] = useState(false)

  const calculatePpeForecast = async () => {
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

    try {
      const response = await fetch(
        'http://localhost:5000/api/ppe/calculate',
        {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(request),
        }
      )

      if (!response.ok) {
        throw new Error(`PPE API returned ${response.status}`)
      }

      const data = await response.json()
      setPpeResult(data)
    } catch (err) {
      console.error('PPE calculation error:', err)
      setPpeResult(null)
    } finally {
      setPpeLoading(false)
    }
  }

  return (
    <section style={panelStyle}>
      <h2
        style={{
          marginTop: 0,
          color: typography.headingColor,
        }}
      >
        PPE Forecast Calculator
      </h2>

      <p>Estimate PPE consumption across outbreak growth scenarios.</p>

      <button onClick={calculatePpeForecast} disabled={ppeLoading}>
        {ppeLoading ? 'Calculating...' : 'Calculate PPE Forecast'}
      </button>

      {ppeResult && Array.isArray(ppeResult.scenarios) && (
        <div style={{ marginTop: '16px', overflowX: 'auto' }}>
          <strong>Facility:</strong> {ppeResult.facilityName} |{' '}
          <strong>Total Residents:</strong> {ppeResult.totalResidents} |{' '}
          <strong>Single Assist:</strong> {ppeResult.singleAssistResidents}

          <table
            style={{
              width: '100%',
              borderCollapse: 'collapse',
              marginTop: '12px',
            }}
          >
            <thead>
              <tr>
                {[
                  'Affected %',
                  'Residents',
                  'Gloves',
                  'Gowns',
                  'Aprons',
                  'Surgical Masks',
                  'N95/P2',
                  'Eye Protection',
                ].map((header) => (
                  <th
                    key={header}
                    style={{
                      border: '1px solid #ddd',
                      padding: '8px',
                      backgroundColor: '#e3f2fd',
                    }}
                  >
                    {header}
                  </th>
                ))}
              </tr>
            </thead>

            <tbody>
              {(ppeResult.scenarios || []).map((row) => (
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
  )
}

export default PpeForecastPanel