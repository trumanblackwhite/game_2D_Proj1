using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3.75f;
    
    private InputAction moveAction;
    private Vector2 moveInput;
    private Rigidbody2D rb;

    void Start()
    {
        // Mencari action "Move" dari Input System bawaan
        moveAction = InputSystem.actions.FindAction("Move");
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Membaca Keyboard
        moveInput = moveAction.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        // Menggerakkan posisi karakter berdasarkan input
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, moveInput.y * moveSpeed);
    }
}