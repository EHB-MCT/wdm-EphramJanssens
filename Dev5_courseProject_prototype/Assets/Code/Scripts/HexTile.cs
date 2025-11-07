using UnityEngine;

[System.Serializable]
public class HexTile
{
    public Vector2Int GridPosition { get; private set; }
    public Vector3 WorldPosition { get; private set; }
    public string TileName { get; set; }
    public Unit OccupyingUnit { get; set; }
    public HexTile(Vector2Int gridPos, Vector3 worldPos)
    {
        this.GridPosition = gridPos;
        this.WorldPosition = worldPos;
        this.TileName = $"Tile {gridPos}";
    }
}
