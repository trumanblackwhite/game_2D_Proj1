using UnityEngine;

public class ChaseState : IState
{
    private readonly WildMonster owner;
    private IState returnState;

    public ChaseState(WildMonster owner) { this.owner = owner; }
    public void SetTransitions(IState ret) => returnState = ret;

    public void Enter() { }

    public void Tick(float dt)
    {
        Vector2 dir = ((Vector2)owner.Target.position - (Vector2)owner.transform.position).normalized;
        owner.SetVelocity(dir * owner.ChaseSpeed);

        // Flip sprite menghadap player
        var sr = owner.GetComponentInChildren<SpriteRenderer>();
        if (sr != null) sr.flipX = dir.x < 0;

        if (owner.LostTarget)
            owner.Fsm.ChangeTo(returnState);
    }

    public void Exit() { }
}