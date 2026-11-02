using UnityEngine;

public class ReturnState : IState
{
    private readonly WildMonster owner;
    private IState wanderState;

    public ReturnState(WildMonster owner) { this.owner = owner; }
    public void SetTransitions(IState wander) => wanderState = wander;

    public void Enter() { }

    public void Tick(float dt)
    {
        Vector2 dir = (owner.HomePos - (Vector2)owner.transform.position).normalized;
        owner.SetVelocity(dir * owner.WanderSpeed);

        if (Vector2.Distance(owner.transform.position, owner.HomePos) < 0.2f)
        {
            owner.SetVelocity(Vector2.zero);
            owner.Fsm.ChangeTo(wanderState);
        }
    }

    public void Exit() { }
}