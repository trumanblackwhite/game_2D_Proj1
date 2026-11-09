using UnityEngine;

public class EscapeAction : BattleAction
{
    public override void Execute(BattleSession session, MonsterInstance user, MonsterInstance target)
    {
        Debug.Log("Mencoba kabur dari battle!");
        session.TryEscape(); // Memanggil logika kabur di BattleSession
    }
}