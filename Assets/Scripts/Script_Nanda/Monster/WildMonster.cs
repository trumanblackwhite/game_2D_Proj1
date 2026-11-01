using UnityEngine;

public class WildMonster : MonoBehaviour
{
    public StateMachine StateMachine { get; private set; }

    [SerializeField] private GameObject alert; // sprite UI_Alert (tanda seru !)
    
    private Transform playerTransform;
    private Vector2 homePosition;
    
    // Variabel pendukung sesuai modul
    private float cooldown = 0f;
    private bool inBattle = false; // Akan dihubungkan saat sistem battle aktif
    private bool stationary = false; // True jika monster diam (seperti boss)
    
    // Radius deteksi sesuai modul
    private float sightRadius = 3.25f;
    private float giveUpRadius = 6.25f;

    // Properti kondisi tenang (Calm) sesuai modul
    private bool Calm => cooldown > 0f; // Bisa ditambah kondisi session.IsSafe atau gate.IsLocked nanti

    public float DistanceToTarget => playerTransform != null ? Vector2.Distance(transform.position, playerTransform.position) : Mathf.Infinity;
    
    // Logika CanSeeTarget dan LostTarget diperbarui dengan kondisi Calm
    public bool CanSeeTarget => playerTransform != null && !Calm && DistanceToTarget < sightRadius;
    public bool LostTarget => Calm || DistanceToTarget > giveUpRadius;
    
    public bool HasReachedHome => Vector2.Distance(transform.position, homePosition) < 0.1f;

    void Start()
    {
        homePosition = transform.position;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }

        StateMachine = new StateMachine();
        StateMachine.ChangeTo(new WanderState(this));
    }

    void Update()
    {
        // Logika Cooldown dan Update FSM per frame sesuai modul
        if (cooldown > 0f) cooldown -= Time.deltaTime;

        if (stationary || inBattle || playerTransform == null)
        {
            // Jika diam, dalam battle, atau target null, kecepatan 0
        }
        else
        {
            StateMachine.Tick(Time.deltaTime);
        }

        // Mengatur aktif/tidaknya ikon tanda seru (!) berdasarkan state Chase
        if (alert != null)
        {
            // Memeriksa apakah state aktif saat ini adalah Chase
            bool isChasing = StateMachine.Current is ChaseState;
            alert.SetActive(isChasing && !inBattle);
        }
    }

    public void SetAlertActive(bool isActive)
    {
        if (alert != null) alert.SetActive(isActive);
    }
}