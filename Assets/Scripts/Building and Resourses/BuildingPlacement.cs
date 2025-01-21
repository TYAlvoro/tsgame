using System.Collections;
using System.Collections.Generic;
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

    private Queue<GameObject> buildingQueue = new Queue<GameObject>(); // Building Queue
    [SerializeField] private Text buildingQueueText; // The text to display the queue

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

            if (Input.GetMouseButtonDown(0) && currentPlaceholder != null)
            {
                if (IsPlacementValid())
                {
                    PlaceBuilding();
                    UpdateQueueText(); // Updating queue information
                }
                else
                {
                    Debug.Log("Placement invalid!");
                }
            }
        }
    }


    private void UpdateQueueText()
    {
        // Debugging queue state
        Debug.Log($"Queue count: {buildingQueue.Count}");

        if (buildingQueue.Count > 0)
        {
            buildingQueueText.text = $"Next building: {buildingQueue.Peek().name}"; // Show the next building in the queue
        }
        else
        {
            buildingQueueText.text = "No buildings in the queue";
        }
    }

    public void ClearBuildingQueue()
    {
        buildingQueue.Clear(); // Clearing the queue
        Debug.Log("Building queue cleared.");
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
            buildingQueue.Enqueue(selectedBuildingPrefab); // Add the building to the queue
            Debug.Log($"Building added to queue: {selectedBuildingPrefab.name}");

            // Immediately update the queue text
            UpdateQueueText();

            // If building mode is not active, start building the first item in the queue
            if (!isBuildingModeActive && buildingQueue.Count > 0)
            {
                StartNextBuilding(); // Start the first building
            }

            Debug.Log($"Building selected: {selectedBuildingPrefab.name}");
            buildingSelectionPanel.SetActive(false);
        }
    }

    private void StartNextBuilding()
    {
        if (buildingQueue.Count > 0) // If there are buildings in the queue
        {
            GameObject buildingToBuild = buildingQueue.Peek(); // Look at the first building in the queue, but don't remove it yet
            selectedBuildingPrefab = buildingToBuild; // Set it as the selected building
            isBuildingModeActive = true; // Activate building mode
            Debug.Log($"Building {selectedBuildingPrefab.name} is starting.");
        }
        else
        {
            Debug.Log("No more buildings in the queue.");
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
        StartCoroutine(PlaceBuildingWithDelay(currentPlaceholder.transform.position)); // Pass the position of the placeholder to the coroutine
        Destroy(currentPlaceholder); // Remove the placeholder immediately when building starts
        isBuildingModeActive = false; // Deactivate building mode
    }

    private IEnumerator PlaceBuildingWithDelay(Vector3 placementPosition)
    {
        // Add a small delay to simulate construction time
        yield return new WaitForSeconds(5f);

        // Place the building at the saved position of the placeholder
        Instantiate(selectedBuildingPrefab, placementPosition, Quaternion.identity);
        Debug.Log($"Building placed at: {placementPosition}");

        // Remove the building from the queue now that it has been placed
        buildingQueue.Dequeue(); // Remove the building from the queue only after placement

        // Update the queue text after the building is placed
        UpdateQueueText();

        // Start the next building
        StartNextBuilding();
    }




}
