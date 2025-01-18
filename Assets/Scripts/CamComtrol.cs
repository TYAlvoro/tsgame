using UnityEngine;

public class CamControl : MonoBehaviour
{
    public float moveSpeed = 10f;    // �������� �������� ������
    public float scrollSpeed = 2f;   // �������� ���������������
    public float minHeight = 20f;    // ����������� ������
    public float maxHeight = 100f;   // ������������ ������

    void Update()
    {
        // ���������� ��������� ������
        float horizontal = Input.GetAxis("Horizontal");   // �����/�����
        float vertical = Input.GetAxis("Vertical");       // �����/����
        transform.Translate(new Vector3(horizontal, 0, vertical) * moveSpeed * Time.deltaTime);

        // ���������� ���������������� (�����/����)
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            Vector3 position = transform.position;
            position.y -= scroll * scrollSpeed * 100f;  // ���������� �� ��� Y
            position.y = Mathf.Clamp(position.y, minHeight, maxHeight);  // ����������� ������
            transform.position = position;
        }
    }
}
