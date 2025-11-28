using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class HexGridMeshGenerator : MonoBehaviour
{
    [field: SerializeField] public LayerMask gridLayer { get; private set; }
    [field: SerializeField] public HexGrid hexGrid { get; private set; }
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private GameObject explosionTest;


    private void Awake()
    {
        if (hexGrid == null)
            hexGrid = GetComponentInParent<HexGrid>();
        if (hexGrid == null)
            Debug.LogError("HexGridMeshGenerator could not find a HexGrid component in its parent or itself.");
    }

    public void CreateHexMesh()
    {
        CreateHexMesh(hexGrid.Width, hexGrid.Height, hexGrid.HexSize, hexGrid.Orientation, gridLayer);
    }

    public void CreateHexMesh(HexGrid hexGrid, LayerMask layerMask)
    {
        this.hexGrid = hexGrid;
        this.gridLayer = layerMask;
        CreateHexMesh(hexGrid.Width, hexGrid.Height, hexGrid.HexSize, hexGrid.Orientation, layerMask);
    }

    public void CreateHexMesh(int width, int height, float HexSize, HexOrientation orientation, LayerMask layerMask)
    {
        ClearHexGridMesh();
        Vector3[] vertices = new Vector3[7 * width * height];

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 centerPosition = HexMetrics.Center(HexSize, x, z, orientation);
                vertices[(z * width + x) * 7] = centerPosition;
                for (int s = 0; s < HexMetrics.Corners(HexSize, orientation).Length; s++)
                {
                    vertices[(z * width + x) * 7 + s + 1] = centerPosition + HexMetrics.Corners(HexSize, orientation)[s % 6];
                }
            }
        }

        int[] triangles = new int[3 * 6 * width * height];

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int s = 0; s < HexMetrics.Corners(HexSize, orientation).Length; s++)
                {
                    int cornerIndex = s + 2 > 6 ? s + 2 - 6 : s + 2;
                    triangles[3 * 6 * (z * width + x) + s * 3 + 0] = (z * width + x) * 7;
                    triangles[3 * 6 * (z * width + x) + s * 3 + 1] = (z * width + x) * 7 + cornerIndex;
                    triangles[3 * 6 * (z * width + x) + s * 3 + 2] = (z * width + x) * 7 + s + 1;
                }
            }
        }

        Mesh mesh = new Mesh();
        mesh.name = "Hex Mesh";
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();
        mesh.RecalculateUVDistributionMetrics();

        GetComponent<MeshFilter>().sharedMesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;

         int gridLayerIndex = GetLayerIndex(layerMask);
         Debug.Log("Layer Index:" + gridLayerIndex);
         gameObject.layer = gridLayerIndex;

         MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
         if (meshRenderer.sharedMaterial == null)
         {
        meshRenderer.sharedMaterial = defaultMaterial != null
        ? defaultMaterial
        : new Material(Shader.Find("Standard"));
        }
    }

    public void ClearHexGridMesh()
    {
        if (GetComponent<MeshFilter>().sharedMesh == null)
            return;
        GetComponent<MeshFilter>().sharedMesh.Clear();
        GetComponent<MeshCollider>().sharedMesh.Clear();
    }

    private int GetLayerIndex(LayerMask layerMask)
    {
        int layerMaskValue = layerMask.value;
        Debug.Log("Layer Mask Value:" + layerMaskValue);
        for (int i = 0; i < 32; i++)
        {
            if (((1 << i) & layerMaskValue) != 0)
            {
                Debug.Log("Layer Index Loop:" + i);
                return i;
            }
        }
        return 0;
    }

    private void OnLeftMouseClick(RaycastHit hit)
    {
        Debug.Log("Hit object:" + hit.transform.name + "at position" + hit.point);
        float localX = hit.point.x - hit.transform.position.x;
        float localZ = hit.point.z - hit.transform.position.z;
        //Debug.Log("Hex position:" + HexMetrics.CoordinateToAxial(localX, localZ, Grid.Hexsize, Grid.Orientation));
        Debug.Log("Offset Position:" + HexMetrics.CoordinateToOffset(localX, localZ, hexGrid.HexSize, hexGrid.Orientation));
    }

    private void OnRightMouseClick(RaycastHit hit)
{
    float localX = hit.point.x - hexGrid.transform.position.x;
    float localZ = hit.point.z - hexGrid.transform.position.z;
    Vector2 axial = HexMetrics.CoordinateToAxial(localX, localZ, hexGrid.HexSize, hexGrid.Orientation);
    Vector2Int clickPos = new Vector2Int(Mathf.RoundToInt(axial.x), Mathf.RoundToInt(axial.y));

    HexTile clickedTile = hexGrid.GetTileAt(clickPos);

    if (clickedTile != null)
    {
        Debug.Log($"Hit. Clicked on tile: {clickedTile.TileName} on worldposition {clickedTile.WorldPosition}");
    }
    else
    {
        Debug.Log("Missed. Clicked outside of grid data.");
    }
}
}
