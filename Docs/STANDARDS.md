# 📝 STANDARDS.md - Game Analytics System

## 1. Architecture: Modular Service Structure

This system follows a **Microservices Architecture** with clear separation between frontend, backend, and database layers.

We organize components by **responsibility** rather than technical concerns.

---

### 1.1 Service Organization

Every service operates independently with defined interfaces:

```
wdm-EphramJanssens/
├── docker-compose.yml        # Service orchestration
├── wdm_Backend/           # Node.js API Service
│   ├── api/
│   │   ├── server.js      # Express application
│   │   └── Dockerfile     # Backend container
│   └── init-db.sql       # Database schema
├── wdm_Admin/             # Vue 3 Frontend Service
│   ├── src/
│   │   └── App.vue        # Single-page application
│   ├── Dockerfile           # Frontend container
│   └── package.json        # Vue 3 dependencies
├── Dev5_courseProject_prototype/  # Unity Game Client
│   └── Assets/Code/Scripts/
│       └── GameLogger.cs      # Game data logging
└── conversation_logs/       # Development logs
```

---

### 1.2 Service Responsibilities

#### A. Backend Service (`wdm_Backend/`)
- **Role**: Data Persistence & API Interface
- **Responsibilities**:
  * HTTP request handling (Express.js)
  * Database operations (PostgreSQL)
  * CORS management
  * Game data validation
- **Rule**: No UI logic, pure API layer

#### B. Frontend Service (`wdm_Admin/`)
- **Role**: User Interface & Data Visualization
- **Responsibilities**:
  * Real-time data display
  * User interaction handling
  * Dashboard rendering
  * Statistical calculations
- **Rule**: No database access, pure presentation layer

#### C. Game Client (`Dev5_courseProject_prototype/`)
- **Role**: Data Generation & User Interaction
- **Responsibilities**:
  * Game event logging
  * User session management
  * HTTP API communication
- **Rule**: No analytics logic, pure data collection

#### D. Database Service (`PostgreSQL`)
- **Role**: Data Storage & Persistence
- **Responsibilities**:
  * Structured data storage
  * Query optimization
  * Data integrity enforcement
- **Rule**: ACID compliance and indexing

---

## 2. Coding Conventions

### 2.1 JavaScript/Node.js Standards

| Entity | Convention | Example |
|--------|------------|---------|
| Variables | camelCase | `userData`, `isValid`, `gameLogs` |
| Functions | camelCase | `getUserLogs()`, `validateAction()`, `formatDateTime()` |
| Constants | UPPER_SNAKE_CASE | `API_URL`, `DB_HOST`, `MAX_LOGS_LIMIT` |
| Booleans | Prefix with `is`, `has`, `should` | `isConnected`, `hasPermission`, `shouldRefresh` |
| Classes | PascalCase | `GameLogger`, `AnalyticsService` |
| Files | kebab-case | `game-logger.js`, `user-service.js` |

### 2.2 Vue.js Standards

| Entity | Convention | Example |
|--------|------------|---------|
| Components | PascalCase | `GameDashboard.vue`, `UserDetailView.vue` |
| Props | camelCase | `userId`, `gameLogs`, `refreshInterval` |
| Events | kebab-case | `@user-selected`, `@data-refresh` |
| Reactive State | camelCase | `selectedUserId`, `isConnected`, `loadingState` |
| Methods | camelCase | `selectUser()`, `fetchLogs()`, `togglePayload()` |

### 2.3 C# Unity Standards

| Entity | Convention | Example |
|--------|------------|---------|
| Classes | PascalCase | `GameLogger`, `PlayerController`, `AttackSystem` |
| Methods | PascalCase | `LogAction()`, `InitializeUserId()`, `PostLogRoutine()` |
| Variables | camelCase | `currentUserId`, `serverUrl`, `enableLogging` |
| Properties | PascalCase | `Instance`, `EnableLogging`, `ServerUrl` |
| Constants | UPPER_SNAKE_CASE | `DEFAULT_SERVER_URL`, `PLAYER_PREFS_UID_KEY` |

### 2.4 Configuration

* **No Secrets in Code**: Database credentials, API keys, and server URLs must be loaded from Environment Variables (`.env` file).
* **Environment Configuration**: Use `.env.template` as starting point, create local `.env` for actual values.
* **Service Discovery**: Use environment variables for service-to-service communication (`DB_HOST=db`, `FRONTEND_URL=http://localhost:5173`).

---

## 3. Error Handling

### 3.1 Backend Services
* **Structured Error Objects**: Create error classes with consistent structure.
* **HTTP Status Codes**: Map business logic errors to appropriate HTTP codes.
  * 400 Bad Request (Validation)
  * 401 Unauthorized (Login failed)
  * 404 Not Found
  * 500 Internal Server Error
* **Logging Strategy**: Log errors with context but never expose stack traces to clients in production.

### 3.2 Frontend Services
* **User-Friendly Messages**: Translate technical errors to understandable user messages.
* **Loading States**: Provide visual feedback during async operations.
* **Fallback Content**: Show meaningful content when data is unavailable.
* **Network Error Recovery**: Implement retry logic with exponential backoff.

### 3.3 C# Unity Services
* **Graceful Degradation**: Disable logging if server unavailable.
* **Local Caching**: Queue failed requests for retry.
* **PlayerPrefs Management**: Persist critical user data locally.
* **Async Operations**: Use coroutines for network requests to avoid blocking.

---

## 4. Documentation (JSDoc)

All services and shared utilities must be documented using JSDoc standards.

### 4.1 Required Tags
* `@param` for every function argument (include type).
* `@returns` for function output (include type).
* `@throws` if function explicitly raises an error.
* `@example` for usage examples.
* `@deprecated` for obsolete methods.

### 4.2 Examples

```javascript
/**
 * Validates and saves a game log entry to the database.
 * 
 * @param {string} userId - Unique identifier for the user
 * @param {string} actionType - Type of game action performed
 * @param {Object} payload - Additional action-specific data
 * @returns {Promise<Object>} Saved log entry with database-generated ID
 * @throws {ValidationError} If required fields are missing or invalid
 * @throws {DatabaseError} If database operation fails
 * 
 * @example
 * // Log a player movement action
 * await createGameLog('user-123', 'Move', {
 *   from: {x: 5, y: 3},
 *   to: {x: 7, y: 3}
 * });
 */
const createGameLog = async (userId, actionType, payload) => { ... };
```

---

## 5. Testing Standards

### 5.1 Unit Testing
* **Test Naming**: `describe` feature, `test` specific behavior.
* **Arrange-Act-Assert**: Structure tests with clear setup, execution, and verification phases.
* **Mock Dependencies**: Use dependency injection for reliable, fast tests.

### 5.2 Integration Testing
* **API Testing**: Test all endpoints with various payloads.
* **Database Testing**: Test with actual database fixtures.
* **End-to-End Testing**: Test complete user flows from game to dashboard.

---

## 6. Git & Version Control

### 6.1 Branch Strategy
* **main**: Production-ready code.
* **develop**: Integration branch for features.
* **feature/***: Individual feature development.

### 6.2 Commit Messages

Follow **Conventional Commits** specification:

```
<type>[optional scope]: <description>

[optional body]

[optional footer(s)]
```

**Types**: `feat`, `fix`, `docs`, `style`, `refactor`, `test`, `chore`

**Examples**:
```
feat(api): add user statistics endpoint
fix(frontend): resolve memory leak in data refresh
docs(readme): update installation instructions
refactor(backend): extract database logic to service layer
```

### 6.3 Development Workflow
* **Feature Branches**: `git checkout -b feature/amazing-feature`
* **Regular Commits**: Small, focused commits with clear messages.
* **Pull Requests**: Code review required before merging.
* **Release Tags**: Semantic versioning for releases.

---

## 7. Security Standards

### 7.1 Input Validation
* **Sanitize All Input**: Never trust client-side data.
* **Parameter Validation**: Validate types, ranges, and formats.
* **SQL Injection Prevention**: Use parameterized queries exclusively.

### 7.2 Authentication & Authorization
* **CORS Configuration**: Explicit whitelist for allowed origins.
* **Rate Limiting**: Implement per-user and per-IP limits.
* **Secrets Management**: Never commit secrets to version control.

### 7.3 Data Protection
* **Privacy by Design**: Collect only necessary data for stated purposes.
* **Data Retention**: Implement automated cleanup policies.
* **Encryption**: Sensitive data encryption at rest and in transit.

---

## 8. Performance Standards

### 8.1 Database Optimization
* **Query Efficiency**: Use appropriate indexes for common queries.
* **Connection Pooling**: Reuse database connections effectively.
* **Batch Operations**: Group multiple operations where possible.

### 8.2 Frontend Optimization
* **Virtual Scrolling**: Large datasets should use virtualization.
* **Memoization**: Cache expensive computations.
* **Bundle Size**: Optimize for fast initial loads.

### 8.3 API Performance
* **Response Caching**: Cache frequently accessed data.
* **Compression**: Enable gzip compression for API responses.
* **Pagination**: Limit large result sets with proper pagination.

---

## 9. Deployment Standards

### 9.1 Environment Configuration
* **Development**: Local development with hot reload.
* **Staging**: Production-like environment for testing.
* **Production**: Optimized, secure, monitored deployment.

### 9.2 Containerization
* **Dockerfiles**: Multi-stage builds for efficiency.
* **Health Checks**: Implement readiness probes.
* **Resource Limits**: Define memory and CPU constraints.

### 9.3 Monitoring
* **Application Logging**: Structured logs with log levels.
* **Performance Metrics**: Monitor response times and resource usage.
* **Error Tracking**: Aggregate and alert on error rates.

---

## 10. Code Quality Tools

### 10.1 Linting
* **JavaScript/Node.js**: ESLint with consistent configuration.
* **Vue.js**: ESLint Vue plugin for template linting.
* **C# Unity**: StyleCop for consistent code style.

### 10.2 Formatting
* **Prettier**: Automatic code formatting for consistency.
* **EditorConfig**: Unified editor configuration across team.
* **Pre-commit Hooks**: Ensure code quality before commits.

---

## 11. API Standards

### 11.1 RESTful Design
* **HTTP Methods**: Use appropriate verbs (GET, POST, PUT, DELETE).
* **Status Codes**: Consistent error handling across endpoints.
* **Response Format**: Standardized JSON structure for all responses.

### 11.2 Endpoints Documentation
```http
POST /api/logs
Content-Type: application/json

Request:
{
  "userId": "uuid-string",
  "actionType": "PlayerMove", 
  "payload": {"position": "10.5,3.2,0.0"}
}

Response:
{
  "id": 123,
  "userId": "uuid-string",
  "actionType": "PlayerMove",
  "payload": {...},
  "created_at": "2025-01-02T10:30:00.000Z"
}
```

### 11.3 Error Responses
```json
{
  "error": "Validation failed",
  "message": "Missing required field: userId and actionType",
  "code": 400,
  "timestamp": "2025-01-02T10:30:00.000Z"
}
```

---

## 12. Database Standards

### 12.1 Schema Design
* **Primary Keys**: UUID or auto-increment integers for uniqueness.
* **Indexes**: Create indexes on frequently queried columns.
* **Constraints**: Foreign keys and NOT NULL constraints where appropriate.

### 12.2 Query Standards
* **Prepared Statements**: Always use parameterized queries.
* **Transaction Management**: Use transactions for multi-step operations.
* **Connection Management**: Properly close database connections.

---

## 13. Vue.js Component Standards

### 13.1 Component Structure
```vue
<template>
  <!-- Semantic HTML with Bootstrap classes -->
</template>

<script setup>
// Composition API with <script setup>
import { ref, computed, onMounted } from 'vue'
</script>

<style scoped>
/* Component-specific styles */
</style>
```

### 13.2 State Management
* **Local State**: Use `ref` and `reactive` for component state.
* **Computed Properties**: Use `computed` for derived values.
* **Props/Emits**: Use `defineProps` and `defineEmits` for component interfaces.

---

These standards ensure maintainable, secure, and performant code across all services in the Game Analytics system.