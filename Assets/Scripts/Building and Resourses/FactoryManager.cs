using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class FactoryManager : MonoBehaviour
{
    [SerializeField] private GameObject factoryMenu; // Меню фабрики
    [SerializeField] private Button startProductionButton; // Кнопка запуска производства
    [SerializeField] private TMP_Text productCounterText; // UI элемент для товаров
    [SerializeField] private TMP_Text stoneCounterText; // UI элемент для камней
    [SerializeField] private TMP_Text treeCounterText; // UI элемент для деревьев

    private int totalProducts = 0; // Общее количество товаров
    private int storedStones = 0; // Количество камней
    private int storedTrees = 0; // Количество деревьев
    private bool isMenuActive = false; // Состояние меню

    private void Start()
    {
        factoryMenu.SetActive(false); // Скрываем меню изначально
        startProductionButton.onClick.AddListener(StartAllUnits);
        UpdateUI();
    }

    private void Update()
    {
        // Обработка открытия/закрытия меню при клике мыши
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            if (IsMouseOverFactory())
            {
                ToggleMenu(true);
            }
            else if (isMenuActive)
            {
                ToggleMenu(false);
            }
        }
    }

    private bool IsMouseOverFactory()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.collider.gameObject == gameObject;
        }
        return false;
    }

    private void ToggleMenu(bool show)
    {
        factoryMenu.SetActive(show);
        isMenuActive = show;
        Debug.Log(show ? "Factory menu opened." : "Factory menu closed.");
    }

    public void AddStone()
    {
        storedStones++;
        UpdateUI();
    }

    public void AddTree()
    {
        storedTrees++;
        UpdateUI();
    }

    public void TryProduceProduct()
    {
        if (storedStones > 0 && storedTrees > 0)
        {
            storedStones--;
            storedTrees--;
            totalProducts++;
            Debug.Log($"FactoryManager: Product produced. Total products: {totalProducts}");
            UpdateUI();
        }
        else
        {
            Debug.Log("FactoryManager: Not enough resources to produce a product.");
        }
    }

    private void UpdateUI()
    {
        if (productCounterText != null)
        {
            productCounterText.text = $"Товары: {totalProducts}";
        }
        if (stoneCounterText != null)
        {
            stoneCounterText.text = $"Камни: {storedStones}";
        }
        if (treeCounterText != null)
        {
            treeCounterText.text = $"Деревья: {storedTrees}";
        }
    }

    private void StartAllUnits()
    {
        UnitProduction[] units = FindObjectsOfType<UnitProduction>();
        foreach (var unit in units)
        {
            unit.StartProduction();
        }
        Debug.Log("All units started production.");
        ToggleMenu(false); // Закрыть меню после запуска
    }
}
