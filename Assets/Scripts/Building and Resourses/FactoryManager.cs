using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FactoryManager : MonoBehaviour
{
    [SerializeField] private GameObject factoryMenu; // Панель UI фабрики
    [SerializeField] private Button startProductionButton; // Кнопка запуска производства
    [SerializeField] private TMP_Text productCounterText; // Счетчик товаров
    [SerializeField] private TMP_Text stoneCounterText; // Счетчик камней
    [SerializeField] private TMP_Text treeCounterText; // Счетчик деревьев

    private int totalProducts = 0;
    private int storedStones = 0;
    private int storedTrees = 0;

    private void Start()
    {
        factoryMenu.SetActive(false); // Отключаем меню фабрики по умолчанию
        startProductionButton.onClick.AddListener(StartProductionForAllUnits); // Привязываем метод к кнопке
        UpdateUI();
    }

    private void OnMouseDown()
    {
        // Логика для активации UI при клике на фабрику
        if (factoryMenu != null)
        {
            factoryMenu.SetActive(!factoryMenu.activeSelf);
        }
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

    private void StartProductionForAllUnits()
    {
        var productionUnits = FindObjectsOfType<ProductionUnit>();
        foreach (var unit in productionUnits)
        {
            unit.PerformAction(); // Запуск цикла производства у юнитов
        }

        Debug.Log("FactoryManager: All production units started.");
        factoryMenu.SetActive(false); // Скрываем меню после запуска
    }
}
