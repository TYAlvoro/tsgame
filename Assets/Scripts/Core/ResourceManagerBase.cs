using System.Collections.Generic;
using UnityEngine;

public abstract class ResourceManagerBase : MonoBehaviour, IResourceManager
{
    protected List<GameObject> resources = new List<GameObject>();

    public Transform GetNearestResource(Vector3 position)
    {
        if (resources.Count == 0) return null;

        GameObject nearestResource = null;
        float minDistance = float.MaxValue;

        foreach (var resource in resources)
        {
            float distance = Vector3.Distance(position, resource.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestResource = resource;
            }
        }

        return nearestResource?.transform;
    }

    public void RemoveResource(Vector3 position)
    {
        Transform nearest = GetNearestResource(position);
        if (nearest != null)
        {
            resources.Remove(nearest.gameObject);
            Destroy(nearest.gameObject);
        }
    }

    public bool HasResources() => resources.Count > 0;

    public void ResetResources()
    {
        foreach (var resource in resources)
        {
            Destroy(resource);
        }
        resources.Clear();
    }

    public abstract void AddResource();
}