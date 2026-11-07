using UnityEngine;
using System;

public enum BattleResult { Win, Lose, Flee, Escape }

public class BattleSession
{
    public const float SafeSeconds = 2.2f;
    private Action<BattleResult> onFinished;
    private float safeUntil;

    public MonsterInstance Enemy { get; private set; }
    public bool IsBoss { get; private set; }
    public bool IsSafe => Time.time < safeUntil;
    public event Action<BattleResult, bool> Ended;
    public bool Escaped { get; private set; }
    public int Potions { get; private set; } = 3;

    public void Begin(MonsterInstance enemy, bool isBoss)
    {
        Enemy = enemy;
        IsBoss = isBoss;
    }

    public void SetFinishCallback(Action<BattleResult> callback)
    {
        onFinished = callback;
    }

    public void UseItem()
    {
        if (Potions > 0)
        {
            Potions--;
            Debug.Log($"Potion digunakan. Sisa: {Potions}");
        }
        else
        {
            Debug.Log("Tidak ada potion tersisa!");
        }
    }

    public void TryEscape()
    {
        if (IsBoss)
        {
            Debug.Log("Tidak bisa kabur dari boss!");
            return;
        }

        Escaped = true;
        Finish(BattleResult.Flee);
    }

    public void Finish(BattleResult result)
    {
        Action<BattleResult> callback = onFinished;
        bool wasBoss = IsBoss;
        
        onFinished = null;
        Enemy = null;
        IsBoss = false;
        safeUntil = Time.time + SafeSeconds;
        
        callback?.Invoke(result);
        Ended?.Invoke(result, wasBoss);
    }
}