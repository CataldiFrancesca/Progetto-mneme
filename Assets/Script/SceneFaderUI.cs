using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class SceneFaderUI : MonoBehaviour
{
    [Header("Fade Settings")]
    public Image fadeImage;              // L'immagine nera UI (inizialmente opaca)
    public float fadeDuration = 1.5f;

    [Header("Scene")]
    public string nextSceneName = "NextScene";

    [Header("UI Button")]
    public Button continueButton;        // Bottone da premere per continuare
    public CanvasGroup buttonCanvasGroup;

    private bool isFading = false;

    void Start()
    {
        // Imposta l'immagine completamente opaca
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 1f;
            fadeImage.color = c;
        }

        // Disattiva il bottone finché non finisce il fade-in
        if (continueButton != null)
        {
            continueButton.interactable = false;
            continueButton.onClick.AddListener(OnContinueClicked);
        }

        if (buttonCanvasGroup != null)
        {
            buttonCanvasGroup.alpha = 0f;
            buttonCanvasGroup.interactable = false;
            buttonCanvasGroup.blocksRaycasts = false;
        }

        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            fadeImage.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }

        fadeImage.color = new Color(0f, 0f, 0f, 0f);

        // Attiva il bottone dopo il fade-in
        if (buttonCanvasGroup != null)
        {
            float t2 = 0f;
            while (t2 < 1f)
            {
                t2 += Time.deltaTime;
                buttonCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t2 / 1f);
                yield return null;
            }

            buttonCanvasGroup.interactable = true;
            buttonCanvasGroup.blocksRaycasts = true;
        }

        if (continueButton != null)
            continueButton.interactable = true;
    }

    void OnContinueClicked()
    {
        if (!isFading)
            StartCoroutine(FadeOutAndLoadScene());
    }

    IEnumerator FadeOutAndLoadScene()
    {
        isFading = true;

        // Disabilita il bottone
        if (buttonCanvasGroup != null)
        {
            buttonCanvasGroup.interactable = false;
            buttonCanvasGroup.blocksRaycasts = false;
        }

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            fadeImage.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }

        fadeImage.color = new Color(0f, 0f, 0f, 1f);
        SceneManager.LoadScene(nextSceneName);
    }
}
