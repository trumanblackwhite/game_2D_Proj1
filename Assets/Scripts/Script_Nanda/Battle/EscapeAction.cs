using System.Collections;
using UnityEngine;

public class EscapeAction : BattleAction
{
    public override IEnumerator Execute(BattleSystem battle, MonsterInstance user, MonsterInstance target)
    {
        // 1. Tampilkan pesan
        yield return battle.Say($"{user.Data.DisplayName} mencoba kabur...", 0.7f);

        // 2. Jeda sebentar
        yield return new WaitForSeconds(0.5f);

        // 3. Eksekusi logika kabur (akses via property public Session)
        battle.Session.TryEscape();
    }
}