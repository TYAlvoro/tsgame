using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ForestManager : MonoBehaviour
{
    public GameObject forestRoot; // Parent object containing all trees.
    public TMP_Text treeCounterText; // Optional: UI element for displaying tree count.

    private List<GameObject> treeList = new List<GameObject>(); // List of trees in the forest.
    private int treesRemoved = 0; // Counter for removed trees.

    void Start()
    {
        // Initialize the list of trees from the forestRoot's children.
        foreach (Transform child in forestRoot.transform)
        {
            if (!treeList.Contains(child.gameObject))
            {
                treeList.Add(child.gameObject);
            }
        }
    }

    /// <summary>
    /// Resets the forest to its initial state.
    /// </summary>
    public void ResetForest()
    {
        // Destroy all remaining trees.
        foreach (GameObject tree in treeList)
        {
            Destroy(tree);
        }

        treeList.Clear();

        // Reinitialize the forest with the original trees.
        foreach (Transform child in forestRoot.transform)
        {
            GameObject newTree = Instantiate(child.gameObject, child.position, child.rotation, forestRoot.transform);
            treeList.Add(newTree);
        }

        // Reset counters and update UI.
        treesRemoved = 0;
        if (treeCounterText != null)
        {
            treeCounterText.text = $"Дерево: {treesRemoved}";
        }
    }

    public Transform GetNearestTree(Vector3 position)
    {
        if (treeList.Count == 0)
        {
            Debug.Log("ForestManager: No trees left.");
            return null;
        }

        GameObject nearestTree = null;
        float minDistance = float.MaxValue;

        foreach (GameObject tree in treeList)
        {
            float distance = Vector3.Distance(position, tree.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestTree = tree;
            }
        }

        Debug.Log($"ForestManager: Nearest tree found at distance {minDistance}.");
        return nearestTree?.transform;
    }

    public void RemoveNearestTree(Vector3 position)
    {
        Transform nearestTree = GetNearestTree(position);
        if (nearestTree != null)
        {
            Debug.Log($"ForestManager: Removing tree at {nearestTree.position}.");
            treeList.Remove(nearestTree.gameObject);
            Destroy(nearestTree.gameObject);
        }
    }

    public bool HasTrees()
    {
        bool hasTrees = treeList.Count > 0;
        Debug.Log($"ForestManager: Trees available? {hasTrees}");
        return hasTrees;
    }

    public void AddResource()
    {
        treesRemoved++;
        Debug.Log($"ForestManager: Tree delivered. Total trees processed: {treesRemoved}");
        if (treeCounterText != null)
        {
            treeCounterText.text = $"Дерево: {treesRemoved}";
        }
    }
}
