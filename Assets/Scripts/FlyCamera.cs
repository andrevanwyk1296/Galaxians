using UnityEngine;
using UnityEngine.InputSystem;

public class FlyCamera : MonoBehaviour
{
    public float moveSpeed = 20f;
    public float fastSpeed = 60f;
    public float mouseSensitivity = 2f;

    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        var mouse = Mouse.current;
        var keyboard = Keyboard.current;
        if (mouse == null || keyboard == null) return;

        // Mouse look
        Vector2 mouseDelta = mouse.delta.ReadValue();
        rotationX += mouseDelta.x * mouseSensitivity;
        rotationY -= mouseDelta.y * mouseSensitivity;
        rotationY = Mathf.Clamp(rotationY, -90f, 90f);
        transform.rotation = Quaternion.Euler(rotationY, rotationX, 0);

        // Movement
        float speed = keyboard.leftShiftKey.isPressed ? fastSpeed : moveSpeed;
        Vector3 move = Vector3.zero;

        if (keyboard.wKey.isPressed) move += Vector3.forward;
        if (keyboard.sKey.isPressed) move += Vector3.back;
        if (keyboard.aKey.isPressed) move += Vector3.left;
        if (keyboard.dKey.isPressed) move += Vector3.right;
        if (keyboard.eKey.isPressed) move += Vector3.up;
        if (keyboard.qKey.isPressed) move += Vector3.down;

        transform.Translate(move * speed * Time.deltaTime);

        // Unlock cursor with Escape
        if (keyboard.escapeKey.isPressed)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}