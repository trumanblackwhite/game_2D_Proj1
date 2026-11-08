using System.Collections;
using UnityEngine;

public class ItemAction : BattleAction
{
    public override IEnumerator Execute(BattleSystem battle, MonsterInstance user, MonsterInstance target)
    {
        if (battle.Session.Potions <= 0)  
        {
            yield return battle.Say("Ramuan habis!", 0.8f);
            yield break;
        }

        yield return battle.Say($"{user.Data.DisplayName} menggunakan Ramuan!", 0.7f);
        battle.Session.UseItem(); 
        
        int healAmount = Mathf.RoundToInt(user.MaxHP * 0.4f);
        user.Heal(healAmount);
        
        yield return new WaitForSeconds(0.6f);
        yield return battle.Say($"HP pulih {healAmount}! (Sisa: {battle.Session.Potions})", 0.8f);  // ← Huruf besar 'S'
    }
}