using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MonsterPanel : MonoBehaviour
{
    [Header("UI & Sprite References")]
    [SerializeField] private SpriteRenderer body;       
    [SerializeField] private Text nameText;
    [SerializeField] private Text levelText;
    [SerializeField] private Text elementText;          
    [SerializeField] private Image hpFill;
    [SerializeField] private Text hpText;               
    [SerializeField] private Image expFill;             

    private MonsterInstance monster;
    private MonsterData displayData;     private bool boss;
    
    private float hpShow;                               
    private Vector3 homePosition;                       
    private bool isFainting;
    private bool isEvolving;


    public void Show(MonsterInstance m, bool isBoss = false, MonsterData displayAs = null)
    {
        if (monster != null) monster.Changed -= Refresh;
        
        monster = m;
        boss = isBoss;
        isFainting = false;
        isEvolving = false;
        gameObject.SetActive(true); 

        displayData = displayAs ?? m.Data;

        if (body != null)
        {
            homePosition = body.transform.localPosition;
        }

        m.Changed += Refresh;
        hpShow = m.CurrentHP; 
        
        Refresh();
    }

    void OnDestroy()
    {
        if (monster != null) monster.Changed -= Refresh;
    }

    private void Refresh()
    {
        if (monster == null || displayData == null) return;

        nameText.text = (boss ? "★ " : "") + displayData.DisplayName;
        levelText.text = $"Lv {monster.Level}";
        
        if (elementText != null) 
            elementText.text = displayData.Element.ToString();

        if (expFill != null)
        {
            expFill.fillAmount = (float)monster.CurrentEXP / monster.ExpToNext;
        }
    }

    void Update()
    {
        if (monster == null || isFainting || isEvolving) return;

        if (body != null && displayData.Frames != null && displayData.Frames.Length >= 2)
        {
            int frame = Mathf.FloorToInt(Time.time / 0.35f) % 2; 
            body.sprite = displayData.Frames[frame];
        }

        // Animasi HP Lerp
        float target = monster.CurrentHP;
        float k = 1f - Mathf.Pow(0.85f, Time.deltaTime * 60f); 
        hpShow = Mathf.Lerp(hpShow, target, k);

        if (hpFill != null)
        {
            float ratio = monster.CurrentHP <= 0 ? 0f : (monster.MaxHP > 0 ? hpShow / monster.MaxHP : 0f);
            hpFill.fillAmount = ratio;
            hpFill.color = HpColors.For(hpFill.fillAmount);
        }

        if (hpText != null)
        {
            hpText.text = $"{Mathf.RoundToInt(hpShow)}/{monster.MaxHP}";
        }
    }

    public IEnumerator PlayFaint()
    {
        isFainting = true;
        float duration = 0.5f;
        float elapsed = 0f;

        Transform targetTransform = body != null ? body.transform : transform;
        Vector3 startPos = targetTransform.localPosition;
        Vector3 endPos = startPos + new Vector3(0f, -20f, 0f); 

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            targetTransform.localPosition = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        targetTransform.localPosition = endPos;
        gameObject.SetActive(false); 
    }

    public IEnumerator PlayEvolve(MonsterData from, MonsterData to, float duration)
    {
        isEvolving = true; // Stop animasi idle
        Transform targetTransform = body != null ? body.transform : transform;
        Vector3 startScale = targetTransform.localScale;
        Vector3 targetScale = startScale * 1.2f; 

        float elapsed = 0f;
        float swapInterval = 0.15f; // Mulai lambat
        float lastSwap = 0f;

        // Fase 1: Membesar + sprite berganti makin cepat
        while (elapsed < duration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration * 0.5f);
            targetTransform.localScale = Vector3.Lerp(startScale, targetScale, t);

            // Swap interval makin cepat (0.15 → 0.03 detik)
            swapInterval = Mathf.Lerp(0.15f, 0.03f, t);

            if (elapsed - lastSwap >= swapInterval)
            {
                lastSwap = elapsed;
                
                // Berganti antara sprite lama (from) dan baru (to)
                if (body != null && to != null && to.Frames != null && to.Frames.Length >= 2 && from.Frames != null && from.Frames.Length >= 2)
                {
                    int frame = Mathf.FloorToInt(Time.time / 0.1f) % 2;
                    // 50% kemungkinan tampil sprite lama, 50% sprite baru
                    body.sprite = (Random.value < 0.5f ? from : to).Frames[frame];
                }
            }
            yield return null;
        }

        // Fase 2: Mengecil kembali ke normal
        elapsed = 0f;
        while (elapsed < duration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration * 0.5f);
            targetTransform.localScale = Vector3.Lerp(targetScale, startScale, t);
            yield return null;
        }

        targetTransform.localScale = startScale; 
        isEvolving = false; // Kembalikan animasi idle (akan pakai displayData yang baru nanti)
    }
}