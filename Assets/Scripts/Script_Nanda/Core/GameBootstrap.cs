// Kode_P6 — Composition Root di Scn_Bootstrap: membuat semua service lalu membuka scene pertama. Dibuat di P2, diubah di P3, P4, P6.
// Versi demo: toast, menu jeda (Esc), tombol M = suara mati/nyala, hitung waktu bermain.
using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [SerializeField] private string firstScene = "Scn_World";      // P6: sebelumnya "Scn_World"
    [SerializeField] private MonsterDatabase monsterDatabase;      // P4
    [SerializeField] private ScreenFader fader;                    // P4
    [SerializeField] private DialogueRunner dialogueRunner;        // P6
    [SerializeField] private AudioService audioService;            // P6
    [SerializeField] private ToastView toast;                      // versi demo
    [SerializeField] private PauseMenu pauseMenu;                  // versi demo

    private GameServices services;

    void Awake()
    {
        var actionInput = new InputSystemActionInput();
        var gate = new InputGate(new InputSystemMoveInput(), actionInput);

        services = new GameServices
        {
            Input = gate,
            RawAction = actionInput,
            Database = monsterDatabase,
            Party = new PlayerParty(),
            Battle = new BattleSession(),
            Flags = new StoryFlags(),
            Save = new SaveService(),
            Dialogue = dialogueRunner,
            Audio = audioService,
            Toast = toast,
        };
        services.Scenes = new SceneFlow(this, fader, services);
        dialogueRunner.Construct(gate, audioService);
        TextMenu.Sfx = audioService;
        pauseMenu.Construct(services);
    }

    void Start() => services.Scenes.GoTo(firstScene);

    void Update()
    {
        if (MenuInput.Mute) audioService.ToggleMute();
        if (services.Scenes.InGame) services.Party.Playtime += Time.unscaledDeltaTime;   // dunia + battle
    }
}
