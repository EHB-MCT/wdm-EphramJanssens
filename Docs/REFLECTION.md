# 💭 Project Reflection: WDM Game Analytics System
## 1. Project Outcomes
The primary goal of this project was to build a full-stack telemetry system. I successfully established the fundamental infrastructure required for tracking, storing, and visualizing gameplay data.

* **End-to-End Pipeline:** Created a functioning data pipeline connecting a Unity Game Client (C#) to a Node.js Backend and a PostgreSQL database.
* **Data Persistence:** Implemented a robust database schema capable of storing user actions, timestamps, and payload data using Docker containers.
* **Visualization:** Developed a Vue.js Dashboard that successfully retrieves and displays user-specific data logs.
* **Prototype Integration:** The Unity prototype actively sends real-time data via HTTP POST requests, proving the concept works technically.

## 2. Shortcomings & Challenges
While the architectural groundwork is solid, the project is currently a Minimum Viable Product (MVP) and has not yet reached its full analytical potential.

* **Data Granularity:** Although the system tracks basic actions, there is currently insufficient data volume and complexity to perform meaningful predictive analytics or deep behavioral profiling.
* **Backend Complexity:** The learning curve for backend development (Node.js/Express) combined with strict time constraints limited the scope of features.
* **Scope vs. Time:** The focus shifted heavily towards infrastructure stability (Docker/Connectivity), which left less time for implementing advanced statistical algorithms and "win/loss" logic.

## 3. Key Insights & Personal Growth
This project served as a significant learning milestone, particularly in professional software development practices.

* **Code Standards & Git Flow:** This was the first time I rigorously applied industry-standard code conventions and structured Git workflows (feature branches, commit message standards). I now understand their value in maintaining project clarity.
* **Polyglot Development:** Switching context between C# (Unity) and JavaScript (Backend/Frontend) was initially challenging. However, bridging these two worlds gave me valuable insight into full-stack development and API design.
* **Infrastructure as Code:** Learning to orchestrate services with Docker Compose has changed how I view project setup and deployment.