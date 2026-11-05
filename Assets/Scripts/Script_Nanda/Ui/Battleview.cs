using System;
using UnityEngine;
using UnityEngine.UI;

public class BattleView : MonoBehaviour
{
    [SerializeField] private MonsterPanel partnerPanel;
    [SerializeField] private MonsterPanel enemyPanel;
    [SerializeField] private Text sayText;
    [SerializeField] private TextMenu menu;

    public event Action<int> Chosen;
    private BattleSession session;

    public void Construct(BattleSession s, MonsterInstance partner, MonsterInstance enemy)
    {
        session = s;
        partnerPanel.Show(partner);
        enemyPanel.Show(enemy);
        menu.Picked += i => Chosen?.Invoke(i);
        menu.gameObject.SetActive(false);
        sayText.text = "";
    }

    public void Say(string line) => sayText.text = line;

    public void ShowMenu()
    {
        bool[] enabled = { true, true, session.Potions > 0, !session.IsBoss };
        menu.Open(new[] { "Serang", "Jurus", "Ramuan", "Kabur" }, enabled);
    }

    public void HideMenu() => menu.Close();
}