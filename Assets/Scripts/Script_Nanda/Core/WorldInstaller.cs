using UnityEngine;

// PENTING: Ganti : MonoBehaviour jadi : SceneInstaller
public class WorldInstaller : SceneInstaller
{
    [SerializeField] private PlayerMovement player;
    [SerializeField] private PlayerInteractor interactor;
    
    [SerializeField] private MonsterData debugStarter; 

    public override void Install(GameServices s)
    {
        // 1. Inject Input ke Player & Interactor
        if (player != null) player.Construct(s.Input);
        if (interactor != null) interactor.Construct(s.Input);

        // 2. Inject Service ke semua Monster Liar di scene
        foreach (WildMonster wild in FindObjectsOfType<WildMonster>())
        {
            wild.Construct(
                s.Battle, 
                s.Scenes, 
                s.Party, 
                player != null ? player.transform : null
            );
        }

        // 3. Kasih monster starter kalau player belum punya partner
        if (s.Party.Partner == null && debugStarter != null)
        {
            s.Party.SetPartner(new MonsterInstance(debugStarter, 3));
        }
    }
}