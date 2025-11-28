using System.Collections.Generic;
using UnityEngine;

public class GameInteraction : MonoBehaviour
{
    [Header("visuals")]
    public GameObject highlightPrefab;
    private List<GameObject> currentHighlights = new List<GameObject>();

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
            if (selectedUnit == null || hitUnit.Team == UnitTeam.Player)
            {
                if (hitUnit.Team == UnitTeam.Player)
                {
                    HandleSelection(hitUnit);
                }
                return;
            }
            
            if (selectedUnit != null && hitUnit.Team == UnitTeam.Enemy)
            {
                int distance = HexMetrics.GetDistance(selectedUnit.GridPosition, hitUnit.GridPosition);

                if (distance <= selectedUnit.AttackRange)
                {
                    selectedUnit.Attack(hitUnit);
                }
                else
                {
                    Debug.Log("Enemy too far away.");
                }
                return;
            }
        }

        if (selectedUnit != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector2Int gridPos = GetGridPosFromRay(ray);
            HexTile clickedTile = hexGrid.GetTileAt(gridPos);

            if (clickedTile != null && clickedTile.OccupyingUnit == null)
            {
                int distance = HexMetrics.GetDistance(selectedUnit.GridPosition, gridPos);

                if (distance <= selectedUnit.MovementRange)
                {
                    selectedUnit.MoveToTile(gridPos, hexGrid);
                    selectedUnit = null;
                    ClearHighlights();
                }
                else
                {
                    Debug.Log("Destination is too far.");
                }
            }
        }
    }

    private void HandleSelection (Unit unit)
    {
        if (selectedUnit != unit)
        {
            ClearHighlights();
            selectedUnit = unit;
            Debug.Log($"Unit selected: {selectedUnit.name}");
            ShowRange(selectedUnit);
        }
        else
        {
            selectedUnit = null;
            ClearHighlights();
        }
    }

    private void HandleRightClick(RaycastHit hit)
    {
        if (selectedUnit != null)
        {
            selectedUnit = null;
            Debug.Log("Selection cancelled.");
            ClearHighlights();
        }
    }

    private void ShowRange(Unit unit)
    {
        List<Vector2Int> rangeTiles = hexGrid.GetTilesInRadius(unit.GridPosition, unit.MovementRange);

        foreach (Vector2Int pos in rangeTiles)
        {
            if (pos == unit.GridPosition) continue;

            HexTile tile = hexGrid.GetTileAt(pos);
            if (tile != null)
            {
                Vector3 highlightPos = tile.WorldPosition + Vector3.up * 0.1f;
                GameObject highlight = Instantiate(highlightPrefab, highlightPos, highlightPrefab.transform.rotation);
                currentHighlights.Add(highlight);
            }
        }
    }

    private void ClearHighlights()
    {
        foreach (GameObject highlight in currentHighlights)
        {
            Destroy(highlight);
        }
        currentHighlights.Clear();
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
