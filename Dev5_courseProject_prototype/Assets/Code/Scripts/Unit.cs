using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Unit : MonoBehaviour
{
    [Header("Unit stats")]
    [SerializeField] private string unitName = "Nameless unit";
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private int movementRange = 3;
    public int MovementRange => movementRange;
    [SerializeField] private int attackDamage = 25;

    public Vector2Int GridPosition { get; private set; }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void SetStartupPosition(Vector2Int startPos, HexGrid grid)
    {
        MoveToTile(startPos, grid, true);
    }

    public void MoveToTile(Vector2Int newPos, HexGrid grid, bool instant = false, bool logToBackend = true)
    {
        HexTile oldTile = grid.GetTileAt(GridPosition);
        if (oldTile != null && oldTile.OccupyingUnit == this)
        {
            oldTile.OccupyingUnit = null;
        }

        Vector2Int previousPos = GridPosition;

        GridPosition = newPos;
        HexTile newTile = grid.GetTileAt(newPos);

        if (newTile != null)
        {
            newTile.OccupyingUnit = this;

            if (instant)
            {
                transform.position = newTile.WorldPosition;
            }
            else
            {
                transform.position = newTile.WorldPosition;
            }
            Debug.Log($"{unitName} moved to {GridPosition}");

            if (logToBackend && GameLogger.Instance != null)
            {
                var movePayload = new
                {
                    unit = unitName,
                    from = new { z = previousPos.x, y = previousPos.y},
                    to = new { x = GridPosition.x, y = GridPosition.y }
                };
                GameLogger.Instance.LogAction("Move", movePayload);
            }
        }
    }

    public void Attack(Unit target)
    {
        if (target == null) return;
        Debug.Log($"{unitName} attacks {target.name}.");

        target.TakeDamage(attackDamage);

        if (GameLogger.Instance != null)
        {
            var attackPayload = new
            {
                attacker = unitName,
                target = target.name,
                damageDealt = attackDamage,
                weapon = "sword"
            };
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{unitName} took {damage} damage. HP: {currentHealth}/{maxHealth}");

        if (GameLogger.Instance != null)
        {
            var damagePayload = new
            {
                victim = unitName,
                damageReceived = damage,
                remainingHp = currentHealth
            };
            GameLogger.Instance.LogAction("TakeDamage", damagePayload);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    private void Die()
    {
        Debug.Log($"{unitName} has died!");
        
        if (GameLogger.Instance != null)
        {
            GameLogger.Instance.LogAction("UnitDeath", new {unit = unitName, position = GridPosition});
        }

        Destroy(gameObject);
    }
}
