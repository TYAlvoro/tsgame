using UnityEngine;

[ExecuteInEditMode]
public class QuarryGenerator : MonoBehaviour
{
    public GameObject quarryPrefab; // Prefab of the quarry.
    public Terrain terrain; // Reference to the terrain.
    public int numberOfQuarries = 5; // Number of quarries to generate.
    public Vector3 generationAreaCenter = new Vector3(300, 0, 200); // Center of the generation area.
    public Vector3 generationAreaSize = new Vector3(50, 0, 50); // Size of the generation area.
    public float minDistanceBetweenQuarries = 10f; // Minimum distance between quarries.
    public Vector3 minScale = new Vector3(2f, 2f, 2f); // Minimum scale of the quarry.
    public Vector3 maxScale = new Vector3(5f, 5f, 5f); // Maximum scale of the quarry.

    /// <summary>
    /// Generates quarries in the defined generation area.
    /// </summary>
    [ContextMenu("Generate Quarries")]
    public void GenerateQuarries()
    {
        if (!quarryPrefab || !terrain)
        {
            Debug.LogError("QuarryGenerator: Quarry prefab or Terrain is not assigned!");
            return;
        }

        // Remove all existing quarries.
        ClearAllQuarries();

        // Generate new quarries.
        for (int i = 0; i < numberOfQuarries; i++)
        {
            Vector3 position = GetRandomPosition();

            if (position != Vector3.zero && IsPositionValid(position))
            {
                position.y = terrain.SampleHeight(position); // Set the object's height.
                GameObject quarry = Instantiate(quarryPrefab, position, Quaternion.identity, transform);
                quarry.transform.localScale = GetRandomScale(); // Set a random scale.
            }
            else
            {
                i--; // Retry if the position is invalid.
            }
        }
    }

    /// <summary>
    /// Removes all objects that are children of this object.
    /// </summary>
    private void ClearAllQuarries()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// Generates a random position within the generation area.
    /// </summary>
    private Vector3 GetRandomPosition()
    {
        float randomX = Random.Range(generationAreaCenter.x - generationAreaSize.x / 2, generationAreaCenter.x + generationAreaSize.x / 2);
        float randomZ = Random.Range(generationAreaCenter.z - generationAreaSize.z / 2, generationAreaCenter.z + generationAreaSize.z / 2);

        if (randomX < terrain.transform.position.x || randomX > terrain.terrainData.size.x + terrain.transform.position.x ||
            randomZ < terrain.transform.position.z || randomZ > terrain.terrainData.size.z + terrain.transform.position.z)
        {
            return Vector3.zero; // Invalid position.
        }

        return new Vector3(randomX, 0, randomZ);
    }

    /// <summary>
    /// Checks if a position is valid for placing the quarry.
    /// Ensures it is not too close to other quarries.
    /// </summary>
    private bool IsPositionValid(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, minDistanceBetweenQuarries);
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Quarry")) return false; // Too close to another quarry.
        }
        return true;
    }

    /// <summary>
    /// Returns a random scale for the quarry.
    /// </summary>
    private Vector3 GetRandomScale()
    {
        float randomX = Random.Range(minScale.x, maxScale.x);
        float randomY = Random.Range(minScale.y, maxScale.y);
        float randomZ = Random.Range(minScale.z, maxScale.z);

        return new Vector3(randomX, randomY, randomZ); // Randomized scale.
    }
}
