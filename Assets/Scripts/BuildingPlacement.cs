using UnityEngine;
using UnityEngine.UI;

public class BuildingPlacement : MonoBehaviour
{
    [SerializeField] private GameObject[] buildingPrefabs; // Array of building prefabs
    [SerializeField] private Button[] buildingButtons; // Buttons for selecting buildings
    [SerializeField] private GameObject placeholderPrefab; // Placeholder prefab for preview
    [SerializeField] private LayerMask terrainMask; // Mask for terrain
    [SerializeField] private LayerMask buildingMask; // Mask for existing buildings
    [SerializeField] private GameObject buildingSelectionPanel; // Panel for building selection
    [SerializeField] private Button openSelectionPanelButton; // Button to toggle the selection panel

    [SerializeField] private float maxSlopeAngle = 30f; // Maximum slope angle for placement
    [SerializeField] private float buildingYOffset = 1f; // Offset for building placement

    private GameObject currentPlaceholder; // Active placeholder object
    private GameObject selectedBuildingPrefab; // Currently selected building prefab
    private bool isBuildingModeActive = false; // Flag to track building mode

    private void Start()
    {
        // Bind the toggle function to the panel button
        openSelectionPanelButton.onClick.AddListener(ToggleBuildingSelectionPanel);

        // Bind selection functions to building buttons
        for (int i = 0; i < buildingButtons.Length; i++)
        {
            int index = i; // Capture index to avoid closure issues
            buildingButtons[i].onClick.AddListener(() => SelectBuilding(index));
        }

        // Ensure the building selection panel is initially hidden
        buildingSelectionPanel.SetActive(false);
    }

    private void Update()
    {
        if (isBuildingModeActive)
        {
            UpdatePlaceholder();

            if (Input.GetMouseButtonDown(0) && currentPlaceholder != null) // Place building on left-click
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

    /// <summary>
    /// Toggles the visibility of the building selection panel.
    /// </summary>
    private void ToggleBuildingSelectionPanel()
    {
        bool isActive = buildingSelectionPanel.activeSelf;
        buildingSelectionPanel.SetActive(!isActive);

        Debug.Log($"Building selection panel is now {(isActive ? "closed" : "opened")}.");

        if (isActive)
        {
            DeactivateBuildingMode();
        }
    }

    /// <summary>
    /// Deactivates building mode and clears the placeholder.
    /// </summary>
    private void DeactivateBuildingMode()
    {
        isBuildingModeActive = false;
        currentPlaceholder?.SetActive(false);
        Destroy(currentPlaceholder);

        Debug.Log("Building mode deactivated.");
    }

    /// <summary>
    /// Selects a building prefab and activates building mode.
    /// </summary>
    /// <param name="index">Index of the selected building in the prefab array.</param>
    private void SelectBuilding(int index)
    {
        if (index >= 0 && index < buildingPrefabs.Length)
        {
            selectedBuildingPrefab = buildingPrefabs[index];
            isBuildingModeActive = true;
            Debug.Log($"Building selected: {selectedBuildingPrefab.name}");
            buildingSelectionPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Updates the position and validity of the placeholder object.
    /// </summary>
    private void UpdatePlaceholder()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, terrainMask))
        {
            if (currentPlaceholder == null)
            {
                currentPlaceholder = Instantiate(placeholderPrefab, hit.point, Quaternion.identity);
                Renderer renderer = currentPlaceholder.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = new Material(renderer.material); // Ensure unique material instance
                }
            }
            else
            {
                currentPlaceholder.transform.position = hit.point + Vector3.up * buildingYOffset;
            }

            // Change placeholder color based on placement validity
            Renderer placeholderRenderer = currentPlaceholder.GetComponent<Renderer>();
            if (placeholderRenderer != null)
            {
                placeholderRenderer.material.color = IsPlacementValid() ? Color.green : Color.red;
            }
        }
        else if (currentPlaceholder != null)
        {
            Destroy(currentPlaceholder); // Remove placeholder if raycast misses
        }
    }

    /// <summary>
    /// Checks if the current placeholder position is valid for building placement.
    /// </summary>
    /// <returns>True if placement is valid, otherwise false.</returns>
    private bool IsPlacementValid()
    {
        if (currentPlaceholder == null) return false;

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
                return false; // Invalid if colliding with another building
            }
        }

        Vector3 terrainNormal = GetTerrainNormal(currentPlaceholder.transform.position);
        float angle = Vector3.Angle(terrainNormal, Vector3.up);

        if (angle > maxSlopeAngle)
        {
            return false; // Invalid if slope is too steep
        }

        return true; // Valid placement
    }

    /// <summary>
    /// Retrieves the terrain normal at a given position.
    /// </summary>
    /// <param name="position">The position to check.</param>
    /// <returns>The normal vector of the terrain at the given position.</returns>
    private Vector3 GetTerrainNormal(Vector3 position)
    {
        Terrain terrain = Terrain.activeTerrain;
        if (terrain != null)
        {
            return terrain.terrainData.GetInterpolatedNormal(
                (position.x - terrain.transform.position.x) / terrain.terrainData.size.x,
                (position.z - terrain.transform.position.z) / terrain.terrainData.size.z
            );
        }
        return Vector3.up; // Default to vertical normal
    }

    /// <summary>
    /// Places the selected building at the placeholder's position.
    /// </summary>
    private void PlaceBuilding()
    {
        Instantiate(selectedBuildingPrefab, currentPlaceholder.transform.position, Quaternion.identity);
        Destroy(currentPlaceholder);
        isBuildingModeActive = false;
    }
}
