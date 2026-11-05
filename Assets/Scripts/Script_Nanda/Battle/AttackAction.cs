using UnityEngine;

public class AttackAction : BattleAction
{
    public override void Execute(BattleSession session, MonsterInstance user, MonsterInstance target)
    {
        int dmg = DamageFormula.Calculate(user, target, 10f);
        target.TakeDamage(dmg);
        Debug.Log($"{user.Data.DisplayName} menyerang! Damage: {dmg}");
    }
}