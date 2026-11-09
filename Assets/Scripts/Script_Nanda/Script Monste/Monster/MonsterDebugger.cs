// Kode_P6 — Alat uji MonsterInstance tanpa battle. Klik titik tiga pada component saat Play. Ditambahkan di P4.
using UnityEngine;

public class MonsterDebugger : MonoBehaviour
{
    [SerializeField] private MonsterData species;
    [SerializeField] private int level = 3;

    private MonsterInstance monster;

    [ContextMenu("1. Buat monster")]
    private void Create()
    {
        monster = new MonsterInstance(species, level);
        Log();
    }

    [ContextMenu("2. Terima 20 damage")]
    private void Damage()
    {
        monster.TakeDamage(20);
        Log();
    }

    [ContextMenu("3. Beri 100 EXP")]
    private void GiveExp()
    {
        foreach (GrowthEvent growth in monster.GainExp(100)) Debug.Log(growth.Message);
        Log();
    }

    private void Log() =>
        Debug.Log($"{monster.Data.DisplayName} Lv {monster.Level} | HP {monster.CurrentHP}/{monster.MaxHP} | " +
                  $"ATK {monster.Attack} DEF {monster.Defense} SPD {monster.Speed} | EXP {monster.Exp}/{monster.ExpToNext}");
}
