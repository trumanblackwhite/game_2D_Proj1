using UnityEngine;

public class SkillAction : BattleAction
{
    public override void Execute(BattleSession session, MonsterInstance user, MonsterInstance target)
    {
        float power = 14f;    // sementara; cek MonsterData bila ada field power skill
        int dmg = DamageFormula.Calculate(user, target, power);
        target.TakeDamage(dmg);
        Debug.Log($"{user.Data.DisplayName} menggunakan {user.Data.SkillName}! Damage: {dmg}");
    }
}