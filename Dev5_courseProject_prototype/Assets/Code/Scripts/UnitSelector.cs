using UnityEngine;

public class UnitSelector : MonoBehaviour
{
  private MeleeUnit selectedUnit;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                MeleeUnit unit = hit.collider.GetComponent<MeleeUnit>();
                if (unit != null)
                {
                    selectedUnit = unit;
                    Debug.Log($"Selected {unit.name}");
                }
            }
        }

        if (Input.GetMouseButtonDown(1) && selectedUnit != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                MeleeUnit target = hit.collider.GetComponent<MeleeUnit>();
                if (target != null && target != selectedUnit)
                {
                    if (selectedUnit.IsInMeleeRange(target))
                    {
                        selectedUnit.Attack(target);
                    }
                    else
                    {
                        Debug.Log("Target is out of melee range!");
                    }
                }
            }
        }
    }
}
