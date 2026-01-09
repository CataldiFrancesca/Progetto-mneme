using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class MinigiocoFrigoManager : MonoBehaviour,IMiniGameSaveable
{
    public static MinigiocoFrigoManager Instance;
    public Animator oggettoDaAprireAnimator;
    public Animator oggettoDaAprireAnimator2;
    [Header("UI TMP")]
    public TMP_Text riddleText;
    public TMP_InputField answerInput;
    public TMP_Text feedbackText;
    public Button inviaButton;
    public GameObject popupPanel;
    public GameObject puzzlePanel;
    public PuzzleManager puzzleManager;

    [Header("Risposta Corretta")]
    [Tooltip("La parola giusta per risolvere l'indovinello.")]
    public string rispostaCorretta = "microscopio";

    public GameObject pannelloMinigioco;
    public GameObject playerController;

    private CanvasGroup popupCanvasGroup;
    private bool minigiocoCompletato = false;
    private int count = 0;


    

    private void Awake()
    {
        // Singleton pattern base
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (popupPanel != null)
        {
            popupCanvasGroup = popupPanel.GetComponent<CanvasGroup>();
            if (popupCanvasGroup == null)
            {
                popupCanvasGroup = popupPanel.AddComponent<CanvasGroup>();
            }
            popupCanvasGroup.alpha = 0f;
            popupPanel.SetActive(false);
        }
    }

    public void AvviaGioco()
    {
        // Reset UI
        feedbackText.text = "";
        answerInput.text = "";
        answerInput.ActivateInputField();

        // Pulisci vecchi listener per sicurezza
        inviaButton.onClick.RemoveAllListeners();
    }

    public void ControllaRisposta()
    {
        string risposta = answerInput.text.Trim().ToLower();
        string corretta = rispostaCorretta.Trim().ToLower();

        if (risposta == corretta)
        {
            feedbackText.text = " Corretto!";
            minigiocoCompletato = true;
            feedbackText.fontMaterial.SetColor("_FaceColor", Color.green);
            inviaButton.interactable = false;
            StartCoroutine(MostraPopupDopoRitardo(1.5f));
            // TODO: Logica sblocco oggetto
        }
        else
        {
            feedbackText.text = " Sbagliato. Riprova!";
            feedbackText.fontMaterial.SetColor("_FaceColor", Color.red);
            StartCoroutine(NascondiFeedbackDopoRitardo(2f));
        }
    }

    IEnumerator MostraPopupDopoRitardo(float ritardo)
    {
        yield return new WaitForSeconds(ritardo);
        if (popupPanel != null)
        {
            popupPanel.SetActive(true);
            StartCoroutine(FadeInPopup(1.5f));
            
            
        }
    }

    private IEnumerator MostraRisultatoDopoDelay()
    {
        yield return new WaitForSeconds(2f); 

        puzzleManager?.CompleteMinigame(2);
        puzzlePanel.SetActive(true);
    }

    IEnumerator FadeInPopup(float duration)
    {
        if (popupCanvasGroup == null)
            yield break;

        popupCanvasGroup.alpha = 0f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            popupCanvasGroup.alpha = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }
        popupCanvasGroup.alpha = 1f;
    }

    IEnumerator NascondiFeedbackDopoRitardo(float ritardo)
    {
        yield return new WaitForSeconds(ritardo);
        feedbackText.text = "";
    }

    public MiniGameData GetSaveData()
    {
        return new MiniGameData
        {
            miniGameName = "MinigiocoFrigo",
            isActive = pannelloMinigioco.activeSelf,
            popupAttivo = popupPanel != null && popupPanel.activeSelf,
            punteggio = minigiocoCompletato ? 1 : 0
        };
    }

    public void LoadFromData(MiniGameData data)

    {
        Debug.Log("Caricamento dati per: " + data.miniGameName);
        pannelloMinigioco.SetActive(true); // Forza attivazione per setup iniziale

        minigiocoCompletato = data.punteggio > 0;

        if (minigiocoCompletato)
        {
            inviaButton.interactable = false;
            feedbackText.text = " Corretto!";
            feedbackText.fontMaterial.SetColor("_FaceColor", Color.green);
        }

        if (!data.isActive)
            StartCoroutine(DisattivaMinigiocoNextFrame());

        StartCoroutine(ApplicaPopupStato(data.popupAttivo));
    }

    private IEnumerator ApplicaPopupStato(bool showPopup)
{
    yield return null;

    if (popupPanel != null)
    {
        if (showPopup)
        {
            popupPanel.SetActive(true); // 🔁 PRIMA attiva il GameObject

            if (popupCanvasGroup != null)
                popupCanvasGroup.alpha = 1f;

            puzzleManager?.CompleteMinigame(2);
            puzzlePanel.SetActive(true);
        }
        else
        {
            popupPanel.SetActive(false); // altrimenti lo disattivi
            if (popupCanvasGroup != null)
                popupCanvasGroup.alpha = 0f;
        }
    }
}


    private IEnumerator DisattivaMinigiocoNextFrame()
    {
        yield return null; 
        pannelloMinigioco.SetActive(false);
    }

    public void EsciMinigioco()
    {
        Debug.Log("Uscita dal minigioco.");

        pannelloMinigioco.SetActive(false);

        if (playerController != null)
            playerController.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (oggettoDaAprireAnimator != null && popupPanel.activeSelf == true)
        {
            oggettoDaAprireAnimator.SetTrigger("Apri");
            oggettoDaAprireAnimator2.SetTrigger("Apri");
        }
        if (minigiocoCompletato && count == 0)
        {
            count++;
            StartCoroutine(MostraRisultatoDopoDelay());
        }

    }

}
