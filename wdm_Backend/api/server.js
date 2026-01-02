const express = require('express');
const { Pool } = require('pg');
const cors = require('cors');

const app = express();
const port = process.env.PORT || 3000;

app.use(cors()); 
app.use(express.json()); 


const pool = new Pool({
    user: process.env.DB_USER,
    host: process.env.DB_HOST, 
    database: process.env.DB_NAME,
    password: process.env.DB_PASSWORD,
    port: 5432,
});


app.get('/', (req, res) => {
    res.send('Backend API draait en is klaar om data te ontvangen!');
});


app.post('/api/log', async (req, res) => {
    const { userId, actionType, payload } = req.body;


    if (!userId || !actionType) {
        return res.status(400).send({ error: 'Missing required fields: userId and actionType' });
    }

    console.log(`Data ontvangen van user ${userId}: ${actionType}`);

    try {
        const query = 'INSERT INTO game_logs (user_id, action_type, payload) VALUES ($1, $2, $3) RETURNING *';
        const values = [userId, actionType, payload || {}];

        const result = await pool.query(query, values);
        res.status(201).send(result.rows[0]);

    } catch (err) {
        console.error('Error met database:', err);
        res.status(500).send({ error: 'Database insertion error' });
    }
});

app.get ("/api/logs", async (req, res) => {
    try{
        const result = await pool.query("SELECT * FROM game_logs ORDER BY created_at DESC LIMIT 100");
        res.json(result.rows);
    } catch (err) {
        console.error(err);
        res.status(500).json({error: "Database error when fetching logs"});
    }
});

app.listen(port, () => {
    console.log(`API server luistert op http://localhost:${port}`);
});