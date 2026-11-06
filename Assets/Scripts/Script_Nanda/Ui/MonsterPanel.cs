using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MonsterPanel : MonoBehaviour
{
    [SerializeField] private SpriteRenderer body;   // sprite dunia: EnemyMon/PlayerMon
    [SerializeField] private Text nameText;
    [SerializeField] private Text levelText;
    [SerializeField] private Text elementText;      // opsional (panel lawan)
    [SerializeField] private Image hpFill;
  

    private MonsterInstance monster;
    private bool boss;
    private Coroutine idle;

    public void Show(MonsterInstance m, bool isBoss = false)
    {
        if (monster != null) monster.Changed -= Refresh;
        monster = m;
        boss = isBoss;
        m.Changed += Refresh;
        Refresh();
        if (idle != null) StopCoroutine(idle);
        idle = StartCoroutine(IdleLoop());
    }

    void OnDestroy()
    {
        if (monster != null) monster.Changed -= Refresh;
    }

    private void Refresh()
    {
        nameText.text = (boss ? "★ " : "") + monster.Data.DisplayName;
        levelText.text = $"Lv {monster.Level}";
        if (elementText != null) elementText.text = monster.Data.Element.ToString();
        float ratio = monster.MaxHP > 0 ? (float)monster.CurrentHP / monster.MaxHP : 0f;
        hpFill.fillAmount = ratio;
        hpFill.color = HpColors.For(ratio);
    }

    private IEnumerator IdleLoop()
    {
        int i = 0;
        while (true)
        {
            if (body != null && monster?.Data.Frames != null && monster.Data.Frames.Length > 0)
                body.sprite = monster.Data.Frames[i % 2];
            i++;
            yield return new WaitForSeconds(0.35f);
        }
    }
}