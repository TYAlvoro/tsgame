using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuarryManager : ResourceManagerBase
{
    [SerializeField] private GameObject quarryRoot; // Родительский объект для всех каменоломен

    private int stonesProcessed = 0;

    private void Start()
    {
        InitializeResources();
    }

    private void InitializeResources()
    {
        resources.Clear();
        foreach (Transform child in quarryRoot.transform)
        {
            resources.Add(child.gameObject);
        }

        Debug.Log($"QuarryManager: Initialized with {resources.Count} stones.");
    }

    public override void AddResource()
    {
        stonesProcessed++;
    }
}