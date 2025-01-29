using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : MonoBehaviour
{
    private NavMeshAgent agent; // Компонент для управления передвижением
    private bool isSelected = false; // Состояние выбора юнита

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError($"{name} missing NavMeshAgent component!");
        }
    }

    void Update()
    {
        // Если юнит выбран и происходит клик правой кнопкой мыши
        if (isSelected && Input.GetMouseButtonDown(1)) // Правая кнопка мыши
        {
            MoveToCursor();
        }
    }

    void OnMouseDown()
    {
        // Выделяем юнита при клике
        SelectUnit();
    }

    private void MoveToCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Устанавливаем цель для передвижения
            agent.SetDestination(hit.point);
            Debug.Log($"{name} moving to {hit.point}.");
        }
    }

    private void SelectUnit()
    {
        isSelected = true;
        Debug.Log($"{name} selected for movement.");
    }

    public void DeselectUnit()
    {
        // Снимаем выделение юнита
        isSelected = false;
        Debug.Log($"{name} deselected.");
    }

    public void MoveToPosition(Vector3 position)
    {
        // Метод для принудительного задания цели через код
        if (agent != null)
        {
            agent.SetDestination(position);
            Debug.Log($"{name} moving to {position} by script command.");
        }
    }

    private bool HasReachedTarget()
    {
        // Проверяем, достиг ли юнит цели
        return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
    }

    public bool IsMoving()
    {
        // Возвращает true, если юнит в движении
        return agent != null && agent.remainingDistance > agent.stoppingDistance;
    }
}