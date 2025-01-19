using UnityEngine;
using UnityEngine.UI; // Для работы с UI кнопками

public class SimpleBuildingPlacement : MonoBehaviour
{
    public GameObject buildingPrefab; // The building to place
    public GameObject placeholderPrefab; // The placeholder to show preview
    public LayerMask terrainMask; // Terrain layer mask
    public LayerMask buildingMask; // Mask for buildings (to avoid collision with placed buildings)

    public float maxSlopeAngle = 30f; // Maximum allowed slope angle for placement
    public float buildingYOffset = 1f; // Offset for building to avoid sinking into terrain

    private GameObject currentPlaceholder; // Current placeholder object
    private bool isBuildingModeActive = false; // Flag to track if the building mode is active

    public Button buildButton; // Reference to the build button UI

    void Start()
    {
        // Subscribe to the button's onClick event
        buildButton.onClick.AddListener(ToggleBuildingMode);
    }

    void Update()
    {
        if (isBuildingModeActive)
        {
            UpdatePlaceholder();

            if (Input.GetMouseButtonDown(0) && currentPlaceholder != null) // Left Click
            {
                if (IsPlacementValid())
                {
                    PlaceBuilding();
                }
                else
                {
                    Debug.Log("Placement invalid!");
                }
            }
        }
    }

    void ToggleBuildingMode()
    {
        // Toggle the building mode (activate/deactivate)
        isBuildingModeActive = !isBuildingModeActive;

        if (!isBuildingModeActive && currentPlaceholder != null)
        {
            // Destroy the placeholder if building mode is deactivated
            Destroy(currentPlaceholder);
        }
    }

    void UpdatePlaceholder()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, terrainMask))
        {
            if (currentPlaceholder == null)
            {
                // Instantiate the placeholder object
                currentPlaceholder = Instantiate(placeholderPrefab, hit.point, Quaternion.identity);

                // Ensure the placeholder has its own material instance
                Renderer renderer = currentPlaceholder.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = new Material(renderer.material);
                }
            }
            else
            {
                // Update the placeholder position with extra Y offset
                currentPlaceholder.transform.position = hit.point + Vector3.up * buildingYOffset;
            }

            // Check for overlaps to determine valid placement
            if (IsPlacementValid())
            {
                currentPlaceholder.GetComponent<Renderer>().material.color = Color.green; // Valid
            }
            else
            {
                currentPlaceholder.GetComponent<Renderer>().material.color = Color.red; // Invalid
            }
        }
        else if (currentPlaceholder != null)
        {
            Destroy(currentPlaceholder); // Destroy placeholder if raycast misses
        }
    }

    bool IsPlacementValid()
    {
        // Check for overlap using OverlapBox or other methods
        Collider[] colliders = Physics.OverlapBox(
            currentPlaceholder.transform.position,
            currentPlaceholder.GetComponent<Collider>().bounds.extents * 0.9f, // Reduced size for more precise check
            Quaternion.identity,
            buildingMask // Only check for other buildings within the building layer
        );

        // Check if there are any colliders except the placeholder itself
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject != currentPlaceholder)
            {
                return false; // Invalid placement
            }
        }

        // Check terrain slope (angle)
        Vector3 terrainNormal = GetTerrainNormal(currentPlaceholder.transform.position);
        float angle = Vector3.Angle(terrainNormal, Vector3.up); // Calculate the angle between the terrain normal and the vertical axis

        if (angle > maxSlopeAngle) // If the slope is too steep
        {
            return false; // Invalid placement
        }

        return true; // Valid placement
    }

    // Get the terrain normal at a given position
    Vector3 GetTerrainNormal(Vector3 position)
    {
        Terrain terrain = Terrain.activeTerrain;
        if (terrain != null)
        {
            Vector3 terrainNormal = terrain.terrainData.GetInterpolatedNormal(
                (position.x - terrain.transform.position.x) / terrain.terrainData.size.x,
                (position.z - terrain.transform.position.z) / terrain.terrainData.size.z
            );
            return terrainNormal;
        }
        return Vector3.up; // Default to upward normal if terrain is not available
    }

    void PlaceBuilding()
    {
        // Instantiate the building at the placeholder's position
        Instantiate(buildingPrefab, currentPlaceholder.transform.position, Quaternion.identity);

        // Destroy the placeholder after placing the building
        Destroy(currentPlaceholder);
    }
}
