using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class WildMonster : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private MonsterData species;
    [SerializeField] private int level = 3;
    [SerializeField] private bool isBoss;
    [SerializeField] private bool stationary;

    [Header("Detection")]
    [SerializeField] private float sightRadius = 3.25f;
    [SerializeField] private float giveUpRadius = 6.25f;
    [SerializeField] private float wanderSpeed = 1.125f;
    [SerializeField] private float chaseSpeed = 2.5f;
    [SerializeField] private float homeRadius = 3f;

    [Header("Alert")]
    [SerializeField] private GameObject alert;

    // Runtime
    private StateMachine fsm;
    private Rigidbody2D body;
    private MonsterInstance instance;
    private Vector2 homePos;
    private Vector2 desiredVelocity;
    private float cooldown;
    private bool inBattle;

    // Dependency (diberi oleh WorldInstaller)
    private BattleSession session;
    private SceneFlow scenes;
    private PlayerParty party;
    private Transform target;

    // === Property untuk State ===
    public MonsterInstance Instance => instance;
    public Transform Target => target;
    public Vector2 HomePos => homePos;
    public float WanderSpeed => wanderSpeed;
    public float ChaseSpeed => chaseSpeed;
    public float HomeRadius => homeRadius;
    public StateMachine Fsm => fsm;

    public bool Calm => cooldown > 0f || (session != null && session.IsSafe);

    public bool CanSeeTarget =>
        party != null && party.Partner != null && !Calm
        && DistanceToTarget < sightRadius;

    public bool LostTarget =>
        Calm || DistanceToTarget > giveUpRadius;

    public float DistanceToTarget =>
        target == null ? float.MaxValue
                       : Vector2.Distance(transform.position, target.position);

    // === Dependency Injection ===
    public void Construct(BattleSession session, SceneFlow scenes,
                          PlayerParty party, Transform target)
    {
        this.session = session;
        this.scenes = scenes;
        this.party = party;
        this.target = target;
    }

    // === Lifecycle ===
    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.gravityScale = 0;
        body.freezeRotation = true;
        homePos = transform.position;

        instance = new MonsterInstance(species, level);

        // Bangun FSM
        fsm = new StateMachine();
        var wander = new WanderState(this);
        var chase  = new ChaseState(this);
        var ret    = new ReturnState(this);

        wander.SetTransitions(chase);
        chase.SetTransitions(ret);
        ret.SetTransitions(wander);

        fsm.ChangeTo(stationary ? (IState)new IdleState(this, chase) : wander);
    }

    void Update()
    {
        if (cooldown > 0f) cooldown -= Time.deltaTime;

        if (stationary || inBattle || target == null)
        {
            desiredVelocity = Vector2.zero;
        }
        else
        {
            if (fsm != null)
            fsm.Tick(Time.deltaTime);
        }

        // Tanda ! (DIPERBARUI: pakai 'is' agar lebih aman dan sesuai modul)
        if (alert != null && fsm != null)
        {
            bool isChasing = fsm.Current is ChaseState;
            alert.SetActive(isChasing && !inBattle);
        }
    }

    void FixedUpdate()
    {
        body.linearVelocity = desiredVelocity;
        // Flip sprite sesuai arah jalan
        if (desiredVelocity.x != 0)
        {
            var sr = GetComponentInChildren<SpriteRenderer>();
            if (sr != null) sr.flipX = desiredVelocity.x < 0;
        }
    }

    // === Collision dengan Player ===
    void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Player")) return;
        if (inBattle || Calm) return;
        TriggerBattle();
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (!isBoss) return;
        if (!col.gameObject.CompareTag("Player")) return;
        if (inBattle || Calm) return;
        TriggerBattle();
    }

    // === Battle ===
    private void TriggerBattle()
    {
        inBattle = true;
        session.Begin(instance, isBoss);
        scenes.GoToBattle(() => OnBattleEnded());
    }

    private void OnBattleEnded()
    {
        inBattle = false;
        cooldown = 2.5f;
    }

    // === Setter velocity dari state ===
    public void SetVelocity(Vector2 v) => desiredVelocity = v;
}