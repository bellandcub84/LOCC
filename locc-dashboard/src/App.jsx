import { useEffect, useState } from 'react'

import DashboardShell from './components/layout/DashboardShell'
import TopCommandBar from './components/layout/TopCommandBar'
import OperationalEpidemiologyPanel from './components/panels/OperationalEpidemiologyPanel'
import SurveillancePanel from './components/panels/SurveillancePanel'
import EnvironmentalZonesPanel from './components/panels/EnvironmentalZonesPanel'
import PpeForecastPanel from './components/panels/PpeForecastPanel'
import OperationalTasksPanel from './components/panels/OperationalTasksPanel'

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000'

const fetchJson = async (endpoint, fallback) => {
  try {
    const response = await fetch(`${API_BASE_URL}${endpoint}`)

    if (!response.ok) {
      console.error(`${endpoint} failed with status ${response.status}`)
      return fallback
    }

    const contentType = response.headers.get('content-type') || ''

    if (!contentType.includes('application/json')) {
      console.error(`${endpoint} returned non-JSON content`)
      return fallback
    }

    return await response.json()
  } catch (err) {
    console.error(`${endpoint} request failed`, err)
    return fallback
  }
}

function App() {
  const [tasks, setTasks] = useState([])
  const [summary, setSummary] = useState(null)
  const [rooms, setRooms] = useState([])
  const [resources, setResources] = useState([])
  const [zones, setZones] = useState([])
  const [surveillanceCases, setSurveillanceCases] = useState([])
  const [epidemiology, setEpidemiology] = useState(null)

  const [surveillanceSearch, setSurveillanceSearch] = useState('')
  const [collapsedAreas, setCollapsedAreas] = useState({})
  const [expandedRationale, setExpandedRationale] = useState({})

  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    let isMounted = true

    const loadDashboardData = async () => {
      try {
        setLoading(true)
        setError('')

        const [
          tasksData,
          summaryData,
          roomsData,
          resourcesData,
          zonesData,
          surveillanceData,
          epidemiologyData,
        ] = await Promise.all([
          fetchJson('/api/tasks', []),
          fetchJson('/api/outbreak-summary', null),
          fetchJson('/api/rooms', []),
          fetchJson('/api/resources', []),
          fetchJson('/api/zones', []),
          fetchJson('/api/surveillance', []),
          fetchJson('/api/epidemiology/summary', null),
        ])

        if (!isMounted) return

        setTasks(Array.isArray(tasksData) ? tasksData : [])
        setSummary(summaryData)
        setRooms(Array.isArray(roomsData) ? roomsData : [])
        setResources(Array.isArray(resourcesData) ? resourcesData : [])
        setZones(Array.isArray(zonesData) ? zonesData : [])
        setSurveillanceCases(Array.isArray(surveillanceData) ? surveillanceData : [])
        setEpidemiology(epidemiologyData)
      } catch (err) {
        if (isMounted) {
          console.error('Dashboard load failed:', err)
          setError(err.message || 'Dashboard data could not be loaded.')
        }
      } finally {
        if (isMounted) {
          setLoading(false)
        }
      }
    }

    loadDashboardData()

    return () => {
      isMounted = false
    }
  }, [])

  const handleStatusUpdated = (updatedTask) => {
    setTasks((prevTasks) =>
      prevTasks.map((task) =>
        task.taskId === updatedTask.taskId ? updatedTask : task
      )
    )
  }

  const toggleArea = (area) => {
    setCollapsedAreas((prev) => ({
      ...prev,
      [area]: !prev[area],
    }))
  }

  const toggleRationale = (taskId) => {
    setExpandedRationale((prev) => ({
      ...prev,
      [taskId]: !prev[taskId],
    }))
  }

  const updateZone = (facilityRoomId, updates) => {
    setZones((prevZones) =>
      prevZones.map((zone) =>
        zone.facilityRoomId === facilityRoomId
          ? { ...zone, ...updates }
          : zone
      )
    )
  }

  return (
    <DashboardShell>
      <h1>LOCC Incident Controller Dashboard</h1>

      {loading && <p>Loading dashboard data from LOCC API...</p>}

      {error && (
        <div style={{ padding: 12, border: '1px solid red', color: 'red' }}>
          API connection error: {error}
        </div>
      )}

{!loading && !error && (
  <>
    <TopCommandBar summary={summary ?? null} />

    {epidemiology && (
      <OperationalEpidemiologyPanel epidemiology={epidemiology} />
    )}

    <SurveillancePanel
      surveillanceCases={Array.isArray(surveillanceCases) ? surveillanceCases : []}
      surveillanceSearch={surveillanceSearch}
      setSurveillanceSearch={setSurveillanceSearch}
    />

    <PpeForecastPanel />

    <EnvironmentalZonesPanel
      rooms={Array.isArray(rooms) ? rooms : []}
      zones={Array.isArray(zones) ? zones : []}
      updateZone={updateZone}
    />

    <OperationalTasksPanel
      tasks={Array.isArray(tasks) ? tasks : []}
      collapsedAreas={collapsedAreas}
      toggleArea={toggleArea}
      expandedRationale={expandedRationale}
      toggleRationale={toggleRationale}
      onStatusUpdated={handleStatusUpdated}
    />
  </>
)}

    </DashboardShell>
  )
}

export default App