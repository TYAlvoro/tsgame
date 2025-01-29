using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ProductionUnit : BaseUnit
{
    public IResourceManager forestManager;
    public IResourceManager quarryManager;
    public FactoryManager factoryManager;

    private bool isProducing = false; // Флаг состояния производства

    public override void PerformAction()
    {
        if (!isProducing)
        {
            StartCoroutine(ProductionCycle());
        }
    }

    private IEnumerator ProductionCycle()
    {
        isProducing = true;

        while (true)
        {
            // Проверяем наличие ресурсов перед каждым действием
            if (quarryManager.HasResources())
            {
                yield return GatherResource(quarryManager);
                yield return DeliverResource("Камни");
            }
            else
            {
                Debug.Log("No stones left in the quarry!");
            }

            if (forestManager.HasResources())
            {
                yield return GatherResource(forestManager);
                yield return DeliverResource("Деревья");
            }
            else
            {
                Debug.Log("No trees left in the forest!");
            }

            // Проверяем, есть ли еще ресурсы для продолжения
            if (!quarryManager.HasResources() && !forestManager.HasResources())
            {
                Debug.Log($"{name}: No resources left to process. Stopping production.");
                break;
            }
        }

        isProducing = false;
    }

    private IEnumerator GatherResource(IResourceManager resourceManager)
    {
        Transform target = resourceManager.GetNearestResource(transform.position);
        if (target == null)
        {
            yield break;
        }

        agent.SetDestination(target.position);
        yield return new WaitUntil(() => HasReachedTarget());

        resourceManager.RemoveResource(transform.position);
        Debug.Log($"{name} collected resource from {resourceManager.GetType().Name}.");
    }

    private IEnumerator DeliverResource(string resourceType)
    {
        agent.SetDestination(factoryManager.transform.position);
        yield return new WaitUntil(() => HasReachedTarget());

        if (resourceType == "Камни")
        {
            factoryManager.AddStone();
        }
        else if (resourceType == "Деревья")
        {
            factoryManager.AddTree();
        }

        Debug.Log($"{name} delivered {resourceType} to the factory.");
    }

    private bool HasReachedTarget()
    {
        return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
    }
}
