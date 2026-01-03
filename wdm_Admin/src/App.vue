<template>
  <div id="app" class="container-fluid">
    <!-- Main Dashboard View -->
    <div v-if="!selectedUserId">
      <div class="row">
        <div class="col-12">
          <h1 class="text-center my-4">Game Analytics Dashboard</h1>
        </div>
      </div>
      
      <div class="row">
        <div class="col-12">
          <div class="card">
            <div class="card-header d-flex justify-content-between align-items-center">
              <h5 class="mb-0">Game Logs</h5>
              <div>
                <span class="badge bg-success" v-if="isConnected">Connected</span>
                <span class="badge bg-danger" v-else>Disconnected</span>
                <button class="btn btn-sm btn-primary ms-2" @click="refreshData">Refresh</button>
              </div>
            </div>
            <div class="card-body">
              <div v-if="loading" class="text-center">
                <div class="spinner-border" role="status">
                  <span class="visually-hidden">Loading...</span>
                </div>
              </div>
              
              <div v-else-if="error" class="alert alert-danger">
                {{ error }}
              </div>
              
              <div v-else-if="logs.length === 0" class="text-center text-muted">
                No logs available
              </div>
              
              <div v-else class="table-responsive">
                <table class="table table-striped table-hover">
                  <thead class="table-dark">
                    <tr>
                      <th>ID</th>
                      <th>User ID</th>
                      <th>Action Type</th>
                      <th>Payload</th>
                      <th>Created At</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="log in logs" :key="log.id">
                      <td>{{ log.id }}</td>
                      <td 
                        @click="selectUser(log.user_id)" 
                        class="user-id-cell"
                        title="Click to view user details"
                      >
                        {{ log.user_id }}
                      </td>
                      <td>
                        <span class="badge bg-info">{{ log.action_type }}</span>
                      </td>
                      <td>
                        <button 
                          class="btn btn-sm btn-outline-secondary" 
                          @click="togglePayload(log.id)"
                          data-bs-toggle="tooltip" 
                          title="Click to view payload"
                        >
                          {{ showPayload[log.id] ? 'Hide' : 'Show' }} Payload
                        </button>
                        <div v-if="showPayload[log.id]" class="mt-2">
                          <pre class="bg-light p-2 rounded">{{ JSON.stringify(log.payload, null, 2) }}</pre>
                        </div>
                      </td>
                      <td>{{ formatDateTime(log.created_at) }}</td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        </div>
      </div>
      
      <div class="row mt-3">
        <div class="col-12">
          <div class="card">
            <div class="card-header">
              <h6 class="mb-0">Statistics</h6>
            </div>
            <div class="card-body">
              <div class="row">
                <div class="col-md-3">
                  <div class="text-center">
                    <h4 class="text-primary">{{ logs.length }}</h4>
                    <small class="text-muted">Total Logs</small>
                  </div>
                </div>
                <div class="col-md-3">
                  <div class="text-center">
                    <h4 class="text-success">{{ uniqueUsers }}</h4>
                    <small class="text-muted">Unique Users</small>
                  </div>
                </div>
                <div class="col-md-3">
                  <div class="text-center">
                    <h4 class="text-info">{{ uniqueActions }}</h4>
                    <small class="text-muted">Action Types</small>
                  </div>
                </div>
                <div class="col-md-3">
                  <div class="text-center">
                    <h4 class="text-warning">{{ lastUpdate }}</h4>
                    <small class="text-muted">Last Update</small>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- User Detail View -->
    <div v-else>
      <div class="row">
        <div class="col-12">
          <div class="d-flex justify-content-between align-items-center mb-4">
            <h1 class="mb-0">User Details: {{ selectedUserId }}</h1>
            <button class="btn btn-outline-primary" @click="goBack">
              ← Back to Dashboard
            </button>
          </div>
        </div>
      </div>

      <div class="row">
        <div class="col-md-6">
          <div class="card">
            <div class="card-header">
              <h6 class="mb-0">User Statistics</h6>
            </div>
            <div class="card-body">
              <div class="row">
                <div class="col-6">
                  <div class="text-center mb-3">
                    <h4 class="text-primary">{{ userLogs.length }}</h4>
                    <small class="text-muted">Total Logs</small>
                  </div>
                </div>
                <div class="col-6">
                  <div class="text-center mb-3">
                    <h4 class="text-success">{{ favoriteAction }}</h4>
                    <small class="text-muted">Favorite Action</small>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="col-md-6">
          <div class="card">
            <div class="card-header">
              <h6 class="mb-0">Connection Status</h6>
            </div>
            <div class="card-body">
              <div class="text-center">
                <span class="badge bg-success" v-if="isConnected">Connected</span>
                <span class="badge bg-danger" v-else>Disconnected</span>
                <div class="mt-2">
                  <small class="text-muted">Last Update: {{ lastUpdate }}</small>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="row mt-3">
        <div class="col-12">
          <div class="card">
            <div class="card-header d-flex justify-content-between align-items-center">
              <h6 class="mb-0">User Actions</h6>
              <button class="btn btn-sm btn-primary" @click="refreshData">Refresh</button>
            </div>
            <div class="card-body">
              <div v-if="userLogs.length === 0" class="text-center text-muted">
                No actions found for this user
              </div>
              
              <div v-else class="table-responsive">
                <table class="table table-striped table-hover">
                  <thead class="table-dark">
                    <tr>
                      <th>ID</th>
                      <th>Action Type</th>
                      <th>Payload</th>
                      <th>Created At</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="log in userLogs" :key="log.id">
                      <td>{{ log.id }}</td>
                      <td>
                        <span class="badge bg-info">{{ log.action_type }}</span>
                      </td>
                      <td>
                        <button 
                          class="btn btn-sm btn-outline-secondary" 
                          @click="togglePayload(log.id)"
                          data-bs-toggle="tooltip" 
                          title="Click to view payload"
                        >
                          {{ showPayload[log.id] ? 'Hide' : 'Show' }} Payload
                        </button>
                        <div v-if="showPayload[log.id]" class="mt-2">
                          <pre class="bg-light p-2 rounded">{{ JSON.stringify(log.payload, null, 2) }}</pre>
                        </div>
                      </td>
                      <td>{{ formatDateTime(log.created_at) }}</td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue'

// Reactive state
const logs = ref([])
const loading = ref(false)
const error = ref('')
const isConnected = ref(false)
const showPayload = ref({})
const lastUpdate = ref('')
const selectedUserId = ref(null)

let intervalId = null

const API_URL = 'http://localhost:3000/api/logs'

// Computed properties for dashboard
const uniqueUsers = computed(() => {
  const users = new Set(logs.value.map(log => log.user_id))
  return users.size
})

const uniqueActions = computed(() => {
  const actions = new Set(logs.value.map(log => log.action_type))
  return actions.size
})

// Computed properties for user detail view
const userLogs = computed(() => {
  return logs.value.filter(log => log.user_id === selectedUserId.value)
})

const favoriteAction = computed(() => {
  const actionCounts = {}
  userLogs.value.forEach(log => {
    actionCounts[log.action_type] = (actionCounts[log.action_type] || 0) + 1
  })
  
  const entries = Object.entries(actionCounts)
  if (entries.length === 0) return 'No actions'
  
  return entries.sort((a, b) => b[1] - a[1])[0][0]
})

// Methods
const formatDateTime = (dateString) => {
  const date = new Date(dateString)
  return date.toLocaleString()
}

const togglePayload = (logId) => {
  showPayload.value[logId] = !showPayload.value[logId]
}

const selectUser = (userId) => {
  selectedUserId.value = userId
}

const goBack = () => {
  selectedUserId.value = null
}

const fetchLogs = async () => {
  try {
    loading.value = true
    error.value = ''
    
    const response = await fetch(API_URL)
    
    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`)
    }
    
    const data = await response.json()
    logs.value = data
    isConnected.value = true
    lastUpdate.value = new Date().toLocaleTimeString()
    
  } catch (err) {
    console.error('Error fetching logs:', err)
    error.value = `Failed to fetch logs: ${err.message}`
    isConnected.value = false
  } finally {
    loading.value = false
  }
}

const refreshData = () => {
  fetchLogs()
}

const startAutoRefresh = () => {
  intervalId = setInterval(() => {
    fetchLogs()
  }, 300000) // Refresh every 5 minutes (300000ms)
}

const stopAutoRefresh = () => {
  if (intervalId) {
    clearInterval(intervalId)
    intervalId = null
  }
}

// Lifecycle
onMounted(() => {
  fetchLogs()
  startAutoRefresh()
})

onUnmounted(() => {
  stopAutoRefresh()
})
</script>

<style>
#app {
  min-height: 100vh;
  background-color: #f8f9fa;
}

.card {
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.table th {
  border-top: none;
}

pre {
  font-size: 0.8rem;
  max-height: 200px;
  overflow-y: auto;
}

.badge {
  font-size: 0.8rem;
}

.user-id-cell {
  cursor: pointer;
  color: #007bff;
  text-decoration: underline;
  transition: color 0.2s ease;
}

.user-id-cell:hover {
  color: #0056b3;
}

.user-id-cell:active {
  color: #004085;
}
</style>