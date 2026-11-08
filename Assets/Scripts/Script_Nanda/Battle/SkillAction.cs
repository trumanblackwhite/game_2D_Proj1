using System.Collections;
using UnityEngine;

public class SkillAction : BattleAction
{
    public override IEnumerator Execute(BattleSystem battle, MonsterInstance user, MonsterInstance target)
    {
        yield return battle.Say($"{user.Data.DisplayName} menggunakan {user.Data.SkillName}!", 0.7f);
        yield return new WaitForSeconds(0.2f);
        
        float power = 14f;
        int dmg = DamageFormula.Calculate(user, target, power);
        target.TakeDamage(dmg);
        
        yield return new WaitForSeconds(0.45f);
    }
}