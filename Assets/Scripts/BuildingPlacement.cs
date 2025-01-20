using UnityEngine;
using UnityEngine.UI;

public class BuildingPlacement : MonoBehaviour
{
    public GameObject[] buildingPrefabs; // Array of building prefabs
    public Button[] buildingButtons; // Array of buttons for selecting buildings
    public GameObject placeholderPrefab; // Placeholder prefab for preview
    public LayerMask terrainMask; // Mask for terrain
    public LayerMask buildingMask; // Mask for existing buildings
    public GameObject buildingSelectionPanel; // Panel with building selection buttons
    public Button openSelectionPanelButton; // Button to open/close the selection panel

    public float maxSlopeAngle = 30f; // Maximum slope angle for placement
    public float buildingYOffset = 1f; // Y-offset for building placement

    private GameObject currentPlaceholder; // Current placeholder object
    private GameObject selectedBuildingPrefab; // Currently selected building prefab
    private bool isBuildingModeActive = false; // Flag for building mode

    void Start()
    {
        // Initialize button for opening the panel
        openSelectionPanelButton.onClick.AddListener(ToggleBuildingSelectionPanel);

        // Add listeners for each building button
        for (int i = 0; i < buildingButtons.Length; i++)
        {
            int index = i; // Capture index to avoid closure issues
            buildingButtons[i].onClick.AddListener(() => SelectBuilding(index));
        }

        // Ensure the building selection panel is hidden at the start
        buildingSelectionPanel.SetActive(false);
    }

    void Update()
    {
        if (isBuildingModeActive)
        {
            UpdatePlaceholder();

            if (Input.GetMouseButtonDown(0) && currentPlaceholder != null) // Left-click to place building
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

    // Open or close the building selection panel
    void ToggleBuildingSelectionPanel()
    {
        bool isActive = buildingSelectionPanel.activeSelf;
        buildingSelectionPanel.SetActive(!isActive); // Toggle panel visibility

        if (isActive)
        {
            // Deactivate building mode and clear placeholder if panel is closed
            isBuildingModeActive = false;
            if (currentPlaceholder != null)
            {
                Destroy(currentPlaceholder);
            }
        }
    }

    // Select a building and activate building mode
    void SelectBuilding(int index)
    {
        if (index >= 0 && index < buildingPrefabs.Length)
        {
            selectedBuildingPrefab = buildingPrefabs[index]; // Set the selected building prefab
            isBuildingModeActive = true; // Activate building mode
            Debug.Log("Building selected: " + selectedBuildingPrefab.name);

            // Hide the selection panel after selecting a building
            buildingSelectionPanel.SetActive(false);
        }
    }

    // Update the placeholder position and validity
    void UpdatePlaceholder()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, terrainMask))
        {
            if (currentPlaceholder == null)
            {
                // Create the placeholder
                currentPlaceholder = Instantiate(placeholderPrefab, hit.point, Quaternion.identity);
                Renderer renderer = currentPlaceholder.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = new Material(renderer.material); // Ensure material instance
                }
            }
            else
            {
                // Update placeholder position
                currentPlaceholder.transform.position = hit.point + Vector3.up * buildingYOffset;
            }

            // Update placeholder color based on placement validity
            if (IsPlacementValid())
            {
                currentPlaceholder.GetComponent<Renderer>().material.color = Color.green; // Valid placement
            }
            else
            {
                currentPlaceholder.GetComponent<Renderer>().material.color = Color.red; // Invalid placement
            }
        }
        else if (currentPlaceholder != null)
        {
            Destroy(currentPlaceholder); // Remove placeholder if raycast misses
        }
    }

    // Check if the placement is valid
    bool IsPlacementValid()
    {
        Collider[] colliders = Physics.OverlapBox(
            currentPlaceholder.transform.position,
            currentPlaceholder.GetComponent<Collider>().bounds.extents * 0.9f,
            Quaternion.identity,
            buildingMask
        );

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject != currentPlaceholder)
            {
                return false; // Placement invalid due to collision
            }
        }

        Vector3 terrainNormal = GetTerrainNormal(currentPlaceholder.transform.position);
        float angle = Vector3.Angle(terrainNormal, Vector3.up);

        if (angle > maxSlopeAngle)
        {
            return false; // Placement invalid due to slope
        }

        return true; // Placement is valid
    }

    // Get the terrain normal at a position
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
        return Vector3.up; // Default to upward normal
    }

    // Place the building at the placeholder's position
    void PlaceBuilding()
    {
        Instantiate(selectedBuildingPrefab, currentPlaceholder.transform.position, Quaternion.identity); // Place building
        Destroy(currentPlaceholder); // Destroy the placeholder
        isBuildingModeActive = false; // Exit building mode
    }
}