using UnityEngine;

// 1. State Wander (Berkeliaran / Diam di sekitar Home)
public class WanderState : IState
{
    private WildMonster monster;

    public WanderState(WildMonster monster) { this.monster = monster; }

    public void Enter() { }
    
    public void Tick(float deltaTime)
    {
        if (monster.DistanceToTarget < 3.25f && monster.CanSeeTarget)
        {
            monster.StateMachine.ChangeTo(new ChaseState(monster));
        }
    }

    public void Exit() { }
}

// 2. State Chase (Mengejar Pemain)
public class ChaseState : IState
{
    private WildMonster monster;

    public ChaseState(WildMonster monster) { this.monster = monster; }

    public void Enter() 
    { 
        monster.SetAlertActive(true); 
    }

    public void Tick(float deltaTime)
    {
        if (monster.LostTarget || monster.DistanceToTarget > 6.25f)
        {
            monster.StateMachine.ChangeTo(new ReturnState(monster));
        }
    }

    public void Exit() 
    { 
        monster.SetAlertActive(false); 
    }
}

// 3. State Return (Pulang ke Asal)
public class ReturnState : IState
{
    private WildMonster monster;

    public ReturnState(WildMonster monster) { this.monster = monster; }

    public void Enter() { }

    public void Tick(float deltaTime)
    {
        if (monster.HasReachedHome)
        {
            monster.StateMachine.ChangeTo(new WanderState(monster));
        }
    }

    public void Exit() { }
}