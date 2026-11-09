using UnityEngine;
using System;
using System.Collections.Generic;

// ✅ 1. Tambahkan Enum dan Struct ini (Wajib ada di file ini)
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

    // ✅ 2. PERBAIKAN: Heal mengembalikan int (jumlah HP yang naik) sesuai P5 Slide 26
    public int Heal(int amount)
    {
        int before = CurrentHP;
        CurrentHP = Mathf.Min(MaxHP, CurrentHP + amount);
        Changed?.Invoke();
        return CurrentHP - before;
    }

    // ✅ 3. PERBAIKAN: GainExp mengembalikan IEnumerable<GrowthEvent> (P5 Slide 25)
    public IEnumerable<GrowthEvent> GainExp(int amount)
    {
        CurrentEXP += amount;

        while (CurrentEXP >= ExpToNext)
        {
            CurrentEXP -= ExpToNext;
            Level++;
            
            // Simpan data lama SEBELUM berubah, agar animasi evolusi tahu "From" dan "To"
            MonsterData oldData = this.Data;
            CurrentHP = MaxHP;  // Full heal saat level up

            // Yield event Level Up
            yield return new GrowthEvent
            {
                Type = GrowthType.LevelUp,
                Message = $"{oldData.DisplayName} naik ke Lv {Level}!",
                From = oldData,
                To = oldData
            };

            // Cek evolusi
            if (oldData.EvolvesTo != null && Level >= oldData.EvolveLevel)
            {
                this.Data = oldData.EvolvesTo; // Ubah data jadi bentuk evolusi
                CurrentHP = MaxHP;

                // Yield event Evolve
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