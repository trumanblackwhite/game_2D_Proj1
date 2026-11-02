using UnityEngine;

public class MonsterDebugger : MonoBehaviour
{
    [SerializeField] private MonsterData species;
    [SerializeField] private int level = 3;

    private MonsterInstance monster;

    [ContextMenu("1. Buat Monster")]
    private void CreateMonster()
    {
        if (species == null)
        {
            Debug.LogError("Species belum di-set!");
            return;
        }

        monster = new MonsterInstance(species, level);
        Debug.Log($"Dibuat: {monster.Data.DisplayName} Lv {monster.Level}");
        Debug.Log($"HP: {monster.CurrentHP}/{monster.MaxHP}");
        Debug.Log($"ATK: {monster.Attack}, DEF: {monster.Defense}, SPD: {monster.Speed}");
        Debug.Log($"EXP to next: {monster.ExpToNext}");
    }

    [ContextMenu("2. Lihat Status")]
    private void ShowStatus()
    {
        if (monster == null)
        {
            Debug.LogError("Monster belum dibuat! Klik '1. Buat Monster' dulu.");
            return;
        }

        Debug.Log($"=== {monster.Data.DisplayName} Lv {monster.Level} ===");
        Debug.Log($"HP: {monster.CurrentHP}/{monster.MaxHP}");
        Debug.Log($"EXP: {monster.CurrentEXP}/{monster.ExpToNext}");
        Debug.Log($"ATK: {monster.Attack}, DEF: {monster.Defense}, SPD: {monster.Speed}");
    }

    [ContextMenu("3. Beri 100 EXP")]
    private void GiveExp()
    {
        if (monster == null)
        {
            Debug.LogError("Monster belum dibuat!");
            return;
        }

        Debug.Log($"--- Memberi 100 EXP ---");
        monster.GainExp(100);
        ShowStatus();
    }

    [ContextMenu("4. Beri Damage 20")]
    private void TakeDamage()
    {
        if (monster == null)
        {
            Debug.LogError("Monster belum dibuat!");
            return;
        }

        Debug.Log($"--- Menerima 20 damage ---");
        monster.TakeDamage(20);
        ShowStatus();
    }
}