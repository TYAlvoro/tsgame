using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitProduction : MonoBehaviour
{
    public Transform factory; // Factory transform.
    public ForestManager forestManager; // Forest manager.
    public QuarryManager quarryManager; // Quarry manager.
    public float waitTime = 2f; // Wait time at each location.

    private NavMeshAgent agent; // NavMeshAgent for unit movement.
    private Transform currentTarget; // Current target location.
    private bool isProducing = false; // Production state.

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        Debug.Log("UnitProduction: Initialized and waiting for production to start.");
    }

    public void StartProduction()
    {
        if (!isProducing)
        {
            Debug.Log("UnitProduction: Production started.");
            isProducing = true;
            StartCoroutine(ProductionCycle());
        }
        else
        {
            Debug.Log("UnitProduction: Production already in progress.");
        }
    }

    private IEnumerator ProductionCycle()
    {
        while (forestManager.HasTrees() || quarryManager.HasStones())
        {
            // Go to quarry.
            Debug.Log("UnitProduction: Heading to quarry.");
            currentTarget = quarryManager.GetNearestQuarry(transform.position);
            if (currentTarget == null)
            {
                Debug.Log("UnitProduction: No quarries left.");
                break;
            }
            agent.SetDestination(currentTarget.position);
            yield return new WaitUntil(() => HasReachedTarget());

            // Wait at quarry.
            Debug.Log("UnitProduction: Reached quarry. Waiting...");
            yield return new WaitForSeconds(waitTime);
            quarryManager.RemoveNearestStone(transform.position);

            // Go to factory.
            Debug.Log("UnitProduction: Heading to factory.");
            currentTarget = factory;
            agent.SetDestination(currentTarget.position);
            yield return new WaitUntil(() => HasReachedTarget());

            // Wait at factory.
            Debug.Log("UnitProduction: Reached factory. Waiting...");
            yield return new WaitForSeconds(waitTime);
            quarryManager.AddResource();

            // Go to forest.
            Debug.Log("UnitProduction: Heading to forest.");
            currentTarget = forestManager.GetNearestTree(transform.position);
            if (currentTarget == null)
            {
                Debug.Log("UnitProduction: No trees left.");
                break;
            }
            agent.SetDestination(currentTarget.position);
            yield return new WaitUntil(() => HasReachedTarget());

            // Wait at forest.
            Debug.Log("UnitProduction: Reached forest. Waiting...");
            yield return new WaitForSeconds(waitTime);
            forestManager.RemoveNearestTree(transform.position);

            // Go back to factory.
            Debug.Log("UnitProduction: Heading back to factory.");
            currentTarget = factory;
            agent.SetDestination(currentTarget.position);
            yield return new WaitUntil(() => HasReachedTarget());

            // Wait at factory.
            Debug.Log("UnitProduction: Reached factory. Adding resources.");
            yield return new WaitForSeconds(waitTime);
            forestManager.AddResource();
        }

        Debug.Log("UnitProduction: Production finished. No more resources.");
        isProducing = false;
    }

    private bool HasReachedTarget()
    {
        bool hasReached = !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
        Debug.Log($"UnitProduction: Has reached target? {hasReached}");
        return hasReached;
    }
}
