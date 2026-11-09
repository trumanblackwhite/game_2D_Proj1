using System.Collections;
using UnityEngine;

public class AttackAction : BattleAction
{
    public override IEnumerator Execute(BattleSystem battle, MonsterInstance user, MonsterInstance target)
    {
        // Tambahkan prefix "Lawan " 
        string who = (user == battle.Enemy && !battle.IsBoss ? "Lawan " : "") + user.Data.DisplayName;

        // 1. Tampilkan pesan
        yield return battle.Say($"{who} menyerang!", 0.7f);

        // 2. Jeda untuk animasi terjang
        yield return new WaitForSeconds(0.2f);

        // 3. Hitung & terapkan damage (Power 14 sesuai P5 Slide 19)
        int dmg = DamageFormula.Calculate(user, target, 14f);
        target.TakeDamage(dmg);

        // 4. Jeda agar MonsterPanel sempat melakukan animasi HP turun
        yield return new WaitForSeconds(0.45f);
    }
}