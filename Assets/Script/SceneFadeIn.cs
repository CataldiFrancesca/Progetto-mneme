using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneFadeIn : MonoBehaviour
{
    public Image fadeImage;           // Immagine nera a schermo
    public float fadeDuration = 1.5f; // Durata del fade

    void Start()
    {
        if (fadeImage != null)
        {
            // Assicura partenza a nero opaco
            Color c = fadeImage.color;
            c.a = 1f;
            fadeImage.color = c;

            fadeImage.gameObject.SetActive(true);
            StartCoroutine(FadeIn());
        }
    }

    IEnumerator FadeIn()
    {
        float timer = 0f;

        Color originalColor = fadeImage.color;

        while (timer < fadeDuration)
        {
            float t = timer / fadeDuration;
            originalColor.a = Mathf.Lerp(1f, 0f, t);
            fadeImage.color = originalColor;

            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        // Imposta alpha a 0 preciso e disattiva immagine
        originalColor.a = 0f;
        fadeImage.color = originalColor;
        fadeImage.gameObject.SetActive(false);
    }
}
