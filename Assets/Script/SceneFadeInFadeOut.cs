using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneFadeInFadeOut : MonoBehaviour
{
    public Image fadeImage;           // Immagine nera a schermo
    public float fadeDuration = 1.5f; // Durata del fade
    public float delayBetweenFades = 3f;
    public string nextSceneName = "NomeDellaScena";

    void Start()
    {
        if (fadeImage != null)
        {
            // Imposta a nero opaco
            fadeImage.color = new Color(0f, 0f, 0f, 1f);
            fadeImage.gameObject.SetActive(true);

            StartCoroutine(FadeSequence());
        }
        else
        {
            Debug.LogError("fadeImage non assegnato!");
        }
    }

    private IEnumerator FadeSequence()
    {
        yield return StartCoroutine(Fade(1f, 0f));         // Fade In (nero -> trasparente)
        yield return new WaitForSeconds(delayBetweenFades); // Attesa 3 secondi
        yield return StartCoroutine(Fade(0f, 1f));         // Fade Out (trasparente -> nero)

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("Nome della scena non impostato!");
        }
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float timer = 0f;
        Color c = fadeImage.color;

        while (timer < fadeDuration)
        {
            float t = timer / fadeDuration;
            c.a = Mathf.Lerp(startAlpha, endAlpha, t);
            fadeImage.color = c;
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        // Fissa alpha finale
        c.a = endAlpha;
        fadeImage.color = c;
    }
}
