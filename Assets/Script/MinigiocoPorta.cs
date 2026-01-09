using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class MinigiocoPorta : MonoBehaviour, IMiniGameSaveable
{
    public static MinigiocoPorta Instance;

    public GameObject popupSuccesso;
    public GameObject canvasMinigioco;
    public TextMeshProUGUI feedbackText;
    public GameObject feedbackBackground;
    public GameObject playerController;
    public GameObject puzzlePanel;
    public PuzzleManager2 puzzleManager;

    public bool keyDroppedCorrectly = false;
    private Coroutine feedbackRoutine;
    private bool minigiocoCompletato = false;
    private int count = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        if (feedbackText == null)
        {
            Debug.LogWarning("⚠️ feedbackText non è assegnato su MinigiocoPorta!");
        }

        if (popupSuccesso != null)
            popupSuccesso.SetActive(false);
    }

    public void CompletaMinigioco()
    {
        keyDroppedCorrectly = true;
        minigiocoCompletato = true;

        MostraFeedbackPersistente("Hai trovato la serratura giusta!", Color.green);
        StartCoroutine(MostraPopupSuccessoConDelay());
        

        
    }

    private IEnumerator MostraRisultatoDopoDelay()
    {
        yield return new WaitForSeconds(2f); 

        puzzleManager?.CompleteMinigame(4);
        puzzlePanel.SetActive(true);
    }
    private IEnumerator MostraPopupSuccessoConDelay()
    {
        yield return new WaitForSeconds(1.5f);

        if (popupSuccesso != null)
            popupSuccesso.SetActive(true);
    }

    public void Fallimento()
    {
        keyDroppedCorrectly = false;
        MostraFeedbackTemporaneo("Serratura sbagliata! Riprova.", Color.red);
    }

    private void MostraFeedbackPersistente(string message, Color color)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
            feedbackText.color = color;
            feedbackText.gameObject.SetActive(true);
        }

        if (feedbackBackground != null)
            feedbackBackground.SetActive(true);
    }

    private void MostraFeedbackTemporaneo(string message, Color color)
    {
        if (feedbackRoutine != null)
            StopCoroutine(feedbackRoutine);

        feedbackRoutine = StartCoroutine(ShowFeedbackRoutine(message, color));
    }

    private IEnumerator ShowFeedbackRoutine(string message, Color color)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
            feedbackText.color = color;
            feedbackText.gameObject.SetActive(true);
        }

        if (feedbackBackground != null)
            feedbackBackground.SetActive(true);

        yield return new WaitForSeconds(2f);

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);
        if (feedbackBackground != null)
            feedbackBackground.SetActive(false);
    }

    public void ApriMinigioco()
    {
        canvasMinigioco?.SetActive(true);
        popupSuccesso?.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (minigiocoCompletato && popupSuccesso != null)
        {
            popupSuccesso.SetActive(true);
        }

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);
        if (feedbackBackground != null)
            feedbackBackground.SetActive(false);
    }

    public void EsciMinigioco()
    {
        canvasMinigioco?.SetActive(false);
        playerController?.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (minigiocoCompletato && count == 0)
        {
            count++;
            StartCoroutine(MostraRisultatoDopoDelay());
        }
    }

    // ✅ Salvataggio
    public MiniGameData GetSaveData()
    {
        if (!minigiocoCompletato)
        {
            return new MiniGameData
            {
                miniGameName = "MinigiocoPorta",
                jsonData = "",
                popupAttivo = false,
                punteggio = 0
            };
        }

        MiniGiocoPortaData data = new MiniGiocoPortaData
        {
            completato = true
        };

        string json = JsonUtility.ToJson(data);

        return new MiniGameData
        {
            miniGameName = "MinigiocoPorta",
            jsonData = json,
            popupAttivo = true,
            punteggio = 1
        };
    }

    // ✅ Caricamento
    public void LoadFromData(MiniGameData data)
    {
        if (string.IsNullOrEmpty(data.jsonData)) return;

        MiniGiocoPortaData portaData = JsonUtility.FromJson<MiniGiocoPortaData>(data.jsonData);

        if (portaData.completato)
        {
            minigiocoCompletato = true;
            popupSuccesso?.SetActive(true);
            puzzleManager?.CompleteMinigame(4);
            puzzlePanel?.SetActive(true);
        }
    }
}

// ✅ Classe per i dati di salvataggio (aggiungila nel tuo progetto)
[System.Serializable]
public class MiniGiocoPortaData
{
    public bool completato;
}
