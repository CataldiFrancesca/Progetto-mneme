using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class PhraseChecker : MonoBehaviour, IMiniGameSaveable
{
    [Header("UI Elements")]
    public TMP_InputField inputField;
    public Button checkButton;
    public TextMeshProUGUI resultText;

    [Header("Popup")]
    
    private CanvasGroup popupCanvasGroup;

    public GameObject puzzlePanel;
    public PuzzleManager puzzleManager;

    [Header("Gioco")]
    [TextArea]
    public string fraseCorretta = "la conoscenza è potere";

    public GameObject pannelloMinigioco;
    public GameObject playerController;
    private bool completato = false;
    private int count = 0;

    private void Start()
    {
        checkButton.onClick.AddListener(VerificaFrase);
        resultText.text = "";

        // Setup CanvasGroup per il popup

    }

    void VerificaFrase()
    {
        string fraseInserita = inputField.text.Trim().ToLower();

        if (fraseInserita == fraseCorretta.ToLower())
        {
            resultText.text = "Frase corretta!";
            resultText.fontMaterial.SetColor("_FaceColor", Color.green);

            // Disattiva il bottone verifica
            checkButton.interactable = false;
            completato = true;

            
            
        }
        else
        {
            resultText.text = "Frase sbagliata. Riprova...";
            resultText.fontMaterial.SetColor("_FaceColor", Color.red);
            StartCoroutine(NascondiFeedbackDopoRitardo(2f));
        }
    }


    

    private IEnumerator MostraRisultatoDopoDelay()
    {
        yield return new WaitForSeconds(2f); 

        puzzleManager?.CompleteMinigame(1);
        puzzlePanel.SetActive(true);
    }

    

    IEnumerator NascondiFeedbackDopoRitardo(float ritardo)
    {
        yield return new WaitForSeconds(ritardo);
        resultText.text = "";
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
            miniGameName = "FraseMinigioco",
            isActive = pannelloMinigioco.activeSelf,
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

    if (data.punteggio > 0)
    {
        

        if (popupCanvasGroup != null)
            popupCanvasGroup.alpha = 1f;

        puzzlePanel.SetActive(true);
        puzzleManager?.CompleteMinigame(1); // 👈 questo è fondamentale!
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
