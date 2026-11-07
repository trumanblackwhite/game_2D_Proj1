using UnityEngine;
using UnityEngine.InputSystem;

public class BattleDebugInput : MonoBehaviour
{
    [SerializeField] private BattleSystem battle;

    void Update()
    {
        if (battle == null || !battle.Busy) return;
        var k = Keyboard.current;
        if (k == null) return;
        if (k.digit1Key.wasPressedThisFrame) battle.Choose(0);   // Serang
        if (k.digit2Key.wasPressedThisFrame) battle.Choose(1);   // Jurus
        if (k.digit3Key.wasPressedThisFrame) battle.Choose(2);   // Ramuan
        if (k.digit4Key.wasPressedThisFrame) battle.Choose(3);   // Kabur
    }
}