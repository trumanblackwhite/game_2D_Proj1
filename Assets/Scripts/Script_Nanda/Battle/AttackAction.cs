using System.Collections;
using UnityEngine;

public class AttackAction : BattleAction
{
    public override IEnumerator Execute(BattleSystem battle, MonsterInstance user, MonsterInstance target)
    {
        // Prefix "Lawan " jika user adalah enemy dan bukan boss (P5 Slide 19)
        string who = (user == battle.Enemy && !battle.IsBoss ? "Lawan " : "") + user.Data.DisplayName;

        // 1. Tampilkan pesan + tunggu selesai
        yield return battle.Say($"{who} menyerang!", 0.7f);

        // 2. Jeda untuk animasi terjang
        yield return new WaitForSeconds(0.2f);

        // 3. Hitung & terapkan damage
        int dmg = DamageFormula.Calculate(user, target, 14f);
        target.TakeDamage(dmg);

        // 4. KUNCI: Kasih waktu agar MonsterPanel sempat melakukan Lerp HP
        yield return new WaitForSeconds(0.45f);
    }
}