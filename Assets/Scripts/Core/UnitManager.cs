using UnityEngine;

public class UnitManager : MonoBehaviour
{
    [SerializeField] private ForestManager forestManager;
    [SerializeField] private QuarryManager quarryManager;
    [SerializeField] private FactoryManager factoryManager;

    private void Start()
    {
        InitializeUnits();
    }

    private void InitializeUnits()
    {
        // Находим все юниты типа ProductionUnit на сцене
        var productionUnits = FindObjectsOfType<ProductionUnit>();

        foreach (var unit in productionUnits)
        {
            // Назначаем ссылки на менеджеры
            unit.forestManager = forestManager;
            unit.quarryManager = quarryManager;
            unit.factoryManager = factoryManager;

            Debug.Log($"Unit {unit.name} initialized with managers.");
        }

        if (productionUnits.Length == 0)
        {
            Debug.LogWarning("No ProductionUnits found in the scene!");
        }
    }
}