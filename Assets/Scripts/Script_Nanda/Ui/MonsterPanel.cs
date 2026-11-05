using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MonsterPanel : MonoBehaviour
{
    [SerializeField] private Image body;
    [SerializeField] private Text nameText;
    [SerializeField] private Text levelText;
    [SerializeField] private Image hpFill;

    private MonsterInstance monster;
    private Coroutine idle;

    public void Show(MonsterInstance m)
    {
        monster = m;
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
        nameText.text = monster.Data.DisplayName;
        levelText.text = $"Lv {monster.Level}";
        float ratio = monster.MaxHP > 0 ? (float)monster.CurrentHP / monster.MaxHP : 0f;
        hpFill.fillAmount = ratio;
        // hpFill.color = HpColors.For(ratio);   // samakan nama method di HpColors.cs
    }

    private IEnumerator IdleLoop()
    {
        int i = 0;
        while (true)
        {
            body.sprite = monster.Data.Frames[i % 2];   // idle 2 frame berkedip
            i++;
            yield return new WaitForSeconds(0.35f);
        }
    }
}