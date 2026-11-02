using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [SerializeField] private string firstScene = "Scn_World";
    [SerializeField] private MonsterDatabase monsterDatabase;
    [SerializeField] private ScreenFader fader;
    
    private GameServices services;

    void Awake()
    {
        // PENTING: Agar GameBootstrap tidak hancur saat pindah ke Scn_World
        DontDestroyOnLoad(gameObject);

        // InputGate sekarang tidak butuh parameter lagi
        // Di dalam method Awake():
        var gate = new InputGate(new InputSystemMoveInput(), new InputSystemActionInput());
        
        services = new GameServices
        {
            Input = gate, 
            Database = monsterDatabase,
            Party = new PlayerParty(),
            Battle = new BattleSession(),
        };

        services.Scenes = new SceneFlow(this, fader, services);
    }

    void Start()
    {
        services.Scenes.GoTo(firstScene);
    }
}