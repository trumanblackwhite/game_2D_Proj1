using UnityEngine;
using UnityEngine.InputSystem;
public interface IMoveInput
{
    Vector2 Move { get; }
    bool IsRunning { get; }
}
public class InputSystemMoveInput : IMoveInput
{
    private readonly InputAction move;
    private readonly InputAction sprint;
    public InputSystemMoveInput()
    {
        move = InputSystem.actions.FindAction("Move");
        sprint = InputSystem.actions.FindAction("Sprint");
    }
    public Vector2 Move => move.ReadValue<Vector2>();
    public bool IsRunning => sprint != null && sprint.IsPressed();
}