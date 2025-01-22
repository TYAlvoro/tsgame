using UnityEngine;

public class UnitManager : MonoBehaviour
{
    [SerializeField] private Transform factory; // Ссылка на фабрику
    [SerializeField] private ForestManager forestManager; // Лесной менеджер
    [SerializeField] private QuarryManager quarryManager; // Каменный карьер
    [SerializeField] private FactoryManager factoryManager; // Ссылка на FactoryManager

    private void Start()
    {
        InitializeUnits();
    }

    private void InitializeUnits()
    {
        UnitProduction[] units = FindObjectsOfType<UnitProduction>();
        foreach (var unit in units)
        {
            unit.factory = factory;
            unit.forestManager = forestManager;
            unit.quarryManager = quarryManager;
            unit.factoryManager = factoryManager;
            Debug.Log($"Unit {unit.name} initialized.");
        }
    }
}