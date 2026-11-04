using UnityEngine;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.5f;

    private void Awake()
    {
        // Cari CanvasGroup di objek yang sama kalau slotnya lupa diisi
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        
        // Start transparan dan tidak memblokir klik mouse
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }

    // Layar jadi hitam (biasanya dipanggil SEBELUM pindah scene)
    public IEnumerator FadeOut() 
    { 
        canvasGroup.blocksRaycasts = true; // Kunci input player
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    } 

    // Layar jadi bening lagi (dipanggil SETELAH scene baru selesai dimuat)
    public IEnumerator FadeIn()  
    { 
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false; // Buka lagi input player
    }  
}