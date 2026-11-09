// Kode_P6 — Installer Scn_World (dipulihkan: follower + hud)
using System.Collections;
using UnityEngine;

public class WorldInstaller : SceneInstaller
{
    [SerializeField] private PlayerMovement player;
    [SerializeField] private PlayerInteractor interactor;
    [SerializeField] private PartnerFollower follower;   // PULIH
    [SerializeField] private PartnerHud hud;             // PULIH
    [SerializeField] private Vector2 respawnPoint = new Vector2(21.5f, 6f);

    private GameServices services;

    public override void Install(GameServices s)
    {
        services = s;
        s.Player = player.transform;
        player.Construct(s.Input);
        interactor.Construct(s.Input);
        follower.Construct(s.Party, player);             // P4: partner mengikuti
        hud.Construct(s.Party);                          // P5: bar HP + Ramuan ×N  ← COCOKKAN dengan signature Construct di PartnerHud.cs!

        foreach (Npc npc in FindInScene<Npc>()) npc.Construct(s.Dialogue, s.Flags);
        foreach (StarterGiver giver in FindInScene<StarterGiver>())
            giver.ConstructStarter(s.Party, s.Toast, s.Audio);
        foreach (MerchantNpc merchant in FindInScene<MerchantNpc>()) merchant.ConstructMerchant(s.Party, s.Audio);
        foreach (FlagToggle toggle in FindInScene<FlagToggle>()) toggle.Construct(s.Flags);
        foreach (SaveStone stone in FindInScene<SaveStone>()) stone.Construct(s, player.transform);
        foreach (WildMonster wild in FindInScene<WildMonster>())
            wild.Construct(s.Battle, s.Scenes, s.Party, s.Flags, s.Input, s.Dialogue, player.transform);
        foreach (AreaZone zone in FindInScene<AreaZone>()) zone.Construct(s.Audio);
        s.Battle.Ended += OnBattleEnded;

        if (s.Save.PendingLoad != null)
        {
            player.Teleport(s.Save.PendingLoad.playerPosition, Vector2.down);
            s.Save.PendingLoad = null;
            s.Toast.Show("Perjalanan dilanjutkan");
        }
        else if (!s.Flags.Has(StoryFlags.IntroSeen))
        {
            s.Scenes.HoldFade = true;
            StartCoroutine(Intro());
        }
    }

    void OnDestroy()
    {
        if (services != null) { services.Battle.Ended -= OnBattleEnded; services.Player = null; }
    }

    private IEnumerator Intro()
    {
        services.Dialogue.Run(IntroLines());
        yield return null;
        while (services.Dialogue.IsRunning) yield return null;
        services.Flags.Set(StoryFlags.IntroSeen);
        yield return services.Scenes.ReleaseFade();
        services.Toast.Show("Temui Tetua Rahmat (kanan jalan desa)");
    }

    private IEnumerator IntroLines()
    {
        yield return services.Dialogue.Narrate("Di ujung Pulau Batam ada desa kecil yang hidup berdampingan dengan monster.");
        yield return services.Dialogue.Narrate("Hari ini kamu akan mendapat monster partner pertamamu. Temui Tetua Rahmat di desa.");
    }

    private void OnBattleEnded(BattleResult result, bool wasBoss)
    {
        if (result == BattleResult.Lose)
            player.Teleport(respawnPoint, Vector2.up);
    }
}