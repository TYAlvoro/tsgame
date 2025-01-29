using UnityEngine;

public interface IResourceManager
{
    Transform GetNearestResource(Vector3 position);
    void RemoveResource(Vector3 position);
    bool HasResources();
    void ResetResources();
    void AddResource();
}