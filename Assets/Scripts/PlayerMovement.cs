using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    private InputAction moveAction; 

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
    }

    void Update()
    {
        Vector2 direction = Vector2.right;
        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }
}