using UnityEngine;

public class SimpleBuildingPlacement : MonoBehaviour
{
    public GameObject buildingPrefab; // Prefab
    public LayerMask terrainMask; // Terrain layer

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // On left click
        {
            PlaceBuilding();
        }
    }

    void PlaceBuilding()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, terrainMask))
        {
            Instantiate(buildingPrefab, hit.point, Quaternion.identity);
        }
    }
}
