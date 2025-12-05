using UnityEngine;

public enum UnitTeam
{
    Player,
    Enemy
}

public class Unit : MonoBehaviour
{
    [Header("Unit Identifiers")]
    [SerializeField] private string unitName = "Unit";
    [field: SerializeField] public UnitTeam Team {get; private set;} = UnitTeam.Player;

    [Header("Unit Stats")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [field: SerializeField] public int movementRange {get; private set;} = 3;
    public int MovementRange => movementRange;
    [field: SerializeField] public int AttackRange {get; private set;} = 1;
    [SerializeField] private int baseDamage = 25;

    [Header("Turn management")]
    [field: SerializeField] public int MaxActionPoints {get; private set;} = 2;
    public int CurrentActionPoints {get; private set;}

    public Vector2Int GridPosition { get; private set; }

    private void Start()
    {
        currentHealth = maxHealth;
        CurrentActionPoints = MaxActionPoints;
    }

    public void Initialize(UnitTeam team, string name)
    {
        this.Team =team;
        this.unitName = name;

        if (team == UnitTeam.Enemy)
        {
            GetComponent<Renderer>().material.color = Color.red;
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.blue;
        }

    }

     public void ResetTurn()
    {
        CurrentActionPoints = MaxActionPoints;
        if (Team == UnitTeam.Enemy) GetComponent<Renderer>().material.color = Color.red;
        else GetComponent<Renderer>().material.color = Color.blue;
    }

    public bool CanTakeAction(int cost = 1)
    {
        return CurrentActionPoints >= cost;
    }

    public void SpendActionPoints(int cost)
    {
        CurrentActionPoints -= cost;
        Debug.Log($"{unitName} spent {cost} AP. Remaining: {CurrentActionPoints}");

        if (CurrentActionPoints <= 0)
        {
            GetComponent<Renderer>().material.color = Color.gray;
        }
    }

    public void SetStartupPosition(Vector2Int startPos, HexGrid grid)
    {
        GridPosition = startPos;
        HexTile tile = grid.GetTileAt(startPos);

        if (tile != null)
        {
            tile.OccupyingUnit = this;
            transform.position = tile.WorldPosition;
        }
    }

    public void MoveToTile(Vector2Int newPos, HexGrid grid, bool instant = false, bool logToBackend = true)
    {
        if (!CanTakeAction(1) && !instant)
        {
            Debug.Log("Geen actiepunten meer voor bewegen.");
            return;
        }

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

            Vector3 lookDirection = newTile.WorldPosition - transform.position;
            if (lookDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(lookDirection);
            }

            transform.position = newTile.WorldPosition;

            if (!instant)
            {
                SpendActionPoints(1);
            }

            Debug.Log($"{unitName} moved to {GridPosition}");

            if (logToBackend && GameLogger.Instance != null)
            {
                var movePayload = new { unit = unitName, from = new { x = previousPos.x, y = previousPos.y }, to = new { x = GridPosition.x, y = GridPosition.y } };
                GameLogger.Instance.LogAction("Move", movePayload);
            }
        }
    }

    public void Attack(Unit target)
    {
        if (!CanTakeAction(1))
        {
            Debug.Log("No actionpoints left to attack with.");
            return;
        }


        if (target == null) return;

        transform.LookAt(target.transform);

        float damageMultiplier = 1.0f;
        string attackType = "Frontal";

        Vector3 attackDirection = (target.transform.position - transform.position).normalized;

        Vector3 targetForward = target.transform.forward;

        float dot = Vector3.Dot(attackDirection, targetForward);

        if (dot > 0.5f)
        {
            damageMultiplier = 2.0f;
            attackType = "Backstab";
        }
        else if (dot > -0.5f && dot <= 0.5f)
        {
            damageMultiplier = 1.5f;
            attackType = "Flank";
        }

        int finalDamage = Mathf.RoundToInt(baseDamage * damageMultiplier);

        Debug.Log($"{unitName} attacks {target.unitName} ({attackType}) for {finalDamage} damage!");

        SpendActionPoints(1);
        target.TakeDamage(finalDamage);

        if (GameLogger.Instance != null)
        {
            var attackPayload = new
            {
                attacker = unitName,
                target = target.unitName ?? "Unknown",
                attackType = attackType,
                damageDealt = finalDamage,
                multiplier = damageMultiplier
            };
            GameLogger.Instance.LogAction("Attack", attackPayload);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{unitName} took {damage} damage. HP: {currentHealth}/{maxHealth}");

        if (GameLogger.Instance != null)
        {
            GameLogger.Instance.LogAction("TakeDamage", new { victim = unitName, dmg = damage, hp = currentHealth });
        }

        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        Debug.Log($"{unitName} has died!");
        if (GameLogger.Instance != null) GameLogger.Instance.LogAction("UnitDeath", new { unit = unitName, pos = GridPosition });
        Destroy(gameObject);
    }
}
