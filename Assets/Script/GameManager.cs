using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour, IMiniGameSaveable
{
    public static GameManager Instance;
    
    public TMP_InputField userInput;
    public TMP_Text feedbackText;
    public Button bottoneVerifica;
    public Button esciButton;

    public GameObject panelGioco;
    public GameObject playerLookScript;
    public GameObject playerController;
    public PuzzleManager puzzleManager;
    public GameObject puzzlePanel;

    private List<string> discoveredWords = new List<string>();
    private string correctPhrase = "cerca dove l'abito del sapere si veste di bianco";
    private bool completato = false;
    private int count = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void AddWord(string word)
    {
        if (!discoveredWords.Contains(word))
        {
            discoveredWords.Add(word);
        }
    }

    public void CheckPhrase()
    {
        if (completato) return; // Blocco ulteriore verifica se già completato

        string fraseUtente = userInput.text.Trim().ToLower();

        if (fraseUtente == correctPhrase.ToLower())
        {
            completato = true;
            feedbackText.text = "Hai trovato la frase! Chissà cosa può significare...";
            

            userInput.interactable = false;
            bottoneVerifica.interactable = false;
        }
        else
        {
            feedbackText.text = "Frase non corretta, riprova.";
        }
    }

    private IEnumerator MostraRisultatoDopoDelay()
{
    yield return new WaitForSeconds(2f); // aspetta 2 secondi

    puzzleManager?.CompleteMinigame(2);
    puzzlePanel.SetActive(true);
}


    public void OnClickVerifica()
    {
        CheckPhrase();
    }

    public void ChiudiGioco()
    {
        panelGioco.SetActive(false);
        feedbackText.text = "";

        if (playerLookScript != null)
            playerLookScript.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (completato && count == 0)
        {
            count++;
            StartCoroutine(MostraRisultatoDopoDelay());
        }
    }

    private void ApriGioco()
    {
        panelGioco.SetActive(true);
        feedbackText.text = "";

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (playerLookScript != null)
            playerLookScript.SetActive(false);
    }

    // ===== SALVATAGGIO =====

    public MiniGameData GetSaveData()
    {
        return new MiniGameData
        {
            miniGameName = "OcchialiPuzzleMinigioco",
            isActive = panelGioco.activeSelf,
            popupAttivo = puzzlePanel != null && puzzlePanel.activeSelf,
            punteggio = completato ? 1 : 0,
            fraseUtente = userInput.text
        };
    }

    public void LoadFromData(MiniGameData data)
    {
        StartCoroutine(LoadRoutine(data));
    }

    private IEnumerator LoadRoutine(MiniGameData data)
    {
        panelGioco.SetActive(true);
        yield return null;

        if (!data.isActive)
            panelGioco.SetActive(false);
        else
            ApriGioco();
            

        if ((data.popupAttivo||data.punteggio > 0) && puzzlePanel != null)
        {
            puzzlePanel.SetActive(true);
            puzzleManager?.CompleteMinigame(2);
        }
        else if (puzzlePanel != null)
        {
            puzzlePanel.SetActive(false);
        }

        if (!string.IsNullOrEmpty(data.fraseUtente))
            userInput.text = data.fraseUtente;

        completato = data.punteggio > 0;

        if (completato)
        {
            feedbackText.text = "Hai trovato la frase! Chissà cosa può significare...";
            userInput.interactable = false;
            bottoneVerifica.interactable = false;
        }
        else
        {
            feedbackText.text = "";
            userInput.interactable = true;
            bottoneVerifica.interactable = true;
        }
    }
}
