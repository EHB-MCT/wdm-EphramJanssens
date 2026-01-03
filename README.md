# Portfolio
# 🎮 Game Analytics System

> **Real-time analytics platform for Unity games with behavioral tracking and user profiling.**

This project offers a full-stack solution for collecting, storing, and visualizing gameplay data. The system connects a Unity game with a Node.js backend and a Vue 3 frontend dashboard to provide insight into player behavior through detailed logs and statistics.

---

## 🏗️ Technology Stack

| Component | Technologie |
| :--- | :--- |
| **Frontend** | Vue 3 + Vite + Bootstrap 5 |
| **Backend** | Node.js + Express + PostgreSQL |
| **Game Engine** | Unity 6 (C# HTTP API logging) |
| **Deployment** | Docker + Docker Compose |
| **Database** | PostgreSQL |

---

## ✨ Core features

### 📊 Live Dashboard
- **Real-time visualisation:** Direct insight into incoming game data.
- **Auto-refresh:** Dashboard automatically refreshes every 5 seconds to stay up-to-date.
- **Payload Viewer:** View complex JSON data (such as combat stats or positions) in a human-readable format.

### 👤 User Profiling
- **Unique Identification:** Tracking based on GUIDs that are persistent across sessions.
- **User Detail Views:** Clickable user profiles with specific statistics.
- **Behavioral Insights:** Analyze favorite actions (Move vs Attack) and session duration.
- **Action History:** Complete timeline of actions per specific user.

### 🔗 Integration & Architecture
- **Dockerized:** Full stack (DB, Backend, Frontend) with 1 command.
- **RESTful API:** Robust endpoints for receiving (POST) and retrieving (GET) logs.
- **CORS-enabled:** Secure communication between frontend, backend and game client.

## 📸 Screenshots

| **Live Dashboard** | **User Detail View** |
|:---:|:---:|
| ![Dashboard Overview](docs/Screenshots/Dashboard.png) | ![User Details](docs/Screenshots/UserDetails.png) |
| *Real-time of all logs* | *Detailled statistics per player* |

---

## 🚀 Quick Start

### Requirements
* [Docker Desktop](https://www.docker.com/products/docker-desktop/)
* [Git](https://git-scm.com/)
* Unity Editor (recommended 6000.0.64f1)

### Installation

**1. Clone repository**
```bash
git clone <repository-url>
cd wdm-EphramJanssens
```

**2. Unity Client**
1. Unzip the project inside of the wdm-EphramJanssens repo.
2. Open the project in Unity 6 (recommended 6000.0.64f1).
3. Open the scene `Assets>Level>Scenes>PrototypeHexagonTiles`.
4. Select the **GameManager** object in the Hierarchy.
5. Find the **GameLogger (Script)** component in the Inspector.
6. Check that the **Server URL** is set to:
http://localhost:3000/api/logs
7. Make sure the **Enable Logging** checkbox is ON.

**3. Environment setup**
# Make a .env following the .env.template

**4. Start services**
docker-compose up --build

**API Endpoints**
- `POST /api/logs` - Receive game data from Unity
- `GET /api/logs `- Get latest 100 logs
- `GET /` - Status check

**Project structure**
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