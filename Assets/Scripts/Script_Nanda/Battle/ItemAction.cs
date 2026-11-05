using UnityEngine;

public class ItemAction : BattleAction
{
    public override void Execute(BattleSession session, MonsterInstance user, MonsterInstance target)
    {
        if (session.Potions <= 0)
        {
            Debug.Log("Ramuan habis!");
            return;
        }
        session.UseItem();
        user.Heal(50);
        Debug.Log($"{user.Data.DisplayName} minum Ramuan! HP +50 (sisa {session.Potions})");
    }
}