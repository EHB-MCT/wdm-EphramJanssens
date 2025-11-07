const express = require('express');
const { Pool } = require('pg');
const cors = require('cors');

const app = express();
const port = process.env.PORT || 3000;

// --- Middleware ---
app.use(cors()); // Sta Cross-Origin requests toe (voor Unity)
app.use(express.json()); // Parse binnenkomende JSON-data

// --- Database Connectie ---
const pool = new Pool({
    user: process.env.DB_USER,
    // BELANGRIJK: 'db' is de servicenaam in docker-compose.yml
    host: process.env.DB_HOST, 
    database: process.env.DB_NAME,
    password: process.env.DB_PASSWORD,
    port: 5432,
});

// --- Endpoints ---
app.get('/', (req, res) => {
    res.send('Backend API draait en is klaar om data te ontvangen!');
});

// Dit is het endpoint waar je Unity-game naartoe stuurt
app.post('/api/log', async (req, res) => {
    const { userId, actionType, payload } = req.body;

    // Requirement: "Data is cleaned, or otherwise checked"
    if (!userId || !actionType) {
        return res.status(400).send({ error: 'Missing required fields: userId and actionType' });
    }

    console.log(`Data ontvangen van user ${userId}: ${actionType}`);

    try {
        // Sla de data op in de database
        const query = 'INSERT INTO game_logs (user_id, action_type, payload) VALUES ($1, $2, $3) RETURNING *';
        const values = [userId, actionType, payload || {}]; // Stuur {} als payload null is

        const result = await pool.query(query, values);
        res.status(201).send(result.rows[0]); // Stuur de gemaakte log terug

    } catch (err) {
        console.error('Error met database:', err);
        res.status(500).send({ error: 'Database insertion error' });
    }
});

// --- Start Server ---
app.listen(port, () => {
    console.log(`API server luistert op http://localhost:${port}`);
});