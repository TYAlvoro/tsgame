using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float moveSpeed = 50f; // Debug value: 200.
    public float scrollSpeed = 0.4f; // Debug value: 3.
    
    // XYZ limits.
    public float minX = -20f, maxX = 480f;
    public float minY = 20f, maxY = 250f;
    public float minZ = -70f, maxZ = 450f;

    void Update()
    {
        // Horizontal moving.
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontal, 0, vertical) * moveSpeed * Time.deltaTime;
        transform.Translate(movement, Space.World);

        // Clamping X and Z.
        Vector3 position = transform.position;
        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.z = Mathf.Clamp(position.z, minZ, maxZ);

        // Y Mouse Scrolling.
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            position.y -= scroll * scrollSpeed * 100f;
        }

        // Clamping Y.
        position.y = Mathf.Clamp(position.y, minY, maxY);

        // Transforming.
        transform.position = position;
    }
}