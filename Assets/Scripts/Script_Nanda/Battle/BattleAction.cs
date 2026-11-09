using System.Collections;

public abstract class BattleAction
{
    public abstract IEnumerator Execute(BattleSystem battle, MonsterInstance user, MonsterInstance target);
}