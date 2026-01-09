using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CamiceGame : MonoBehaviour, IMiniGameSaveable
{
    [Header("UI Elements")]
    public TMP_InputField inputField;
    public TextMeshProUGUI feedbackText;
    public Button esciButton;
    public Button verificaButton;
    public GameObject panelGioco;

    public GameObject playerLookScript; 
    public GameObject indizioPanel;
    public GameObject puzzlePanel;
    public PuzzleManager puzzleManager;

    [Header("Correct Combination")]
    public string correctCombination = "123456";

    private bool completato = false;
    private int count = 0;

    void Start()
    {
        esciButton.onClick.AddListener(ChiudiGioco);
        indizioPanel.SetActive(false);
    }

    public void ApriGioco()
    {
        panelGioco.SetActive(true);
        feedbackText.text = "";

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (playerLookScript != null)
            playerLookScript.SetActive(false);
    }

    public void VerificaCombinazione()
    {
        if (completato) return;

        string userInput = inputField.text.Trim();

        if (userInput == correctCombination)
        {
            completato = true;
            feedbackText.text = "Combinazione esatta!";
            Invoke("MostraIndizioPanel", 1.5f);
            
            

            inputField.interactable = false;
            verificaButton.interactable = false;
        }
        else
        {
            feedbackText.text = "Combinazione non corretta, riprova.";
        }
    }

    private IEnumerator MostraRisultatoDopoDelay()
    {
        yield return new WaitForSeconds(2f); 

        puzzleManager?.CompleteMinigame(3);
        puzzlePanel.SetActive(true);
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

    private void MostraIndizioPanel()
    {
        indizioPanel.SetActive(true);
    }

    // ========== SALVATAGGIO ==========

    public MiniGameData GetSaveData()
    {
        return new MiniGameData
        {
            miniGameName = "CamiceMinigioco",
            isActive = panelGioco.activeSelf,
            popupAttivo = puzzlePanel != null && puzzlePanel.activeSelf,
            punteggio = completato ? 1 : 0,
            fraseUtente = inputField.text
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

        if ((data.popupAttivo || data.punteggio>0)&& puzzlePanel != null)
        {
            puzzlePanel.SetActive(true);
            puzzleManager?.CompleteMinigame(3);
        }
        else if (puzzlePanel != null)
        {
            puzzlePanel.SetActive(false);
        }

        if (!string.IsNullOrEmpty(data.fraseUtente))
            inputField.text = data.fraseUtente;

        completato = data.punteggio > 0;

        if (completato)
        {
            feedbackText.text = "Combinazione esatta!";
            inputField.interactable = false;
            verificaButton.interactable = false;
            indizioPanel.SetActive(true);
        }
        else
        {
            inputField.interactable = true;
            verificaButton.interactable = true;
        }
    }
}
