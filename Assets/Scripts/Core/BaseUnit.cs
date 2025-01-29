using UnityEngine;
using UnityEngine.AI;

public abstract class BaseUnit : MonoBehaviour, IUnit
{
    protected NavMeshAgent agent;

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public abstract void PerformAction();
}