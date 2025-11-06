using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;
using UnityEngine;
using System.CodeDom.Compiler;
using System;

public class HexGrid : MonoBehaviour
{
    [field: SerializeField] public int Width { get; private set; }
    [field: SerializeField] public int Height { get; private set; }
    [field: SerializeField] public float HexSize { get; private set; }
    [field: SerializeField] public GameObject HexPrefab { get; private set; }
    [field: SerializeField] public HexOrientation Orientation { get; private set; } = HexOrientation.FlatTop;
    public Dictionary<Vector2Int, HexTile> tiles = new Dictionary<Vector2Int, HexTile>();

    public Unit testUnitPrefab;

    private void Awake()
    {
        GenerateGridData();
    }

    private void Start()
{
    if (testUnitPrefab != null)
    {
        int startCol = 2;
        int startRow = 2;

        Vector2 axialFloat = HexMetrics.OffsetToAxial(startCol, startRow, Orientation);
        Vector2Int startAxialPos = new Vector2Int(Mathf.RoundToInt(axialFloat.x), Mathf.RoundToInt(axialFloat.y));

        Unit spawnedUnit = Instantiate(testUnitPrefab);
        spawnedUnit.name = $"Unit {startAxialPos}";

        if (GetTileAt(startAxialPos) != null)
        {
             spawnedUnit.SetStartupPosition(startAxialPos, this);
        }
        else
        {
            Debug.LogError($"Kan unit niet spawnen: Tegel [Offset {startCol},{startRow} / Axial {startAxialPos}] bestaat niet!");
        }
    }
    else
    {
        Debug.LogWarning("Vergeet niet je Test Unit Prefab toe te wijzen in de Inspector van HexGrid!");
    }
}

    public void GenerateGridData()
    {
        tiles.Clear();

        for (int z = 0; z < Height; z++)
        {
            for (int x = 0; x < Width; x++)
            {
                Vector2 axialFloat = HexMetrics.OffsetToAxial(x, z, Orientation);
                Vector2Int gridPos = new Vector2Int(Mathf.RoundToInt(axialFloat.x), Mathf.RoundToInt(axialFloat.y));

                Vector3 worldPos = HexMetrics.Center(HexSize, x, z, Orientation) + transform.position;

                HexTile newTile = new HexTile(gridPos, worldPos);

                if (!tiles.ContainsKey(gridPos))
                {
                    tiles.Add(gridPos, newTile);
                }
            }
        }
        Debug.Log($"HexGrid Data gegenereerd. {tiles.Count} tegels in geheugen.");
    }

    public HexTile GetTileAt(Vector2Int axialPos)
    {
        if (tiles.TryGetValue(axialPos, out HexTile tile))
        {
            return tile;
        }
        return null;
    }

    private void OnDrawGizmos()
    {
        for (int z = 0; z < Height; z++)
        {
            for (int x = 0; x < Width; x++)
            {
                Vector3 centerPosition = HexMetrics.Center(HexSize, x, z, Orientation) + transform.position;
                for (int s = 0; s<HexMetrics.Corners(HexSize, Orientation).Length; s++)
                {
                    Gizmos.DrawLine(
                    centerPosition + HexMetrics.Corners(HexSize, Orientation)[s % 6],
                    centerPosition + HexMetrics.Corners(HexSize, Orientation)[(s + 1) % 6]
                    );
                }
            }
        }
    }
}

public enum HexOrientation
{
    FlatTop,
PointyTop
}