using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuarryManager : MonoBehaviour
{
    public float cutInterval = 5f; // Interval between quarry extractions (seconds).
    public int stonesPerInterval = 3; // Number of stones removed per interval.
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

        // Start the gradual stone extraction process.
        StartCoroutine(GradualStoneExtraction());
    }

    /// <summary>
    /// Gradually removes stones from the quarry at specified intervals.
    /// </summary>
    private IEnumerator GradualStoneExtraction()
    {
        while (quarryList.Count > 0)
        {
            // Remove the specified number of stones per interval.
            for (int i = 0; i < stonesPerInterval; i++)
            {
                if (quarryList.Count > 0)
                {
                    RemoveRandomStone();
                }
            }

            // Wait for the next interval.
            yield return new WaitForSeconds(cutInterval);
        }
    }

    /// <summary>
    /// Removes a random stone from the quarry.
    /// </summary>
    private void RemoveRandomStone()
    {
        if (quarryList.Count == 0) return;

        // Choose a random stone from the list.
        int randomIndex = Random.Range(0, quarryList.Count);
        GameObject stoneToRemove = quarryList[randomIndex];

        // Remove the stone from the list and the scene.
        quarryList.RemoveAt(randomIndex);
        Destroy(stoneToRemove);

        // Update the stone extraction counter and UI.
        stonesExtracted++;
        if (stoneCounterText != null)
        {
            stoneCounterText.text = $"Камень: {stonesExtracted}";
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
}
