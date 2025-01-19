using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float moveSpeed = 50f; // Debug value: 200.
    public float scrollSpeed = 0.4f; // Debug value: 3.
    
    // XYZ limits.
    public float minX = -20f, maxX = 480f;
    public float minY = 20f, maxY = 250f;
    public float minZ = -70f, maxZ = 450f;
    
    public float mouseSensitivity = 5f;

    void Update()
    {
        // Horizontal moving.
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontal, 0, vertical) * moveSpeed * Time.deltaTime;
        transform.Translate(movement, Space.World);

        //  Moving side and forward by mouse.
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseZ = Input.GetAxis("Mouse Y");
            
            // Inverting mouseX and mouseZ for right movement.
            Vector3 mouseMovement = new Vector3(-mouseX, 0, -mouseZ) * mouseSensitivity;
            transform.Translate(mouseMovement, Space.World);
        }

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