using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ForestManager : ResourceManagerBase
{
    [SerializeField] private GameObject forestRoot; // Родительский объект для всех деревьев

    private int treesProcessed = 0;

    private void Start()
    {
        InitializeResources();
    }

    private void InitializeResources()
    {
        resources.Clear();
        foreach (Transform child in forestRoot.transform)
        {
            resources.Add(child.gameObject);
        }

        Debug.Log($"ForestManager: Initialized with {resources.Count} trees.");
    }

    public override void AddResource()
    {
        treesProcessed++;
    }
}