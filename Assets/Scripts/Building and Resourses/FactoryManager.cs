using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FactoryManager : MonoBehaviour
{
    [SerializeField] private GameObject factoryMenu; // Factory menu canvas
    [SerializeField] private Button startProductionButton; // Start Production button
    [SerializeField] private UnitProduction unitProduction; // Reference to UnitProduction script

    private bool isMenuActive = false; // Tracks if the menu is active

    private void Start()
    {
        // Ensure menu is initially hidden
        factoryMenu.SetActive(false);

        // Assign button action
        startProductionButton.onClick.AddListener(StartProduction);
        Debug.Log("FactoryManager initialized.");
    }

    private void Update()
    {
        // Check if the left mouse button is clicked
        if (Input.GetMouseButtonDown(0))
        {
            // If the mouse is over a UI element, skip further checks
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            // If the mouse is over the factory, open the menu
            if (IsMouseOverFactory())
            {
                ToggleMenu(true);
            }
            // If the mouse is not over the factory and the menu is active, close the menu
            else if (isMenuActive)
            {
                ToggleMenu(false);
            }
        }
    }

    /// <summary>
    /// Toggles the visibility of the factory menu.
    /// </summary>
    /// <param name="show">Whether to show or hide the menu.</param>
    private void ToggleMenu(bool show)
    {
        if (show)
        {
            factoryMenu.SetActive(true);
            isMenuActive = true;
            Debug.Log("Factory menu opened.");
        }
        else
        {
            factoryMenu.SetActive(false);
            isMenuActive = false;
            Debug.Log("Factory menu closed.");
        }
    }

    /// <summary>
    /// Checks if the mouse is currently over the factory.
    /// </summary>
    /// <returns>True if the mouse is over the factory, false otherwise.</returns>
    private bool IsMouseOverFactory()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            bool isOverFactory = hit.collider.gameObject == gameObject;
            Debug.Log($"Mouse over factory: {isOverFactory}");
            return isOverFactory;
        }
        return false;
    }

    /// <summary>
    /// Starts the production process.
    /// </summary>
    private void StartProduction()
    {
        if (unitProduction != null)
        {
            unitProduction.StartProduction();
            Debug.Log("Production started.");
        }
        else
        {
            Debug.LogError("UnitProduction reference is not assigned.");
        }

        // Close the menu after starting production
        ToggleMenu(false);
    }
}
