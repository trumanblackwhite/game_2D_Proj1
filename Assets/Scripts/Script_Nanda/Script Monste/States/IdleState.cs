using UnityEngine;

public class IdleState : IState
{
    private readonly WildMonster owner;
    private readonly IState nextState;

    public IdleState(WildMonster owner, IState nextState)
    {
        this.owner = owner;
        this.nextState = nextState;
    }

    public void Enter() => owner.SetVelocity(Vector2.zero);
    public void Tick(float dt)
    {
        if (owner.CanSeeTarget)
            owner.Fsm.ChangeTo(nextState);
    }
    public void Exit() { }
}