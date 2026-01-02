<template>
  <div id="app" class="container-fluid">
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
                    <td>{{ log.user_id }}</td>
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
</template>

<script>
import { ref, computed, onMounted, onUnmounted } from 'vue'

export default {
  name: 'App',
  setup() {
    const logs = ref([])
    const loading = ref(false)
    const error = ref('')
    const isConnected = ref(false)
    const showPayload = ref({})
    const lastUpdate = ref('')
    
    let intervalId = null
    
    const API_URL = 'http://localhost:3000/api/logs'
    
    const uniqueUsers = computed(() => {
      const users = new Set(logs.value.map(log => log.user_id))
      return users.size
    })
    
    const uniqueActions = computed(() => {
      const actions = new Set(logs.value.map(log => log.action_type))
      return actions.size
    })
    
    const formatDateTime = (dateString) => {
      const date = new Date(dateString)
      return date.toLocaleString()
    }
    
    const togglePayload = (logId) => {
      showPayload.value[logId] = !showPayload.value[logId]
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
      }, 5000) // Refresh every 5 seconds
    }
    
    const stopAutoRefresh = () => {
      if (intervalId) {
        clearInterval(intervalId)
        intervalId = null
      }
    }
    
    onMounted(() => {
      fetchLogs()
      startAutoRefresh()
    })
    
    onUnmounted(() => {
      stopAutoRefresh()
    })
    
    return {
      logs,
      loading,
      error,
      isConnected,
      showPayload,
      lastUpdate,
      uniqueUsers,
      uniqueActions,
      formatDateTime,
      togglePayload,
      refreshData
    }
  }
}
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
</style>