using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BattleView : MonoBehaviour
{
    [Header("Panels")]
    public MonsterPanel partnerPanel;
    public MonsterPanel enemyPanel;

    [Header("UI Text References")]
    [SerializeField] private Text messageText;
    [SerializeField] private Text hintText;
    
    [Header("Menu Reference")]
    [SerializeField] private TextMenu menu;
    
    [Header("Evolution Effects")]
    [SerializeField] private Image[] rings;  // Drag Ring_0, Ring_1, Ring_2
    [SerializeField] private Image flash;    // Drag Flash (white screen)

    [Header("Wipe Effects (Intro)")]
    [SerializeField] private Image wipeTop;    // Drag WipeTop
    [SerializeField] private Image wipeBottom; // Drag WipeBottom

    public event Action<int> Chosen;
    
    private BattleSession session;
    private MonsterInstance partner;
     private Coroutine typing;

    public void Construct(BattleSession s, MonsterInstance partnerMon, MonsterInstance enemyMon)
    {
        session = s;
        partner = partnerMon;

        // Initialize panels
        partnerPanel.Show(partnerMon, false);
        enemyPanel.Show(enemyMon, s.IsBoss);

        // Setup menu
        if (menu != null)
        {
            menu.Picked += index => Chosen?.Invoke(index);
            menu.gameObject.SetActive(false);
        }

        // Reset UI
        messageText.text = "";
        if (hintText != null) hintText.enabled = false;
        
        // Reset evolution effects
        if (rings != null)
        {
            foreach (var ring in rings)
            {
                if (ring != null)
                {
                    ring.gameObject.SetActive(false);
                    ring.transform.localScale = Vector3.one * 0.5f;
                }
            }
        }
        
        if (flash != null)
        {
            flash.gameObject.SetActive(false);
            flash.color = new Color(1f, 1f, 1f, 0f);
        }

        // Reset wipe effects
        if (wipeTop != null) wipeTop.gameObject.SetActive(false);
        if (wipeBottom != null) wipeBottom.gameObject.SetActive(false);
    }

    // ✅ ANIMASI NGEHIT PEMBUKA (P5 Slide 31: 0.48 detik)
    public void PlayIntro()
    {
        StartCoroutine(IntroRoutine());
    }

    private IEnumerator IntroRoutine()
    {
        if (wipeTop != null) wipeTop.gameObject.SetActive(true);
        if (wipeBottom != null) wipeBottom.gameObject.SetActive(true);

        float duration = 0.48f;
        float elapsed = 0f;
        
        // 1. Tutup layar (tirai bergerak ke tengah)
        while (elapsed < duration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration * 0.5f);
            if (wipeTop != null) wipeTop.rectTransform.anchoredPosition = Vector2.Lerp(new Vector2(0, 0), new Vector2(0, 180), t);
            if (wipeBottom != null) wipeBottom.rectTransform.anchoredPosition = Vector2.Lerp(new Vector2(0, 0), new Vector2(0, -180), t);
            yield return null;
        }

        // 2. Tahan layar hitam sebentar
        yield return new WaitForSeconds(0.2f);

        // 3. Buka layar (tirai bergerak menjauh)
        elapsed = 0f;
        while (elapsed < duration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration * 0.5f);
            if (wipeTop != null) wipeTop.rectTransform.anchoredPosition = Vector2.Lerp(new Vector2(0, 180), new Vector2(0, 360), t);
            if (wipeBottom != null) wipeBottom.rectTransform.anchoredPosition = Vector2.Lerp(new Vector2(0, -180), new Vector2(0, -360), t);
            yield return null;
        }

        // 4. Sembunyikan kembali setelah selesai
        if (wipeTop != null) wipeTop.gameObject.SetActive(false);
        if (wipeBottom != null) wipeBottom.gameObject.SetActive(false);
    }

    // ✅ EFEK MENGETIK (P5 Slide 30: 55 huruf per detik)
    public void ShowMessage(string text)
    {
        if (hintText != null) hintText.enabled = false;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        
        typingCoroutine = StartCoroutine(TypeText(text));
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
        if (partner == null) return;

        messageText.text = $"Apa yang akan {partner.Data.DisplayName} lakukan?";
        
        if (hintText != null)
        {
            hintText.text = $"Jurus elemen: {partner.Data.SkillName}";
            hintText.enabled = true;
        }

        if (menu != null)
        {
            bool[] enabledOptions = new bool[]
            {
                true,                       // Serang
                true,                       // Jurus
                session.Potions > 0,        // Ramuan
                !session.IsBoss             // Kabur
            };

            string[] options = new string[] { "Serang", "Jurus", "Ramuan", "Kabur" };
            menu.Open(options, enabledOptions);
        }
    }

    public void HideMenu()
    {
        if (menu != null)
        {
            menu.Close();
        }
        
        if (hintText != null)
        {
            hintText.enabled = false;
        }
    }

    // ✅ ANIMASI 3 CINCIN EMAS (P5 Slide 38)
    public void PlayEvolveRings(float duration = 2.4f)
    {
        if (rings == null || rings.Length == 0) return;
        StartCoroutine(EvolveRingsRoutine(duration));
    }

    private IEnumerator EvolveRingsRoutine(float duration)
    {
        foreach (var ring in rings)
        {
            if (ring != null)
            {
                ring.gameObject.SetActive(true);
                ring.color = new Color(1f, 0.78f, 0.1f, 1f);
                ring.transform.localScale = Vector3.one * 0.5f;
            }
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            for (int i = 0; i < rings.Length; i++)
            {
                if (rings[i] != null)
                {
                    float ringDelay = i * 0.15f;
                    float ringT = Mathf.Clamp01((t - ringDelay) / (1f - ringDelay));
                    
                    float scale = Mathf.Lerp(0.5f, 3f, ringT);
                    rings[i].transform.localScale = Vector3.one * scale;
                    rings[i].color = new Color(1f, 0.78f, 0.1f, 1f - ringT);
                }
            }
            yield return null;
        }

        foreach (var ring in rings)
        {
            if (ring != null) ring.gameObject.SetActive(false);
        }
    }

    // ✅ KILAT PUTIH (P5 Slide 38)
    public void PlayFlash()
    {
        if (flash == null) return;
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        flash.gameObject.SetActive(true);
        float duration = 0.5f;
        float elapsed = 0f;

        // Fade in quickly (0.3s)
        while (elapsed < duration * 0.3f)
        {
            elapsed += Time.deltaTime;
            float alpha = elapsed / (duration * 0.3f);
            flash.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        // Fade out (0.7s)
        elapsed = 0f;
        while (elapsed < duration * 0.7f)
        {
            elapsed += Time.deltaTime;
            float alpha = 1f - (elapsed / (duration * 0.7f));
            flash.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        flash.color = new Color(1f, 1f, 1f, 0f);
        flash.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        if (menu != null)
        {
            // Cleanup if needed
        }
    }
}