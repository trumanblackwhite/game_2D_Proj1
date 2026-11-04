// public class SkillAction : BattleAction
// {
//     public override void Execute(BattleSession session, MonsterInstance user, MonsterInstance target)
//     {
//         float power = 140f;   // sementara; cek MonsterData kalau Nanda menyimpan power skill
//         int dmg = DamageFormula.Calculate(user, target, power);
//         target.TakeDamage(dmg);
//         Debug.Log($"{user.Data.displayName} menggunakan {user.Data.skillName}! Damage: {dmg}");
//     }
// }