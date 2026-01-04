using System.Collections.Generic;
using UnityEngine;
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
            SpawnUnit(2, 2, UnitTeam.Player, "Player");
            SpawnUnit(2, 4, UnitTeam.Enemy, "Enemy");
        }
    }

    private void SpawnUnit(int col, int row, UnitTeam team, string name)
    {
        Vector3 startLocal = HexMetrics.Center(HexSize, col, row, Orientation);
        Vector2 axialExact = HexMetrics.CoordinateToAxial(startLocal.x, startLocal.z, HexSize, Orientation);
        Vector2Int gridPos = new Vector2Int(Mathf.RoundToInt(axialExact.x), Mathf.RoundToInt(axialExact.y));

        HexTile tile = GetTileAt(gridPos);

        if (tile != null)
        {
            Unit spawnedUnit = Instantiate(testUnitPrefab);
            spawnedUnit.name = name;
            spawnedUnit.Initialize(team, name);
            spawnedUnit.SetStartupPosition(gridPos, this);
        }
        else
        {
            Debug.LogError($"Could not spawn {name} at {gridPos}");
        }
    }

    public void GenerateGridData()
    {
        tiles.Clear();

        for (int z = 0; z < Height; z++)
    {
        for (int x = 0; x < Width; x++)
        {
            Vector3 centerLocal = HexMetrics.Center(HexSize, x, z, Orientation);
            Vector3 worldPos = centerLocal + transform.position;

            Vector2 axialExact = HexMetrics.CoordinateToAxial(centerLocal.x, centerLocal.z, HexSize, Orientation);
            Vector2Int gridPos = new Vector2Int(Mathf.RoundToInt(axialExact.x), Mathf.RoundToInt(axialExact.y));

            HexTile newTile = new HexTile(gridPos, worldPos);

            if (!tiles.ContainsKey(gridPos))
            {
                tiles.Add(gridPos, newTile);
            }
            else
            {
                 Debug.LogWarning($"Dubble tiles detected on {gridPos}! Check HexSize/Orientation.");
            }
        }
    }
    Debug.Log($"HexGrid Data generated. {tiles.Count} tiles in memory.");
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

    public List<Vector2Int> GetTilesInRadius(Vector2Int center, int range)
    {
        List<Vector2Int> results = new List<Vector2Int>();

        for (int q = -range; q <= range; q++)
        {
            int r1 = Mathf.Max(-range, -q - range);
            int r2 = Mathf.Min(range, -q + range);

            for (int r = r1; r <= r2; r++)
            {
                Vector2Int offset = new Vector2Int(q, r);
                Vector2Int neighbourPos = center + offset;

                if (tiles.ContainsKey(neighbourPos))
                {
                    results.Add(neighbourPos);
                }
            }
        }
        return results;
    }
}

public enum HexOrientation
{
    FlatTop,
PointyTop
}