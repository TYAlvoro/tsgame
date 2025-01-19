using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ForestManager : MonoBehaviour
{
    public float cutInterval = 5f; // Interval between tree removals (seconds).
    public int treesPerInterval = 3; // Number of trees removed per interval.
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

        // Start the tree removal process.
        StartCoroutine(GradualTreeRemoval());
    }

    /// <summary>
    /// Gradually removes trees from the forest at specified intervals.
    /// </summary>
    private IEnumerator GradualTreeRemoval()
    {
        while (treeList.Count > 0)
        {
            // Remove the specified number of trees per interval.
            for (int i = 0; i < treesPerInterval; i++)
            {
                if (treeList.Count > 0)
                {
                    RemoveRandomTree();
                }
            }

            // Wait for the next interval.
            yield return new WaitForSeconds(cutInterval);
        }
    }

    /// <summary>
    /// Removes a random tree from the forest.
    /// </summary>
    private void RemoveRandomTree()
    {
        if (treeList.Count == 0) return;

        // Choose a random tree from the list.
        int randomIndex = Random.Range(0, treeList.Count);
        GameObject treeToRemove = treeList[randomIndex];

        // Remove the tree from the list and the scene.
        treeList.RemoveAt(randomIndex);
        Destroy(treeToRemove);

        // Update the tree removal counter and UI.
        treesRemoved++;
        if (treeCounterText != null)
        {
            treeCounterText.text = $"Дерево: {treesRemoved}";
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
}
