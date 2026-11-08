using UnityEngine;
using System;
using System.Collections.Generic;

// ✅ TAMBAHKAN INI (Mengatasi Error GrowthType & GrowthEvent tidak ditemukan)
public enum GrowthType { LevelUp, Evolve }

public struct GrowthEvent
{
    public GrowthType Type;
    public string Message;
    public MonsterData From;
    public MonsterData To;
}

public class MonsterInstance : IDamageable
{
    public MonsterData Data { get; private set; }
    public int Level { get; private set; }
    public int CurrentHP { get; private set; }
    public int CurrentEXP { get; private set; }

    public event Action Changed;

    private float Scale => 1f + (Level - 1) * 0.1f;

    public int MaxHP => Mathf.RoundToInt(Data.BaseHP * Scale) + Level * 2;
    public int Attack => Mathf.RoundToInt(Data.BaseATK * Scale);
    public int Defense => Mathf.RoundToInt(Data.BaseDEF * Scale);
    public int Speed => Mathf.RoundToInt(Data.BaseSPD * Scale);
    public int ExpToNext => Level * 10;

    public MonsterInstance(MonsterData data, int level)
    {
        Data = data;
        Level = level;
        CurrentHP = MaxHP;
        CurrentEXP = 0;
    }

    public void TakeDamage(int amount)
    {
        CurrentHP = Mathf.Max(0, CurrentHP - amount);
        Changed?.Invoke();
    }

    // ✅ PERBAIKAN: Ubah dari 'void' jadi 'int' (Mengatasi Error Cannot convert void to int)
    public int Heal(int amount)
    {
        int before = CurrentHP;
        CurrentHP = Mathf.Min(MaxHP, CurrentHP + amount);
        Changed?.Invoke();
        return CurrentHP - before;
    }

    // ✅ PERBAIKAN: Ubah dari 'void' jadi 'IEnumerable<GrowthEvent>' (Mengatasi Error foreach void)
    public IEnumerable<GrowthEvent> GainExp(int amount)
    {
        CurrentEXP += amount;

        while (CurrentEXP >= ExpToNext)
        {
            CurrentEXP -= ExpToNext;
            Level++;

            MonsterData oldData = this.Data;
            CurrentHP = MaxHP;

            yield return new GrowthEvent
            {
                Type = GrowthType.LevelUp,
                Message = $"{oldData.DisplayName} naik ke Lv {Level}!",
                From = oldData,
                To = oldData
            };

            if (oldData.EvolvesTo != null && Level >= oldData.EvolveLevel)
            {
                this.Data = oldData.EvolvesTo;
                CurrentHP = MaxHP;

                yield return new GrowthEvent
                {
                    Type = GrowthType.Evolve,
                    Message = $"{oldData.DisplayName} berevolusi menjadi {this.Data.DisplayName}!",
                    From = oldData,
                    To = this.Data
                };
            }
        }

        Changed?.Invoke();
    }
}