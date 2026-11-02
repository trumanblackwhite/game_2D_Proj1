using UnityEngine;

public class WanderState : IState
{
    private readonly WildMonster owner;
    private IState chaseState;
    private Vector2 wanderDir;
    private float dirTimer;

    public WanderState(WildMonster owner) { this.owner = owner; }
    public void SetTransitions(IState chase) => chaseState = chase;

    public void Enter()
    {
        PickNewDirection();
    }

    public void Tick(float dt)
    {
        // Ganti arah acak tiap 2-4 detik
        dirTimer -= dt;
        if (dirTimer <= 0f) PickNewDirection();

        // Jangan keluar radius rumah
        Vector2 toHome = owner.HomePos - (Vector2)owner.transform.position;
        if (toHome.magnitude > owner.HomeRadius)
            wanderDir = toHome.normalized;

        owner.SetVelocity(wanderDir * owner.WanderSpeed);

        // Transisi
        if (owner.CanSeeTarget)
            owner.Fsm.ChangeTo(chaseState);
    }

    public void Exit() { }

    private void PickNewDirection()
    {
        wanderDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        dirTimer = Random.Range(2f, 4f);
    }
}