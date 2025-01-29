using UnityEngine;

public class Chunk : MonoBehaviour
{
    public Vector2Int chunkCoordinates; // Координаты чанка в сетке
    public bool isActive = false; // Активен ли чанк

    /// <summary>
    /// Инициализирует чанк
    /// </summary>
    public void Initialize(Vector2Int coordinates)
    {
        chunkCoordinates = coordinates;
    }

    /// <summary>
    /// Активирует чанк
    /// </summary>
    public void Activate()
    {
        if (!isActive)
        {
            gameObject.SetActive(true);
            isActive = true;
        }
    }

    /// <summary>
    /// Деактивирует чанк
    /// </summary>
    public void Deactivate()
    {
        if (isActive)
        {
            gameObject.SetActive(false);
            isActive = false;
        }
    }
}