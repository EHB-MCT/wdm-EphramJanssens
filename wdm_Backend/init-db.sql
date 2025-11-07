-- Maak de tabel aan als deze nog niet bestaat
CREATE TABLE IF NOT EXISTS game_logs (
    id SERIAL PRIMARY KEY,
    user_id VARCHAR(255) NOT NULL,
    action_type VARCHAR(100) NOT NULL,
    
    -- JSONB is de reden waarom we Postgres gebruiken.
    -- Hiermee kun je flexibele data (clicks, moves, etc.)
    -- in één kolom opslaan.
    payload JSONB,
    
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);