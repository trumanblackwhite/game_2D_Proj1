using System.Collections;
using UnityEngine;

public class SkillAction : BattleAction
{
    public override IEnumerator Execute(BattleSystem battle, MonsterInstance user, MonsterInstance target)
    {
        // 1. Tampilkan pesan
        yield return battle.Say($"{user.Data.DisplayName} menggunakan {user.Data.SkillName}!", 0.7f);

        // 2. Jeda untuk animasi
        yield return new WaitForSeconds(0.2f);

        // 3. Hitung damage dengan power lebih tinggi (22 sesuai P5 Slide 19)
        float power = 22f;
        int dmg = DamageFormula.Calculate(user, target, power);
        target.TakeDamage(dmg);

        // 4. Jeda animasi
        yield return new WaitForSeconds(0.45f);
    }
}