using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;  // Corretto namespace per TextMeshPro

public class TestTubeMixer : MonoBehaviour, IMiniGameSaveable
{
    public List<Button> testTubeButtons;
    public Image resultTestTubeImage;
    public Image targetColorDisplay;
    public Button mixButton;
    public TextMeshProUGUI resultText;  // Cambiato da Text a TextMeshProUGUI
    public Button exitButton;
    public GameObject puzzlePanel;
    public PuzzleManager2 puzzleManager;

    public Sprite emptyTestTubeSprite;

    [Header("Indizio (Image con CanvasGroup)")]
    public CanvasGroup indizioCanvasGroup;
    public GameObject popupSuccesso;  // <-- Questo sarà applicato all'Image

    // ** Aggiunti per uscita dal minigioco **
    public GameObject pannelloMinigioco;
    public GameObject playerController;

    private List<Color> selectedColors = new List<Color>();
    private bool combinazioneCorretta = false;
    private bool completato = false;
    private int count = 0;

    private Color targetColor = new Color(0.643f, 0.473f, 0.637f); // Magenta chiaro

    private List<Color> testTubeColors = new List<Color>
    {
        new Color(237f / 255f, 85f / 255f, 77f / 255f),    // Rosso
        new Color(77f / 255f, 158f / 255f, 221f / 255f),   // Blu
        new Color(242f / 255f, 203f / 255f, 67f / 255f),   // Giallo
        new Color(178f / 255f, 119f / 255f, 189f / 255f),  // Viola
        new Color(85f / 255f, 170f / 255f, 170f / 255f),   // Verde acqua
    };

    void Start()
    {
        for (int i = 0; i < testTubeButtons.Count; i++)
        {
            int capturedIndex = i;
            testTubeButtons[i].onClick.AddListener(() => SelectTestTube(capturedIndex));
        }

        mixButton.onClick.AddListener(MixColors);

        targetColorDisplay.color = targetColor;
        resultText.text = "";

        resultTestTubeImage.sprite = emptyTestTubeSprite;
        resultTestTubeImage.color = Color.white;

        if (exitButton != null)
        {
            exitButton.gameObject.SetActive(true);
            exitButton.onClick.AddListener(EsciMinigioco);  // listener per uscita
        }

        if (indizioCanvasGroup != null)
        {
            indizioCanvasGroup.alpha = 0f;
            indizioCanvasGroup.gameObject.SetActive(false);
        }
    }

    void SelectTestTube(int index)
    {
        if (selectedColors.Count >= 3) return;

        Color c = testTubeColors[index];
        selectedColors.Add(c);
        testTubeButtons[index].interactable = false;

        Debug.Log($"Provetta {index + 1} selezionata - R:{c.r:F2}, G:{c.g:F2}, B:{c.b:F2}");
    }

    public void MixColors()
    {
        if (selectedColors.Count != 3)
        {
            resultText.text = "Seleziona 3 provette!";
            return;
        }

        Color mixedColor = (selectedColors[0] + selectedColors[1] + selectedColors[2]) / 3f;

        float tolerance = 0.07f;

        combinazioneCorretta =
            Mathf.Abs(mixedColor.r - targetColor.r) < tolerance &&
            Mathf.Abs(mixedColor.g - targetColor.g) < tolerance &&
            Mathf.Abs(mixedColor.b - targetColor.b) < tolerance;

        if (combinazioneCorretta)
        {
            resultText.text = "Combinazione esatta!";
            completato = true;

            Sprite filledTube = Resources.Load<Sprite>("magenta_chiara");
            if (filledTube != null)
            {
                resultTestTubeImage.sprite = filledTube;
                resultTestTubeImage.color = Color.white;
            }

            if (exitButton != null)
                exitButton.gameObject.SetActive(true);

            if (indizioCanvasGroup != null)
                StartCoroutine(MostraIndizioConFade());
		 

        }
        else
        {
            resultText.text = "Colore sbagliato, riprova!";
            resultTestTubeImage.sprite = emptyTestTubeSprite;
            resultTestTubeImage.color = mixedColor;
        }

        StartCoroutine(ResetAfterDelay());
    }
    private IEnumerator MostraRisultatoDopoDelay()
    {
        yield return new WaitForSeconds(2f); 

        puzzleManager?.CompleteMinigame(0);
        puzzlePanel.SetActive(true);
    }

    private IEnumerator ResetAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);
        ResetSelection();
    }

    void ResetSelection()
    {
        selectedColors.Clear();
        foreach (var button in testTubeButtons)
        {
            button.interactable = true;
        }

        if (!combinazioneCorretta)
        {
            resultTestTubeImage.sprite = emptyTestTubeSprite;
            resultTestTubeImage.color = Color.white;
        }

        combinazioneCorretta = false;
    }

    private IEnumerator MostraIndizioConFade()
    {
        yield return new WaitForSeconds(1.5f);

        indizioCanvasGroup.gameObject.SetActive(true);
        indizioCanvasGroup.alpha = 0f;

        float duration = 1f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            indizioCanvasGroup.alpha = Mathf.Lerp(0f, 1f, time / duration);
            yield return null;
        }

        indizioCanvasGroup.alpha = 1f;
    }

    public void EsciMinigioco()
    {
        Debug.Log("Uscita dal minigioco.");

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


   public MiniGameData GetSaveData()
{
    // Salviamo i colori selezionati in formato esadecimale
    List<string> hexColors = new List<string>();
    foreach (var color in selectedColors)
    {
        hexColors.Add(ColorUtility.ToHtmlStringRGB(color));
    }

    return new MiniGameData
    {
        miniGameName = "Minigiocoprovette",
        isActive = pannelloMinigioco != null && pannelloMinigioco.activeSelf,
        popupAttivo = indizioCanvasGroup != null && indizioCanvasGroup.gameObject.activeSelf,
        punteggio = completato ? 1 : 0,
        fraseUtente = string.Join(",", hexColors)  // Salvo i colori selezionati
    };
}

public void LoadFromData(MiniGameData data)
{
    StartCoroutine(LoadRoutine(data));
}

private IEnumerator LoadRoutine(MiniGameData data)
{
    yield return null;

    // Riattiva il pannello minigioco solo se era attivo
    if (pannelloMinigioco != null)
        pannelloMinigioco.SetActive(data.isActive);

    // Blocca input se minigioco è attivo
    if (data.isActive && playerController != null)
        playerController.SetActive(false);

    Cursor.lockState = data.isActive ? CursorLockMode.None : CursorLockMode.Locked;
    Cursor.visible = data.isActive;

    selectedColors.Clear();
    foreach (var button in testTubeButtons)
        button.interactable = true;

    // Carico i colori selezionati (se presenti)
    if (!string.IsNullOrEmpty(data.fraseUtente))
    {
        string[] hexArray = data.fraseUtente.Split(',');
        foreach (string hex in hexArray)
        {
            if (ColorUtility.TryParseHtmlString("#" + hex, out Color c))
                selectedColors.Add(c);
        }
    }

    completato = data.punteggio > 0;

    if (completato)
    {
        resultText.text = "Combinazione esatta!";
        Sprite filledTube = Resources.Load<Sprite>("magenta_chiara");
        if (filledTube != null)
        {
            resultTestTubeImage.sprite = filledTube;
            resultTestTubeImage.color = Color.white;
        }

        exitButton?.gameObject.SetActive(true);
        puzzleManager?.CompleteMinigame(0);

        // Attiva pannelli di successo anche se il minigioco non è più attivo
        if (puzzlePanel != null)
            puzzlePanel.SetActive(true);

        if (popupSuccesso != null)
            popupSuccesso.SetActive(true);

        if (indizioCanvasGroup != null)
        {
            indizioCanvasGroup.gameObject.SetActive(true);
            indizioCanvasGroup.alpha = 1f;
        }
    }
    else
    {
        resultText.text = "Inserisci tre colori e premi Mix.";
        resultTestTubeImage.sprite = emptyTestTubeSprite;
        resultTestTubeImage.color = Color.white;

        if (puzzlePanel != null)
            puzzlePanel.SetActive(false);

        if (popupSuccesso != null)
            popupSuccesso.SetActive(false);

        if (indizioCanvasGroup != null)
        {
            indizioCanvasGroup.alpha = 0f;
            indizioCanvasGroup.gameObject.SetActive(false);
        }

        exitButton?.gameObject.SetActive(false);
    }
}





}
