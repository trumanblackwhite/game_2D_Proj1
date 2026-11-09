using System.Collections;
using UnityEngine;

public class ItemAction : BattleAction
{
    public override IEnumerator Execute(BattleSystem battle, MonsterInstance user, MonsterInstance target)
    {
        // 1. Cek apakah ramuan masih ada
        if (battle.Session.Potions <= 0)
        {
            yield return battle.Say("Ramuan habis!", 0.8f);
            yield break; // Hentikan eksekusi action ini
        }

        // 2. Tampilkan pesan penggunaan
        yield return battle.Say($"{user.Data.DisplayName} menggunakan Ramuan!", 0.7f);

        // 3. Kurangi stok ramuan di session
        battle.Session.UseItem();
        
        // 4. Heal 40% dari MaxHP (sesuai P5)
        int healAmount = Mathf.RoundToInt(user.MaxHP * 0.4f);
        int healed = user.Heal(healAmount); // Method Heal sekarang mengembalikan int
        
        // 5. Jeda animasi
        yield return new WaitForSeconds(0.6f);
        
        // 6. Tampilkan hasil heal
        yield return battle.Say($"HP pulih {healed}! (Sisa ramuan: {battle.Session.Potions})", 0.8f);
    }
}