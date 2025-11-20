using UnityEditor;
using UnityEngine;

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

    public void MoveToTile(Vector2Int newPos, HexGrid grid, bool instant = false)
    {
        HexTile oldTile = grid.GetTileAt(GridPosition);
        if (oldTile != null && oldTile.OccupyingUnit == this)
        {
            oldTile.OccupyingUnit = null;
        }

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
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{unitName} took {damage} damage! HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    private void Die()
    {
        Debug.Log($"{unitName} has died!");
        Destroy(gameObject);
    }
}
