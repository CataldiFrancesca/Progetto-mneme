using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class IntroText : MonoBehaviour
{
    public TextMeshProUGUI introText;
    public Button startButton;

    public CanvasGroup startButtonCanvasGroup;

    [Header("Fade Panel")]
    public GameObject fadePanelObject; // pannello nero (Image con alpha 0)
    public Image fadeImage;            // l'immagine nera da sfumare
    public float fadeDuration = 1.5f;

    [TextArea(3, 10)]
    public string fullText;
    public float typingSpeed = 0.05f;
    public string nomeScena = "camera da letto";

    [Header("Audio")]
    public AudioSource scritturaAudio;


    private bool isFading = false;
    private Coroutine typingCoroutine;
    private bool textCompleted = false;



    void Start()
    {
        if (fadePanelObject != null)
            fadePanelObject.SetActive(false);

        startButtonCanvasGroup.alpha = 0f;
        startButtonCanvasGroup.interactable = false;
        startButtonCanvasGroup.blocksRaycasts = false;

        typingCoroutine = StartCoroutine(TypeText());
        startButton.onClick.AddListener(OnStartButtonClicked);
    }



    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !textCompleted)
        {
            SkipText();
        }
    }

    void SkipText()
{
    if (typingCoroutine != null)
        StopCoroutine(typingCoroutine);

    if (scritturaAudio != null)
        scritturaAudio.Stop(); // Ferma eventuali suoni residui

    introText.text = fullText;
    textCompleted = true;
    StartCoroutine(FadeInButton());
}




   IEnumerator TypeText()
{
    introText.text = "";

    // ▶️ Avvia l'audio solo una volta
    if (scritturaAudio != null && scritturaAudio.clip != null)
    {
        scritturaAudio.loop = true;
        scritturaAudio.Play();
    }

    foreach (char c in fullText)
    {
        introText.text += c;
        yield return new WaitForSeconds(typingSpeed);
    }

    // 🛑 Ferma l’audio al termine dell’animazione
    if (scritturaAudio != null && scritturaAudio.isPlaying)
    {
        scritturaAudio.Stop();
        scritturaAudio.loop = false;
    }

    textCompleted = true;
    yield return new WaitForSeconds(1f);
    StartCoroutine(FadeInButton());
}




    IEnumerator FadeInButton()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeDuration);
            startButtonCanvasGroup.alpha = alpha;
            yield return null;
        }

        startButtonCanvasGroup.interactable = true;
        startButtonCanvasGroup.blocksRaycasts = true;
    }

    public void OnStartButtonClicked()
    {
        startButton.interactable = false;

        if (!string.IsNullOrEmpty(nomeScena) && !isFading)
        {
            StartCoroutine(FadeAndLoadScene(nomeScena));
        }
        else
        {
            Debug.LogError("Nome della scena non impostato o fade già in corso.");
        }
    }

    private IEnumerator FadeAndLoadScene(string scena)
    {
        isFading = true;

        if (fadePanelObject != null)
            fadePanelObject.SetActive(true);

        float timer = 0f;
        Color color = fadeImage.color;

        while (timer < fadeDuration)
        {
            float alpha = timer / fadeDuration;
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            timer += Time.deltaTime;
            yield return null;
        }

        // Assicura che l'alpha sia 1
        fadeImage.color = new Color(color.r, color.g, color.b, 1f);

        SceneManager.LoadScene(scena);
    }
}
