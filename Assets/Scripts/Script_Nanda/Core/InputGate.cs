using UnityEngine;

public class InputGate : IMoveInput, IActionInput
{
    private readonly IMoveInput moveInput;
    private readonly IActionInput actionInput;
    public bool IsLocked { get; private set; }

    public InputGate(IMoveInput moveInput, IActionInput actionInput)
    {
        this.moveInput = moveInput;
        this.actionInput = actionInput;
    }

    public Vector2 Move => IsLocked ? Vector2.zero : moveInput.Move;
    public bool IsRunning => !IsLocked && moveInput.IsRunning;
    public bool InteractPressed => !IsLocked && actionInput.InteractPressed;

    public void Lock() => IsLocked = true;
    public void Unlock() => IsLocked = false;
}