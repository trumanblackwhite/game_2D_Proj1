using UnityEngine;
using UnityEngine.InputSystem;
public interface IMoveInput
{
    Vector2 Move { get; }
    bool IsRunning { get; }
}