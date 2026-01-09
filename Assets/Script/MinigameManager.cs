using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class MinigameManager : MonoBehaviour, IMiniGameSaveable
{
    [Header("Frase corretta da indovinare")]
    [TextArea]
    public string fraseCorretta = "nelle fotografie si celano ricordi";

    [Header("Input dell'utente")]
    public TMP_InputField inputField;

    [Header("Testo di feedback")]
    public TextMeshProUGUI resultText;

    [Header("Bottoni")]
    public Button bottoneConferma;
    public Button bottoneEsci;
    public TextMeshProUGUI bottoneEsciText;

    [Header("Minigioco")]
    public GameObject pannelloMinigioco;

    [Header("Controllo giocatore")]
    public GameObject playerController;
    public GameObject puzzlePanel;
    public PuzzleManager puzzleManager;

    private bool completato = false;
    private int count = 0;

    private void Start()
    {
        bottoneConferma.onClick.AddListener(ConfermaFrase);
    }

    public void ConfermaFrase()
    {
        string fraseUtente = inputField.text.Trim().ToLower();

        if (fraseUtente == fraseCorretta.ToLower())
        {
            resultText.text = "Frase corretta! Cosa potrà significare?";
            resultText.color = Color.green;

            completato = true;
            
            
        }
        else
        {
            resultText.text = "Frase errata, riprova.";
            resultText.color = Color.red;
        }
    }

    private IEnumerator MostraRisultatoDopoDelay()
    {
        yield return new WaitForSeconds(2f); 

        puzzleManager?.CompleteMinigame(0);
        puzzlePanel.SetActive(true);
    }



    public void EsciMinigioco()
    {
        Debug.Log("Uscita dal minigioco.");
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

    // ========== SALVATAGGIO ==========

    public MiniGameData GetSaveData()
    {
        return new MiniGameData
        {
            miniGameName = "FrasePuzzleMinigioco",
            isActive = pannelloMinigioco.activeSelf,
            popupAttivo = puzzlePanel != null && puzzlePanel.activeSelf,
            punteggio = completato ? 1 : 0
        };
    }

    public void LoadFromData(MiniGameData data)
    {
        pannelloMinigioco.SetActive(true);
        StartCoroutine(RipristinaStatoDopoFrame(data));
    }

    private IEnumerator RipristinaStatoDopoFrame(MiniGameData data)
    {
        yield return null;

        if ((data.popupAttivo || data.punteggio > 0) && puzzlePanel != null)
        {
            puzzlePanel.SetActive(true);
            puzzleManager?.CompleteMinigame(0);
            resultText.text = "Frase corretta! Cosa potrà significare?";
            resultText.color = Color.green;
            completato = true;
        }

        if (!data.isActive)
            StartCoroutine(DisattivaMinigiocoNextFrame());
    }

    private IEnumerator DisattivaMinigiocoNextFrame()
    {
        yield return null;
        pannelloMinigioco.SetActive(false);
    }
}
