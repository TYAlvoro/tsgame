using UnityEngine;

[ExecuteInEditMode]
public class ForestGenerator : MonoBehaviour
{
    public GameObject treePrefab; // Prefab of the tree.
    public Terrain terrain; // Reference to the terrain.
    public int numberOfTrees = 150; // Target number of trees.
    public Vector3 generationAreaCenter = new Vector3(300, 0, 200); // Generation area center.
    public Vector3 generationAreaSize = new Vector3(50, 0, 50); // Generation area size.
    public float distanceBetweenTrees = 2f; // Minimum distance between trees.
    public Vector3 minScale = new Vector3(0.8f, 0.8f, 0.8f); // Min tree scale.
    public Vector3 maxScale = new Vector3(1.5f, 2f, 1.5f); // Max tree scale.

    [ContextMenu("Generate Forest")]
    public void ManualGenerateForest()
    {
        GenerateForest();
    }

    private void GenerateForest()
    {
        if (!treePrefab || !terrain)
        {
            Debug.LogError("TreeGenerator: No TreePrefab or Terrain assigned!");
            return;
        }

        ClearAllTrees(); // Remove existing trees.

        for (int i = 0; i < numberOfTrees; i++)
        {
            Vector3 position = GetRandomPosition();

            if (position != Vector3.zero && IsPositionValid(position))
            {
                position.y = terrain.SampleHeight(position); // Adjust height to terrain.
                GameObject tree = Instantiate(treePrefab, position, Quaternion.identity, transform);
                tree.transform.localScale = GetRandomScale(); // Randomize scale.
            }
            else
            {
                i--; // Retry if invalid position.
            }
        }
    }

    private void ClearAllTrees()
    {
        // Remove all child objects.
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        if (transform.childCount > 0)
        {
            Debug.LogError("Some objects were not deleted properly!");
        }
    }

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

    private bool IsPositionValid(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, distanceBetweenTrees);
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Tree")) return false; // Too close to another tree.
        }
        return true;
    }

    private Vector3 GetRandomScale()
    {
        float randomX = Random.Range(minScale.x, maxScale.x);
        float randomY = Random.Range(minScale.y, maxScale.y);
        float randomZ = Random.Range(minScale.z, maxScale.z);

        return new Vector3(randomX, randomY, randomZ); // Randomized scale.
    }
}
