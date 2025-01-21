using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuarryManager : MonoBehaviour
{ 
    public GameObject quarryRoot; // Parent object containing all quarries.
    public TMP_Text stoneCounterText; // Optional: UI element for displaying stone count.

    private List<GameObject> quarryList = new List<GameObject>(); // List of quarries in the area.
    private int stonesExtracted = 0; // Counter for extracted stones.

    void Start()
    {
        // Initialize the list of quarries from the quarryRoot's children.
        foreach (Transform child in quarryRoot.transform)
        {
            if (!quarryList.Contains(child.gameObject))
            {
                quarryList.Add(child.gameObject);
            }
        }
    }

    /// <summary>
    /// Resets the quarry to its initial state.
    /// </summary>
    public void ResetQuarry()
    {
        // Destroy all remaining stones.
        foreach (GameObject stone in quarryList)
        {
            Destroy(stone);
        }
        quarryList.Clear();

        // Reinitialize the quarry with the original stones.
        foreach (Transform child in quarryRoot.transform)
        {
            GameObject newStone = Instantiate(child.gameObject, child.position, child.rotation, quarryRoot.transform);
            quarryList.Add(newStone);
        }

        // Reset counters and update UI.
        stonesExtracted = 0;
        if (stoneCounterText != null)
        {
            stoneCounterText.text = $"Камень: {stonesExtracted}";
        }
    }
    
    public Transform GetNearestQuarry(Vector3 position)
    {
        if (quarryList.Count == 0)
        {
            Debug.Log("QuarryManager: No quarries left.");
            return null;
        }

        GameObject nearestQuarry = null;
        float minDistance = float.MaxValue;

        foreach (GameObject quarry in quarryList)
        {
            float distance = Vector3.Distance(position, quarry.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestQuarry = quarry;
            }
        }

        Debug.Log($"QuarryManager: Nearest quarry found at distance {minDistance}.");
        return nearestQuarry?.transform;
    }

    public void RemoveNearestStone(Vector3 position)
    {
        Transform nearestQuarry = GetNearestQuarry(position);
        if (nearestQuarry != null)
        {
            Debug.Log($"QuarryManager: Removing stone at {nearestQuarry.position}.");
            quarryList.Remove(nearestQuarry.gameObject);
            Destroy(nearestQuarry.gameObject);
        }
    }

    public bool HasStones()
    {
        bool hasStones = quarryList.Count > 0;
        Debug.Log($"QuarryManager: Stones available? {hasStones}");
        return hasStones;
    }
    
    public void AddResource()
    {
        stonesExtracted++;
        Debug.Log($"QuarryManager: Stone delivered. Total stones processed: {stonesExtracted}");
        if (stoneCounterText != null)
        {
            stoneCounterText.text = $"Камень: {stonesExtracted}";
        }
    }
}
