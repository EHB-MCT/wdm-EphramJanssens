using UnityEngine;

public class GameInteraction : MonoBehaviour
{
    [Header("Debug")]
    public GameObject debugMarkerPrefab;
    [Header("References")]
    public HexGrid hexGrid;
    private Unit selectedUnit;

    private void Start()
    {
        if (hexGrid == null)
            hexGrid = FindFirstObjectByType<HexGrid>();

        if (MouseController.instance != null)
        {
            MouseController.instance.OnLeftMouseClick += HandleLeftClick;
            MouseController.instance.OnRightMouseClick += HandleRightClick;
            Debug.Log("GameInteraction coupled.");
        }
    }

    private void OnDestroy()
    {
        if (MouseController.instance != null)
        {
            MouseController.instance.OnLeftMouseClick -= HandleLeftClick;
            MouseController.instance.OnRightMouseClick -= HandleRightClick;
        }
    }

    private void HandleLeftClick(RaycastHit hit)
    {
        Unit hitUnit = hit.collider.GetComponent<Unit>();
        if (hitUnit != null)
        {
            if (selectedUnit != hitUnit)
            {
                selectedUnit = hitUnit;
                Debug.Log($"Unit Selected: {selectedUnit.name} on {selectedUnit.GridPosition}");
            }
            else
            {
                selectedUnit = null;
                Debug.Log("Unit Deselected.");
            }
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector2Int gridPos = GetGridPosFromRay(ray);

        HexTile clickedTile = hexGrid.GetTileAt(gridPos);

        if (clickedTile == null) return;

        if (selectedUnit != null)
        {
            if (clickedTile.OccupyingUnit == null)
            {
                Debug.Log($"Moving to: {gridPos}");
                selectedUnit.MoveToTile(gridPos, hexGrid);
                selectedUnit = null;
            }
            else if (clickedTile.OccupyingUnit != selectedUnit)
            {
                selectedUnit = clickedTile.OccupyingUnit;
                Debug.Log($"New unit selected: {selectedUnit.name}");
            }
        }
        else
        {
            if (clickedTile.OccupyingUnit != null)
            {
                selectedUnit = clickedTile.OccupyingUnit;
                Debug.Log($"Unit selected on tile: {selectedUnit.name}");
            }
        }
    }

    private void HandleRightClick(RaycastHit hit)
    {
        if (selectedUnit != null)
        {
            selectedUnit = null;
            Debug.Log("Selection cancelled.");
        }
    }

    private Vector2Int GetGridPosFromRay(Ray ray)
    {
        Plane gridPlane = new Plane(Vector3.up, new Vector3(0, hexGrid.transform.position.y, 0));

        float enterDistance;
        if (gridPlane.Raycast(ray, out enterDistance))
        {
            Vector3 hitPoint = ray.GetPoint(enterDistance);

            if (debugMarkerPrefab != null)
            {
                 // Als we nog geen instantie hebben, maak er een. Anders, verplaats hem.
                 if (GameObject.Find("DEBUG_MARKER") == null)
                 {
                     GameObject marker = Instantiate(debugMarkerPrefab, hitPoint, Quaternion.identity);
                     marker.name = "DEBUG_MARKER";
                 }
                 else
                 {
                     GameObject.Find("DEBUG_MARKER").transform.position = hitPoint;
                 }
            }

            float localX = hitPoint.x - hexGrid.transform.position.x;
            float localZ = hitPoint.z - hexGrid.transform.position.z;

            Vector2 axial = HexMetrics.CoordinateToAxial(localX, localZ, hexGrid.HexSize, hexGrid.Orientation);
            return new Vector2Int(Mathf.RoundToInt(axial.x), Mathf.RoundToInt(axial.y));
        }

        return Vector2Int.zero;
    }
}
