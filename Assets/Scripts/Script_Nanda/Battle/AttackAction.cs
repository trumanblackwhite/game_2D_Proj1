// public class AttackAction : BattleAction
// {
//     public override void Execute(BattleSession session, MonsterInstance user, MonsterInstance target)
//     {
//         int dmg = DamageFormula.Calculate(user, target, 100f);
//         target.TakeDamage(dmg);
//         Debug.Log($"{user.Data.displayName} menyerang! Damage: {dmg}");
//     }
// }