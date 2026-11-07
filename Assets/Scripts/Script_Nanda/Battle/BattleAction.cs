public abstract class BattleAction
{
    public abstract void Execute(BattleSession session, MonsterInstance user, MonsterInstance target);
}