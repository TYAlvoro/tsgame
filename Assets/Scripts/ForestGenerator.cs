using UnityEngine;

[ExecuteInEditMode]
public class ForestGenerator : MonoBehaviour
{
    public GameObject treePrefab; // Prefab of the tree.
    public Terrain terrain; // Reference to the terrain.
    public int numberOfTrees = 150; // Number of trees to generate.
    public Vector3 generationAreaCenter = new Vector3(300, 0, 200); // Center of the generation area.
    public Vector3 generationAreaSize = new Vector3(50, 0, 50); // Size of the generation area.
    public float distanceBetweenTrees = 2f; // Minimum distance between trees.
    public Vector3 minScale = new Vector3(0.8f, 0.8f, 0.8f); // Minimum tree scale.
    public Vector3 maxScale = new Vector3(1.5f, 2f, 1.5f);   // Maximum tree scale.
    public bool generateForest = false; // Trigger for forest generation.

    void Update()
    {
        if (generateForest)
        {
            GenerateForest();
            generateForest = false;
        }
    }

    /// <summary>
    /// Manually triggers the forest generation process.
    /// </summary>
    [ContextMenu("Generate Forest")]
    public void ManualGenerateForest()
    {
        GenerateForest();
    }

    /// <summary>
    /// Generates a forest of trees within the defined area.
    /// Clears existing trees before generating new ones.
    /// </summary>
    private void GenerateForest()
    {
        if (!treePrefab || !terrain)
        {
            Debug.LogError("TreeGenerator: No TreePrefab or Terrain assigned!");
            return;
        }
        
        // Clear old trees.
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }

        // Generate trees.
        for (int i = 0; i < numberOfTrees; i++)
        {
            Vector3 position = GetRandomPosition();

            if (position != Vector3.zero && IsPositionValid(position))
            {
                position.y = terrain.SampleHeight(position); // Adjust to terrain height.
                GameObject tree = Instantiate(treePrefab, position, Quaternion.identity, transform);
                tree.transform.localScale = GetRandomScale(); // Randomize tree scale.
                tree.isStatic = true;
            }
            else
            {
                i--; // Retry if position is invalid.
            }
        }
    }

    /// <summary>
    /// Gets a random position within the defined generation area.
    /// Ensures the position is within terrain bounds.
    /// </summary>
    /// <returns>A valid position or Vector3.zero if invalid.</returns>
    private Vector3 GetRandomPosition()
    {
        float randomX = Random.Range(generationAreaCenter.x - generationAreaSize.x / 2, generationAreaCenter.x + generationAreaSize.x / 2);
        float randomZ = Random.Range(generationAreaCenter.z - generationAreaSize.z / 2, generationAreaCenter.z + generationAreaSize.z / 2);

        // Ensure the position is within terrain bounds.
        if (randomX < terrain.transform.position.x || randomX > terrain.terrainData.size.x + terrain.transform.position.x ||
            randomZ < terrain.transform.position.z || randomZ > terrain.terrainData.size.z + terrain.transform.position.z)
        {
            return Vector3.zero;
        }

        return new Vector3(randomX, 0, randomZ);
    }

    /// <summary>
    /// Validates if a position is suitable for placing a tree.
    /// Ensures the position is not too close to other trees.
    /// </summary>
    /// <param name="position">The position to validate.</param>
    /// <returns>True if the position is valid, false otherwise.</returns>
    private bool IsPositionValid(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, distanceBetweenTrees);
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Tree"))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Generates a random scale for a tree within the defined size range.
    /// </summary>
    /// <returns>A Vector3 representing the random scale.</returns>
    private Vector3 GetRandomScale()
    {
        float randomX = Random.Range(minScale.x, maxScale.x);
        float randomY = Random.Range(minScale.y, maxScale.y);
        float randomZ = Random.Range(minScale.z, maxScale.z);

        return new Vector3(randomX, randomY, randomZ);
    }
}