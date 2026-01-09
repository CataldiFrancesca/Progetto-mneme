using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LockManager : MonoBehaviour, IMiniGameSaveable
{
    [Header("UI Elements")]
    public TMP_InputField codiceInputField;
    public Button verificaButton;
    public Button esciButton;
    public GameObject pannelloLucchetto;
    public CanvasGroup fadeGroup;
    public CanvasGroup immagineEsci;
    public GameObject puzzlePanel;
    public PuzzleManager2 puzzleManager;
    public TMP_Text feedbackText; // Campo per il feedback visivo


    [Header("Minigioco")]
    public GameObject pannelloMinigioco;
    public GameObject playerController;

    [Header("Impostazioni")]
    public string codiceCorretto = "1234";

    [Header("Animazioni Porte")]
    public Animator portaSinistraAnimator;
    public Animator portaDestraAnimator;

    [Header("GameObject Porte")]
    public GameObject portaSinistraGO;
    public GameObject portaDestraGO;

    [Header("Nome Script Interazione Porte")]
    public string nomeScriptInterazionePortaSinistra = "InterazionePorta";
    public string nomeScriptInterazionePortaDestra = "InterazionePorta";

    private bool completato = false;
    private int count = 0;

    void Start()
    {
        verificaButton.onClick.AddListener(VerificaCodice);
        esciButton.onClick.AddListener(EsciMinigioco);

        if (!completato)
        {
            pannelloLucchetto.SetActive(true);
            DisattivaScriptInterazione(portaSinistraGO, nomeScriptInterazionePortaSinistra);
            DisattivaScriptInterazione(portaDestraGO, nomeScriptInterazionePortaDestra);
        }

        if (fadeGroup != null) fadeGroup.alpha = 1f;

        if (immagineEsci != null && !completato)
        {
            immagineEsci.alpha = 0f;
            immagineEsci.gameObject.SetActive(false);
        }


    }

    void VerificaCodice()
    {
        string codiceInserito = codiceInputField.text;

        if (codiceInserito == codiceCorretto)
        {
            completato = true;

            if (feedbackText != null)
            {
                feedbackText.text = "Codice corretto!";
                feedbackText.color = Color.green; // ✅ Verde per corretto
            }

            StartCoroutine(ShowImmagineEsciDelayed(1.5f));

            if (portaSinistraAnimator != null)
                portaSinistraAnimator.SetTrigger("TrOpen");

            if (portaDestraAnimator != null)
                portaDestraAnimator.SetTrigger("TrOpen");

            AttivaScriptInterazione(portaSinistraGO, nomeScriptInterazionePortaSinistra);
            AttivaScriptInterazione(portaDestraGO, nomeScriptInterazionePortaDestra);

            
        }
        else
        {
            if (feedbackText != null)
            {
                feedbackText.text = "Codice errato!";
                feedbackText.color = Color.red; // ❌ Rosso per errore
            }
        }
    }



    private IEnumerator MostraRisultatoDopoDelay()
    {
        yield return new WaitForSeconds(2f); 

        puzzleManager?.CompleteMinigame(1);
        puzzlePanel.SetActive(true);
    }

    IEnumerator ShowImmagineEsciDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (immagineEsci != null)
        {
            immagineEsci.gameObject.SetActive(true);
            StartCoroutine(FadeIn(immagineEsci, 1.5f));
        }
    }

    IEnumerator FadeIn(CanvasGroup group, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }
        group.alpha = 1f;
    }

    public void EsciMinigioco()
    {
        if (pannelloMinigioco != null)
            pannelloMinigioco.SetActive(false);

        if (playerController != null)
            playerController.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (completato && count == 0)
        {
            count++;
            StartCoroutine(MostraRisultatoDopoDelay());
        }
    }

    void DisattivaScriptInterazione(GameObject porta, string nomeScript)
    {
        if (porta == null) return;

        var script = porta.GetComponent(nomeScript) as MonoBehaviour;
        if (script != null)
            script.enabled = false;
    }

    void AttivaScriptInterazione(GameObject porta, string nomeScript)
    {
        if (porta == null) return;

        var script = porta.GetComponent(nomeScript) as MonoBehaviour;
        if (script != null)
            script.enabled = true;
    }

    // ✅ Salvataggio
    public MiniGameData GetSaveData()
    {
        return new MiniGameData
        {
            miniGameName = "LockManager",
            punteggio = completato ? 1 : 0,
            popupAttivo = immagineEsci != null && immagineEsci.gameObject.activeSelf
        };
    }

    // ✅ Caricamento
    public void LoadFromData(MiniGameData data)
    {
        completato = data.punteggio > 0;

        if (completato)
        {
            // Ripristina stato porte e UI
            if (pannelloLucchetto != null) pannelloLucchetto.SetActive(true);
            if (portaSinistraAnimator != null) portaSinistraAnimator.SetTrigger("TrOpen");
            if (portaDestraAnimator != null) portaDestraAnimator.SetTrigger("TrOpen");

            AttivaScriptInterazione(portaSinistraGO, nomeScriptInterazionePortaSinistra);
            AttivaScriptInterazione(portaDestraGO, nomeScriptInterazionePortaDestra);



            if (data.punteggio> 0 || data.popupAttivo)
            {
                immagineEsci.gameObject.SetActive(true);
                immagineEsci.alpha = 1f;
                puzzleManager?.CompleteMinigame(1);
                if (puzzlePanel != null)
                    puzzlePanel.SetActive(true);
            }
        }
        else 
        {
            verificaButton.onClick.AddListener(VerificaCodice);
            esciButton.onClick.AddListener(EsciMinigioco);

            if (!completato)
            {
                pannelloLucchetto.SetActive(true);
                DisattivaScriptInterazione(portaSinistraGO, nomeScriptInterazionePortaSinistra);
                DisattivaScriptInterazione(portaDestraGO, nomeScriptInterazionePortaDestra);
            }

            if (fadeGroup != null) fadeGroup.alpha = 1f;

            if (data.punteggio == 0 || !data.popupAttivo)
            {
                immagineEsci.alpha = 0f;
                immagineEsci.gameObject.SetActive(false);
            }
        
        }
    }
}
