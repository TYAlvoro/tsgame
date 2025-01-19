using UnityEngine;

public class SimpleBuildingPlacement : MonoBehaviour
{
    public GameObject buildingPrefab; // Building
    public GameObject placeholderPrefab; // Placeholder
    public LayerMask terrainMask; // Terrain layer

    private GameObject currentPlaceholder;

    void Update()
    {
        UpdatePlaceholder();

        if (Input.GetMouseButtonDown(0) && currentPlaceholder != null) // Left Click
        {
            PlaceBuilding();
        }
    }

    void UpdatePlaceholder()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, terrainMask))
        {
            if (currentPlaceholder == null)
            {
                currentPlaceholder = Instantiate(placeholderPrefab, hit.point, Quaternion.identity);
            }
            else
            {
                currentPlaceholder.transform.position = hit.point;
            }
        }
        else if (currentPlaceholder != null)
        {
            Destroy(currentPlaceholder);
        }
    }

    void PlaceBuilding()
    {
        Instantiate(buildingPrefab, currentPlaceholder.transform.position, Quaternion.identity);
        Destroy(currentPlaceholder);
    }
}
