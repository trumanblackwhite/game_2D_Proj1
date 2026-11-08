using System;
using UnityEngine;
using UnityEngine.UI;

public class BattleView : MonoBehaviour
{
    [SerializeField] private MonsterPanel partnerPanel;
    [SerializeField] private MonsterPanel enemyPanel;
    [SerializeField] private Text messageText;
    [SerializeField] private Text hintText;
    [SerializeField] private TextMenu menu;

    public event Action<int> Chosen;
    private BattleSession session;
    private MonsterInstance partner;
     private Coroutine typing;

    public void Construct(BattleSession s, MonsterInstance partnerMon, MonsterInstance enemyMon)
    {
        session = s;
        partner = partnerMon;
        partnerPanel.Show(partnerMon, false);
        enemyPanel.Show(enemyMon, s.IsBoss);
        menu.Picked += i => Chosen?.Invoke(i);
        menu.gameObject.SetActive(false);
        messageText.text = "";
        hintText.enabled = false;
    }

    public void Say(string line) => messageText.text = line;
    public void ShowMessage(string text)
    {
        hintText.enabled = false;
        if (typing != null) StopCoroutine(typing);
        typing = StartCoroutine(Type(text));
    }

    private System.Collections.IEnumerator Type(string text)
    {
        messageText.text = "";
        float delay = 1f / 55f; // Kecepatan 55 huruf/detik (sesuai modul Slide 30)
        foreach (char c in text)
        {
            messageText.text += c;
            yield return new WaitForSeconds(delay);
        }
    }
    public void ShowMenu()
    {
        messageText.text = $"Apa yang akan {partner.Data.DisplayName} lakukan?";
        hintText.text = $"Jurus elemen: {partner.Data.SkillName}";
        hintText.enabled = true;
        bool[] enabled = { true, true, session.Potions > 0, !session.IsBoss };
        menu.Open(new[] { "Serang", "Jurus", "Ramuan", "Kabur" }, enabled);
    }

    public void HideMenu()
    {
        menu.Close();
        hintText.enabled = false;
    }
}