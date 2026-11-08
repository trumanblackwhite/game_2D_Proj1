using System.Collections;

public class EscapeAction : BattleAction
{
    public override IEnumerator Execute(BattleSystem battle, MonsterInstance user, MonsterInstance target)
    {
        // Tampilin teks, tunggu sampai selesai diketik
        yield return battle.Say($"{user.Data.DisplayName} mencoba kabur...", 0.7f);
        
        // Langsung kabur (tanpa nunggu 0.5 detik lagi)
        battle.Session.TryEscape(); 
    }
}