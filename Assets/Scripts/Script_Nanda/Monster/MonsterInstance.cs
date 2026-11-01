using System;
using UnityEngine;

public class MonsterInstance : IDamageable
{
    public MonsterData Data { get; private set; }
    public int Level { get; private set; }
    public int CurrentHP { get; private set; }

    public event Action Changed;

    // Kalkulasi skala berdasarkan level
    private float Scale => 1f + (Level - 1) * 0.1f;

    public int MaxHP => Mathf.RoundToInt(Data.BaseHP * Scale) + Level * 2;
    public int Attack => Mathf.RoundToInt(Data.BaseATK * Scale);
    public int Defense => Mathf.RoundToInt(Data.BaseDEF * Scale);
    public int Speed => Mathf.RoundToInt(Data.BaseSPD * Scale);
    
    public int ExpToNext => Level * 10;
    public int CurrentExp { get; private set; }

    // Konstruktor untuk membuat instance monster baru
    public MonsterInstance(MonsterData data, int level)
    {
        Data = data;
        Level = level;
        CurrentHP = MaxHP;
    }

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

    public void GainExp(int amount)
    {
        CurrentExp += amount;
        while (CurrentExp >= ExpToNext)
        {
            CurrentExp -= ExpToNext;
            Level++;
            CurrentHP = MaxHP; // Pemulihan penuh saat naik level

            // Cek evolusi jika mencapai level evolusi
            if (Data.EvolvesTo != null && Level >= Data.EvolveLevel)
            {
                Debug.Log($"{Data.DisplayName} berevolusi menjadi {Data.EvolvesTo.DisplayName}!");
                Data = Data.EvolvesTo;
            }
        }
        Changed?.Invoke();
    }
}