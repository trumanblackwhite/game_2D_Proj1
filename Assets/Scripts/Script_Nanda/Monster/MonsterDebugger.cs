using UnityEngine;

public class MonsterDebugger : MonoBehaviour
{
    [Header("Referensi Monster")]
    public MonsterInstance monsterInstance;

    [ContextMenuItem("Buat monster", "InitializeMonster")]
    [ContextMenuItem("Beri 100 EXP", "GiveExp")]
    public string menuHelper = "Klik kanan garis ini / titik 3 di Inspector";

    private void InitializeMonster()
    {
        if (monsterInstance != null)
        {
            Debug.Log($"[Debugger] Monster diinisialisasi pada Level {monsterInstance.Level}");
        }
    }

    private void GiveExp()
    {
        if (monsterInstance != null)
        {
            monsterInstance.GainExp(100);
            Debug.Log($"[Debugger] Memberikan 100 EXP! Level sekarang: {monsterInstance.Level}, Current HP: {monsterInstance.CurrentHP}");
        }
        else
        {
            Debug.LogWarning("[Debugger] MonsterInstance belum di-assign ke Inspector!");
        }
    }
}