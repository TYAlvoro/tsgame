using UnityEngine;

public class Chunk : MonoBehaviour
{
    public Vector2Int chunkCoordinates; // ���������� ����� � �����
    public bool isActive = false; // ������� �� ����

    /// <summary>
    /// �������������� ����
    /// </summary>
    public void Initialize(Vector2Int coordinates)
    {
        chunkCoordinates = coordinates;
    }

    /// <summary>
    /// ���������� ����
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
    /// ������������ ����
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