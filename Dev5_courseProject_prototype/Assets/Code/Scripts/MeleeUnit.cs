using UnityEngine;

public class MeleeUnit : MonoBehaviour
{
    public int attackDamage = 20;
    public int health = 100;
    public Vector3Int gridPosition;

    public bool IsInMeleeRange(MeleeUnit other)
    {
        Vector3Int diff = other.gridPosition - gridPosition;

        return (Mathf.Abs(diff.x) == 1 && diff.z == 0) ||
               (Mathf.Abs(diff.z) == 1 && diff.x == 0);
    }

    public void Attack(MeleeUnit target)
    {
        target.TakeDamage(attackDamage);
        Debug.Log($"{name} attacked {target.name} for {attackDamage} damage!");
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"{name} took {damage} damage. Remaining health: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{name} has died.");
        Destroy(gameObject);
    }
}
