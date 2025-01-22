using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class UnitProduction : MonoBehaviour
{
    public Transform factory; // Ссылка на фабрику
    public ForestManager forestManager; // Лесной менеджер
    public QuarryManager quarryManager; // Каменный карьер
    public FactoryManager factoryManager; // Ссылка на FactoryManager

    public float waitTime = 2f; // Время ожидания на каждом объекте
    public float deliveryDelay = 2f; // Задержка перед обновлением счетчиков ресурсов
    public float productCreationDelay = 3f; // Задержка перед созданием товара

    private NavMeshAgent agent; // NavMeshAgent для движения юнита
    private Transform currentTarget; // Текущая цель
    private bool isProducing = false; // Состояние производства

    private void Start()
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
            // Сбор камней
            currentTarget = quarryManager.GetNearestQuarry(transform.position);
            if (currentTarget == null) break;

            agent.SetDestination(currentTarget.position);
            yield return new WaitUntil(() => HasReachedTarget());
            yield return new WaitForSeconds(waitTime);
            quarryManager.RemoveNearestStone(transform.position);

            // Доставка камней на фабрику
            currentTarget = factory;
            agent.SetDestination(currentTarget.position);
            yield return new WaitUntil(() => HasReachedTarget());
            yield return new WaitForSeconds(deliveryDelay);

            // Обновляем счетчик камней
            factoryManager.AddStone();

            // Сбор деревьев
            currentTarget = forestManager.GetNearestTree(transform.position);
            if (currentTarget == null) break;

            agent.SetDestination(currentTarget.position);
            yield return new WaitUntil(() => HasReachedTarget());
            yield return new WaitForSeconds(waitTime);
            forestManager.RemoveNearestTree(transform.position);

            // Доставка деревьев на фабрику
            currentTarget = factory;
            agent.SetDestination(currentTarget.position);
            yield return new WaitUntil(() => HasReachedTarget());
            yield return new WaitForSeconds(deliveryDelay);

            // Обновляем счетчик деревьев
            factoryManager.AddTree();

            // Запускаем производство товара через заданное время
            StartCoroutine(DelayedProductCreation());
        }

        Debug.Log("UnitProduction: Production finished. No more resources.");
        isProducing = false;
    }

    private IEnumerator DelayedProductCreation()
    {
        yield return new WaitForSeconds(productCreationDelay);
        factoryManager.TryProduceProduct();
    }

    private bool HasReachedTarget()
    {
        return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
    }
}
