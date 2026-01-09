using UnityEngine;
using TMPro;
using System.Collections;

public class HangmanGame : MonoBehaviour, IMiniGameSaveable
{
    public TextMeshProUGUI nameDisplayText;
    public TMP_InputField letterInputField;
    public TextMeshProUGUI statusText;
    public GameObject hangmanCanvas;
    public MonoBehaviour playerController;
    public MonoBehaviour cameraControlScript;
    public GameObject hintImageObject;
    public CanvasGroup hintImageCanvasGroup;
    public GameObject puzzlePanel;
    public PuzzleManager2 puzzleManager;

    private string fullName = "ROBERT THOMSON";
    private char[] revealedLetters;
    private bool[] lockedPositions;
    private bool completato = false;
    private int count = 0;

    void Start()
    {
        ResetGame();
    }

    public void CheckLetter()
    {
        if (completato) return;

        string input = letterInputField.text.ToUpper();
        letterInputField.text = "";

        if (input.Length != 1 || !char.IsLetter(input[0]))
        {
            statusText.text = "Inserisci una sola lettera valida.";
            return;
        }

        char guessedLetter = input[0];
        bool found = false;

        for (int i = 0; i < fullName.Length; i++)
        {
            if (!lockedPositions[i] && fullName[i] == guessedLetter)
            {
                revealedLetters[i] = guessedLetter;
                lockedPositions[i] = true;
                found = true;
            }
        }

        if (found)
            statusText.text = $"Lettera '{guessedLetter}' trovata!";
        else
            statusText.text = $"La lettera '{guessedLetter}' non è presente.";

        UpdateDisplay();

        if (AllLettersRevealed())
        {
            completato = true;
            statusText.text = "Hai indovinato tutte le lettere!";
            Invoke(nameof(ShowHintImage), 1.5f);
            

            letterInputField.interactable = false;
        }
    }

    private IEnumerator MostraRisultatoDopoDelay()
    {
        yield return new WaitForSeconds(2f); 

        puzzleManager?.CompleteMinigame(2);
        puzzlePanel.SetActive(true);
    }

    public void ResetGame()
    {
        revealedLetters = new char[fullName.Length];
        lockedPositions = new bool[fullName.Length];

        for (int i = 0; i < fullName.Length; i++)
        {
            if (fullName[i] == ' ')
            {
                revealedLetters[i] = ' ';
                lockedPositions[i] = true;
            }
            else if (fullName[i] == 'O' || fullName[i] == 'E' || fullName[i] == 'T')
            {
                revealedLetters[i] = '_';
                lockedPositions[i] = false;
            }
            else
            {
                revealedLetters[i] = fullName[i];
                lockedPositions[i] = true;
            }
        }

        UpdateDisplay();
        statusText.text = "Inserisci una lettera e premi Verifica.";
        letterInputField.text = "";
    }

    private bool AllLettersRevealed()
    {
        for (int i = 0; i < fullName.Length; i++)
        {
            if (!lockedPositions[i] && fullName[i] != ' ')
                return false;
        }
        return true;
    }

    private void ShowHintImage()
    {
        if (hintImageObject != null && hintImageCanvasGroup != null)
        {
            hintImageObject.SetActive(true);
            StartCoroutine(FadeInHintImage());
        }
    }

    private IEnumerator FadeInHintImage()
    {
        float duration = 2f;
        float elapsed = 0f;
        hintImageCanvasGroup.alpha = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            hintImageCanvasGroup.alpha = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        hintImageCanvasGroup.alpha = 1f;
    }

    public void ChiudiMinigioco()
    {
        if (hangmanCanvas != null)
        {
            hangmanCanvas.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (playerController != null)
                playerController.gameObject.SetActive(true);

            if (cameraControlScript != null)
                cameraControlScript.enabled = true;
            if (completato && count == 0)
            {
                count++;
                StartCoroutine(MostraRisultatoDopoDelay());
            }        
        }
    }

    private void UpdateDisplay()
    {
        nameDisplayText.text = string.Join(" ", revealedLetters);
    }

    // ======= SALVATAGGIO =======

    public MiniGameData GetSaveData()
    {
        return new MiniGameData
        {
            miniGameName = "HangmanMinigioco",
            isActive = hangmanCanvas.activeSelf,
            popupAttivo = puzzlePanel != null && puzzlePanel.activeSelf,
            punteggio = completato ? 1 : 0,
            fraseUtente = new string(revealedLetters)
        };
    }

    public void LoadFromData(MiniGameData data)
    {
        StartCoroutine(LoadRoutine(data));
    }

    private IEnumerator LoadRoutine(MiniGameData data)
    {
        hangmanCanvas.SetActive(true);
        yield return null;

        if (!data.isActive)
            hangmanCanvas.SetActive(false);

        if (data.isActive)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (playerController != null)
                playerController.gameObject.SetActive(false);

            if (cameraControlScript != null)
                cameraControlScript.enabled = false;
        }

        puzzlePanel.SetActive(data.popupAttivo);

        if (!string.IsNullOrEmpty(data.fraseUtente))
        {
            revealedLetters = data.fraseUtente.ToCharArray();
            lockedPositions = new bool[revealedLetters.Length];

            for (int i = 0; i < revealedLetters.Length; i++)
                lockedPositions[i] = (revealedLetters[i] != '_');

            UpdateDisplay();
        }

        completato = data.punteggio > 0;

        if (completato)
        {
            statusText.text = "Hai indovinato tutte le lettere!";
            letterInputField.interactable = false;
            hintImageObject.SetActive(true);
            hintImageCanvasGroup.alpha = 1f;
            puzzleManager?.CompleteMinigame(2);
            if (puzzlePanel != null)
                puzzlePanel.SetActive(true);
        }
        else
        {
            statusText.text = "Inserisci una lettera e premi Verifica.";
            letterInputField.interactable = true;
        }
    }
}
