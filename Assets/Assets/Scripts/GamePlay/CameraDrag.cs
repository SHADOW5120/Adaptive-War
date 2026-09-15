using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class CameraDrag : MonoBehaviour
{
    [Header("Camera Bounds")]
    public float minX = -15f;
    public float maxX = 15f;
    public float minY = -10f;
    public float maxY = 10f;

    private Vector3 dragOrigin;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        // Stop panning if the player is currently trying to place a unit
        if (UnitSpawner2D.IsPlacingUnit) return;

        // Ensure we don't start dragging if the player is just tapping a UI button
        if (Pointer.current.press.wasPressedThisFrame && EventSystem.current.IsPointerOverGameObject()) return;

        // PC LOGIC: Middle Mouse Button
        if (Mouse.current != null)
        {
            if (Mouse.current.middleButton.wasPressedThisFrame)
            {
                dragOrigin = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            }
            if (Mouse.current.middleButton.isPressed)
            {
                Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                MoveCamera(difference);
                return; // Prevent running mobile logic simultaneously on PC
            }
        }

        // MOBILE LOGIC: Single Finger Swipe
        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;

            if (touch.press.wasPressedThisFrame)
            {
                dragOrigin = cam.ScreenToWorldPoint(touch.position.ReadValue());
            }
            if (touch.press.isPressed)
            {
                Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(touch.position.ReadValue());
                MoveCamera(difference);
            }
        }
    }

    private void MoveCamera(Vector3 difference)
    {
        Vector3 targetPosition = cam.transform.position + difference;

        float clampedX = Mathf.Clamp(targetPosition.x, minX, maxX);
        float clampedY = Mathf.Clamp(targetPosition.y, minY, maxY);

        cam.transform.position = new Vector3(clampedX, clampedY, targetPosition.z);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        float centerX = (minX + maxX) / 2f;
        float centerY = (minY + maxY) / 2f;
        Vector3 center = new Vector3(centerX, centerY, transform.position.z);
        Vector3 size = new Vector3(maxX - minX, maxY - minY, 0.1f);
        Gizmos.DrawWireCube(center, size);
    }
}