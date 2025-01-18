using UnityEngine;

public class CamControl : MonoBehaviour
{
    public float moveSpeed = 10f;    // Скорость движения камеры
    public float scrollSpeed = 2f;   // Скорость масштабирования
    public float minHeight = 20f;    // Минимальная высота
    public float maxHeight = 100f;   // Максимальная высота

    void Update()
    {
        // Управление движением камеры
        float horizontal = Input.GetAxis("Horizontal");   // Влево/вправ
        float vertical = Input.GetAxis("Vertical");       // Вверх/вниз
        transform.Translate(new Vector3(horizontal, 0, vertical) * moveSpeed * Time.deltaTime);

        // Управление масштабированием (вверх/вниз)
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            Vector3 position = transform.position;
            position.y -= scroll * scrollSpeed * 100f;  // Управление по оси Y
            position.y = Mathf.Clamp(position.y, minHeight, maxHeight);  // Ограничения высоты
            transform.position = position;
        }
    }
}
