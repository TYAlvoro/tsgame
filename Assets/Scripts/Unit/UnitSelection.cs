using UnityEngine;

public class UnitSelection : MonoBehaviour
{
    private Renderer unitRenderer;
    private Color originalColor;

    void Start()
    {
        unitRenderer = GetComponent<Renderer>();
        originalColor = unitRenderer.material.color;
    }

    void OnMouseDown()
    {
        unitRenderer.material.color = Color.green; // Change color to indicate selection
    }

    void OnMouseExit()
    {
        unitRenderer.material.color = originalColor; // Revert color when deselected
    }
}
