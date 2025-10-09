using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class SimpleCameraController2D : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 10f;

    [Header("Zoom")]
    public float zoomSpeed = 5f;
    public float minZoom = 2f;
    public float maxZoom = 20f;

    private Camera cam;
    private Vector2 moveInput;
    private float zoomInput;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true;
    }

    private void Update()
    {
        // Move with WASD or arrow keys
        Vector3 move = new Vector3(moveInput.x, moveInput.y, 0f) * moveSpeed * Time.deltaTime;
        transform.position += move;

        // Zoom with mouse wheel
        if (Mathf.Abs(zoomInput) > 0.001f)
        {
            float targetZoom = cam.orthographicSize - zoomInput * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(targetZoom, minZoom, maxZoom);
            zoomInput = 0f; // reset scroll input
        }
    }

    // --- Input System Callbacks ---
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        zoomInput = context.ReadValue<float>();
    }
}
