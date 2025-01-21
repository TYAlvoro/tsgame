using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float moveSpeed = 50f; // Debug value: 200.
    public float scrollSpeed = 0.4f; // Debug value: 3.
    
    // XYZ limits.
    public float minX = -20f, maxX = 480f;
    public float minY = 20f, maxY = 250f;
    public float minZ = -70f, maxZ = 450f;
    
    public float mouseSensitivity = 5f; // Debug value: 10.

    // Rotation speeds.
    public float rotationSpeed = 50f;
    public float mouseRotationSpeed = 2f;

    private Vector3 targetPosition; // Target position for smooth movement.

    void Start()
    {
        targetPosition = transform.position; // Initialize target position.
    }

    void Update()
    {
        // Keyboard movement (WASD or arrow keys).
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        MoveCamera(horizontal, vertical);
        
        RotateCamera();
    }

    /// <summary>
    /// Moving camera in right way.
    /// </summary>
    /// <param name="horizontal">Horizontal axis for moving.</param>
    /// <param name="vertical">Vertical axis for moving.</param>
    private void MoveCamera(float horizontal, float vertical)
    {
        Vector3 movement = (transform.right * horizontal + transform.forward * vertical) * moveSpeed * Time.deltaTime;
        movement.y = 0;

        // Mouse movement (right mouse button).
        if (Input.GetMouseButton(1)) // Right mouse button is held.
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            
            // Invert mouseX and mouseZ for correct direction.
            Vector3 mouseMovement = (transform.right * -mouseX + transform.forward * -mouseY) * mouseSensitivity;
            movement += mouseMovement;
            movement.y = 0;
        }

        // Update target position and clamp it within limits.
        targetPosition += movement;
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX); // Clamp X-axis.
        targetPosition.z = Mathf.Clamp(targetPosition.z, minZ, maxZ); // Clamp Z-axis.

        // Mouse scroll for height adjustment.
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            targetPosition.y -= scroll * scrollSpeed * 100f;
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY); // Clamp Y-axis.
        }

        // Smooth movement to target position.
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10f);
    }
    
    /// <summary>
    /// Rotate camera in right way.
    /// </summary>
    private void RotateCamera()
    {
        float rotationInput = 0f;
        if (Input.GetKey(KeyCode.Q)) rotationInput = -1f;
        if (Input.GetKey(KeyCode.E)) rotationInput = 1f;

        if (rotationInput != 0)
        {
            transform.Rotate(0f, rotationInput * rotationSpeed * Time.deltaTime, 0f, Space.World);
        }

        if (Input.GetMouseButton(2))
        {
            float mouseX = Input.GetAxis("Mouse X");
            transform.Rotate(Vector3.up, mouseX * mouseRotationSpeed, Space.World);
            
            // Obsolete: float mouseY = Input.GetAxis("Mouse Y");
            // Obsolete: transform.Rotate(Vector3.right, -mouseY * mouseRotationSpeed, Space.Self);
        }
    }
}
