using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class UnitProduction : MonoBehaviour
{
    public Transform factory; // Factory transform.
    public ForestManager forestManager; // Forest manager.
    public QuarryManager quarryManager; // Quarry manager.
    public TMP_Text productCounterText; // UI element for total products count.
    public TMP_Text stoneCounterText; // UI element for stone count.
    public TMP_Text treeCounterText; // UI element for tree count.
    public float waitTime = 2f; // Wait time at each location.
    public float conversionDelay = 2f; // Delay before resources are converted into a product.

    private NavMeshAgent agent; // NavMeshAgent for unit movement.
    private Transform currentTarget; // Current target location.
    private bool isProducing = false; // Production state.

    private int totalProducts = 0; // Total number of products.
    private int storedStones = 0; // Stones stored at factory.
    private int storedTrees = 0; // Trees stored at factory.

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        Debug.Log("UnitProduction: Initialized and waiting for production to start.");

        // Initialize UI counters
        UpdateProductCounterUI();
        UpdateResourceCounterUI();
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
            Debug.Log("UnitProduction: Reached quarry. Collecting stone...");
            yield return new WaitForSeconds(waitTime);
            quarryManager.RemoveNearestStone(transform.position);

            // Deliver stone to factory.
            Debug.Log("UnitProduction: Heading to factory.");
            currentTarget = factory;
            agent.SetDestination(currentTarget.position);
            yield return new WaitUntil(() => HasReachedTarget());

            Debug.Log("UnitProduction: Reached factory. Delivering stone...");
            yield return new WaitForSeconds(waitTime);
            storedStones++; // Add stone to factory storage immediately.
            UpdateResourceCounterUI();

            // Schedule product conversion after delay.
            StartCoroutine(DelayedProductConversion());

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
            Debug.Log("UnitProduction: Reached forest. Collecting tree...");
            yield return new WaitForSeconds(waitTime);
            forestManager.RemoveNearestTree(transform.position);

            // Deliver tree to factory.
            Debug.Log("UnitProduction: Heading back to factory.");
            currentTarget = factory;
            agent.SetDestination(currentTarget.position);
            yield return new WaitUntil(() => HasReachedTarget());

            Debug.Log("UnitProduction: Reached factory. Delivering tree...");
            yield return new WaitForSeconds(waitTime);
            storedTrees++; // Add tree to factory storage immediately.
            UpdateResourceCounterUI();

            // Schedule product conversion after delay.
            StartCoroutine(DelayedProductConversion());
        }

        Debug.Log("UnitProduction: Production finished. No more resources.");
        isProducing = false;
    }

    private IEnumerator DelayedProductConversion()
    {
        yield return new WaitForSeconds(conversionDelay); // Wait for the delay.
        TryProduceProduct(); // Attempt to produce a product.
    }

    private void TryProduceProduct()
    {
        if (storedStones > 0 && storedTrees > 0) // Check if both resources are available.
        {
            storedStones--;
            storedTrees--;
            totalProducts++;
            Debug.Log($"UnitProduction: Product produced. Total products: {totalProducts}");
            UpdateResourceCounterUI();
            UpdateProductCounterUI();
        }
        else
        {
            Debug.Log("UnitProduction: Not enough resources to produce a product.");
        }
    }

    private void UpdateResourceCounterUI()
    {
        if (stoneCounterText != null)
        {
            stoneCounterText.text = $"Камни: {storedStones}";
        }
        if (treeCounterText != null)
        {
            treeCounterText.text = $"Деревья: {storedTrees}";
        }
    }

    private void UpdateProductCounterUI()
    {
        if (productCounterText != null)
        {
            productCounterText.text = $"Товары: {totalProducts}";
        }
    }

    private bool HasReachedTarget()
    {
        bool hasReached = !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
        Debug.Log($"UnitProduction: Has reached target? {hasReached}");
        return hasReached;
    }
}
