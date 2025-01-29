using UnityEngine;
using UnityEngine.AI;

public class UnitSelection : MonoBehaviour
{
    private Renderer unitRenderer;
    private Color originalColor;
    [SerializeField] private Color selectedColor = Color.green; // Цвет при выборе юнита
    private bool isSelected = false;

    void Start()
    {
        unitRenderer = GetComponent<Renderer>();
        originalColor = unitRenderer.material.color;
    }

    void Update()
    {
        // Если юнит выбран, можно добавить действия (например, выделение или передвижение)
        if (isSelected && Input.GetMouseButtonDown(1)) // Правая кнопка мыши
        {
            HandleRightClick();
        }
    }

    void OnMouseDown()
    {
        // Выбор юнита по клику
        SelectUnit();
    }

    void OnMouseExit()
    {
        // Если юнит больше не под указателем мыши, можно визуально убрать выделение
        if (!isSelected)
        {
            DeselectUnit();
        }
    }

    private void SelectUnit()
    {
        isSelected = true;
        unitRenderer.material.color = selectedColor;
        Debug.Log($"{name} selected.");
    }

    private void DeselectUnit()
    {
        isSelected = false;
        unitRenderer.material.color = originalColor;
        Debug.Log($"{name} deselected.");
    }

    private void HandleRightClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Логика для выполнения действия юнитом
            Debug.Log($"{name} received right-click command to move to {hit.point}.");
            BaseUnit unit = GetComponent<BaseUnit>();
            if (unit)
            {
                // Например, перемещение юнита
                unit.GetComponent<NavMeshAgent>().SetDestination(hit.point);
            }
        }
    }

    public void ForceDeselect()
    {
        // Принудительная отмена выбора юнита
        DeselectUnit();
    }
}