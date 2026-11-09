using UnityEngine;

public class BattlePresenter : MonoBehaviour
{
    private BattleSystem battle;
    private BattleView view;
    private GameServices services;

    public void Construct(GameServices s, BattleSystem b, BattleView v)
    {
        services = s;
        battle = b;
        view = v;

        // 1. Construct View (Inisialisasi panel, reset UI)
        v.Construct(s.Battle, s.Party.Partner, s.Battle.Enemy);

        // 2. Wiring Pesan (BattleSystem.Say akan memicu BattleView.ShowMessage)
        b.Said += v.ShowMessage;

        // 3. Wiring Menu Pilihan
        b.ChoiceRequested += _ => v.ShowMenu();
        v.Chosen += i =>
        {
            v.HideMenu();
            battle.Choose(i);
        };

        // 4. Wiring Animasi Pingsan
        b.Fainted += (monster) =>
        {
            // Tentukan panel mana yang pingsan (Partner atau Enemy)
            MonsterPanel panel = (monster == s.Party.Partner) ? v.partnerPanel : v.enemyPanel;
            if (panel != null)
            {
                panel.StartCoroutine(panel.PlayFaint());
            }
        };

        b.Evolving += (from, to) =>
        {
            if (v.partnerPanel != null)
            {
                // ⭐ KRUSIAL: Paksa panel menampilkan data LAMA (from) selama animasi
                v.partnerPanel.Show(services.Party.Partner, false, from);
                
                // Jalankan animasi sprite berkedip dan membesar/mengecil
                v.partnerPanel.StartCoroutine(v.partnerPanel.PlayEvolve(from, to, 2.4f));
            }
            
            // Mainkan animasi 3 cincin emas melebar di BattleView
            if (v != null)
            {
                v.PlayEvolveRings(2.4f);
            }
        };

        b.Evolved += (monster) =>
        {
            // Kilat putih menutupi layar
            if (v != null)
            {
                v.PlayFlash();
            }
            
            // Update panel dengan data BARU (nama berubah jadi bentuk evolusi)
            // Parameter ke-3 (displayAs) dibiarkan null, jadi akan otomatis pakai monster.Data yang baru
            if (v.partnerPanel != null)
            {
                v.partnerPanel.Show(monster, false);
            }
        };

        // 7. Wiring Level Up
        b.LevelUp += (from) =>
        {
            if (v.partnerPanel != null)
            {
                // Refresh panel untuk update teks Level (masih pakai data yang sama)
                v.partnerPanel.Show(services.Party.Partner, false, from);
            }
        };
    }
}