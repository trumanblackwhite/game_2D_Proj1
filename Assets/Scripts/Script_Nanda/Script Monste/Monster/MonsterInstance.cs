using UnityEngine;
using System;

public class MonsterInstance : IDamageable
{
    public MonsterData Data { get; private set; }
    public int Level { get; private set; }
    public int CurrentHP { get; private set; }
    public int CurrentEXP { get; private set; }

    public event Action Changed;  // Untuk HUD di P5

    // Formula scaling
    private float Scale => 1f + (Level - 1) * 0.1f;

    // Stat yang dihitung dari base + level
    public int MaxHP => Mathf.RoundToInt(Data.BaseHP * Scale) + Level * 2;
    public int Attack => Mathf.RoundToInt(Data.BaseATK * Scale);
    public int Defense => Mathf.RoundToInt(Data.BaseDEF * Scale);
    public int Speed => Mathf.RoundToInt(Data.BaseSPD * Scale);
    public int ExpToNext => Level * 10;

    // Constructor
    public MonsterInstance(MonsterData data, int level)
    {
        Data = data;
        Level = level;
        CurrentHP = MaxHP;
        CurrentEXP = 0;
    }

    // Interface IDamageable
    public void TakeDamage(int amount)
    {
        CurrentHP = Mathf.Max(0, CurrentHP - amount);
        Changed?.Invoke();
    }

    public void Heal(int amount)
    {   
        CurrentHP = Mathf.Min(MaxHP, CurrentHP + amount);
        Changed?.Invoke();
    }

    // Gain EXP, cek level up dan evolusi
    public void GainExp(int amount)
    {
        CurrentEXP += amount;

        while (CurrentEXP >= ExpToNext)
        {
            CurrentEXP -= ExpToNext;
            Level++;
            CurrentHP = MaxHP;  // Full heal saat level up
            Debug.Log($"{Data.DisplayName} naik ke Lv {Level}!");

            // Cek evolusi
            if (Data.EvolvesTo != null && Level >= Data.EvolveLevel)
            {
                Data = Data.EvolvesTo;
                CurrentHP = MaxHP;
                Debug.Log($"{Data.DisplayName} berevolusi menjadi {Data.DisplayName}!");
            }
        }

        Changed?.Invoke();
    }
}