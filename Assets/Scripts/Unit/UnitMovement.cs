using UnityEngine;
using UnityEngine.AI;

public class UnitController : MonoBehaviour
{
    public NavMeshAgent agent; // NavMeshAgent for the unit
    private bool isSelected = false; // Tracks if the unit is selected

    void Update()
    {
        // Check if the unit is selected
        if (isSelected)
        {
            // Handle right-click movement
            if (Input.GetMouseButtonDown(1)) // Right-click
            {
                MoveToCursor();
            }
        }

        // Deselect the unit if the user clicks elsewhere
        if (Input.GetMouseButtonDown(0) && !IsMouseOverUnit())
        {
            isSelected = false;
        }
    }

    void OnMouseDown()
    {
        // Select the unit when clicked
        isSelected = true;
    }

    private void MoveToCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            agent.SetDestination(hit.point);
        }
    }

    private bool IsMouseOverUnit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.collider.gameObject == gameObject; // Check if clicked object is this unit
        }
        return false;
    }
}
