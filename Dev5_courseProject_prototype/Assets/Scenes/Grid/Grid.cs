using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    public GameObject objectToPlace;   
    public float gridSize = 1f;        
    private GameObject ghostObject;    

    private Dictionary<Vector3, GameObject> placedObjects = new Dictionary<Vector3, GameObject>();

    private enum GhostMode { Place, Remove }
    private GhostMode currentMode = GhostMode.Place;

    private void Start()
    {
        CreateGhostObject();
    }

    private void Update()
    {
        UpdateGhostPosition();

        
        if (Input.GetKey(KeyCode.LeftShift))
            currentMode = GhostMode.Remove;
        else
            currentMode = GhostMode.Place;

        if (Input.GetMouseButtonDown(0))
        {
            if (currentMode == GhostMode.Place)
                PlaceObject();
            else if (currentMode == GhostMode.Remove)
                RemoveObject();
        }
    }

    void CreateGhostObject()
    {
        ghostObject = Instantiate(objectToPlace);
        ghostObject.GetComponent<Collider>().enabled = false;

        Renderer[] renderers = ghostObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material mat = renderer.material;
            Color color = mat.color;
            color.a = 0.5f;
            mat.color = color;
        }
    }

    void UpdateGhostPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 point = hit.point;

            Vector3 snappedPosition = new Vector3(
                Mathf.Round(point.x / gridSize) * gridSize,
                Mathf.Round(point.y / gridSize) * gridSize,
                Mathf.Round(point.z / gridSize) * gridSize
            );

            ghostObject.transform.position = snappedPosition;

            if (currentMode == GhostMode.Place)
            {
                if (placedObjects.ContainsKey(snappedPosition))
                    SetGhostColor(new Color(1f, 0f, 0f, 0.5f)); 
                else
                    SetGhostColor(new Color(1f, 1f, 1f, 0.5f)); 
            }
            else if (currentMode == GhostMode.Remove)
            {
                if (placedObjects.ContainsKey(snappedPosition))
                    SetGhostColor(new Color(1f, 0f, 0f, 0.5f)); 
                else
                    SetGhostColor(new Color(0.5f, 0.5f, 0.5f, 0.3f)); 
            }
        }
    }

    void SetGhostColor(Color color)
    {
        Renderer[] renderers = ghostObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material mat = renderer.material;
            mat.color = color;
        }
    }

    void PlaceObject()
{
    Vector3 placementPosition = ghostObject.transform.position;

    if (!placedObjects.ContainsKey(placementPosition))
    {
        GameObject placed = Instantiate(objectToPlace, placementPosition, Quaternion.identity);
        placedObjects.Add(placementPosition, placed);

        MeleeUnit unit = placed.GetComponent<MeleeUnit>();
        if (unit != null)
        {
                Debug.Log("failed to find cube");
            unit.gridPosition = Vector3Int.RoundToInt(placementPosition);
        }
    }
}



    void RemoveObject()
    {
        Vector3 targetPosition = ghostObject.transform.position;

        if (placedObjects.ContainsKey(targetPosition))
        {
            GameObject toRemove = placedObjects[targetPosition];
            Destroy(toRemove);
            placedObjects.Remove(targetPosition);
        }
    }
}
