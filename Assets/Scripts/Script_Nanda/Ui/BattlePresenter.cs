using UnityEngine;
using System.Collections;

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

        v.Construct(s.Battle, s.Party.Partner, s.Battle.Enemy);

        b.Said += v.ShowMessage;
        b.ChoiceRequested += _ => v.ShowMenu();
        v.Chosen += i => { v.HideMenu(); b.Choose(i); };

        b.Fainted += (monster) =>
        {
            MonsterPanel panel = (monster == s.Party.Partner) ? view.partnerPanel : view.enemyPanel;
            if (panel != null) panel.StartCoroutine(panel.PlayFaint());
        };

        // ✅ EVOLVING: Mainkan animasi cincin + sprite berganti, TAPI nama JANGAN diubah
        b.Evolving += (from, to) =>
        {
            if (view.partnerPanel != null)
            {
                // Nama tetap "Pyrit" karena displayAs = from (data lama)
                view.partnerPanel.Show(services.Party.Partner, false, from);
                view.partnerPanel.StartCoroutine(view.partnerPanel.PlayEvolve(from, to, 2.4f));
            }
            if (view != null) view.PlayEvolveRings(2.4f);
        };

        // ✅ EVOLVED: Baru update nama jadi "Pyrago" + kilat putih
        b.Evolved += (monster) =>
        {
            if (view != null) view.PlayFlash();
            if (view.partnerPanel != null)
            {
                // Sekarang nama berubah jadi data baru
                view.partnerPanel.Show(monster, false, monster.Data);
            }
        };

        b.LevelUp += (from) =>
        {
            if (view.partnerPanel != null)
            {
                view.partnerPanel.Show(services.Party.Partner, false, from);
            }
        };
    }
}