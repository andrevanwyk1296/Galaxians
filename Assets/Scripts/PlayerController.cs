using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float jumpForce = 8f;
    public float gravity = -20f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        controller.height = 1.8f;
        controller.radius = 0.4f;
        controller.center = new Vector3(0, 0.9f, 0);

        // Collision stability tweaks
        controller.stepOffset = 0.1f;
        controller.skinWidth = 0.05f;
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        // Grounded check
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // Movement input
        Vector3 move = Vector3.zero;
        if (keyboard[Key.W].isPressed) move += transform.forward;
        if (keyboard[Key.S].isPressed) move -= transform.forward;
        if (keyboard[Key.A].isPressed) move -= transform.right;
        if (keyboard[Key.D].isPressed) move += transform.right;

        move = move.normalized * walkSpeed;

        // Jump
        if (keyboard[Key.Space].wasPressedThisFrame && isGrounded)
            velocity.y = jumpForce;

        // Gravity
        velocity.y += gravity * Time.deltaTime;

        // Single Move call
        Vector3 finalMove = (move + Vector3.up * velocity.y) * Time.deltaTime;
        CollisionFlags flags = controller.Move(finalMove);

        // Debug collision flags
        if (flags != CollisionFlags.None)
            Debug.Log("Collision flags: " + flags);
    }
}
