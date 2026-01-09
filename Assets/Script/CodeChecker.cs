using UnityEngine;
using TMPro;  
using System.Collections;
using UnityEngine.UI;

public class CodeChecker : MonoBehaviour, IMiniGameSaveable
{
    public DigitSlot[] digitSlots;
    public string correctCode = "02052024";
    public TMP_Text feedbackText;
    public RectTransform trophyTopImage;
    public GameObject ticketImage;
    public CanvasGroup ticketCanvasGroup;
    public RectTransform trophyBottomImage;
    public RectTransform digitHolder;
    public RectTransform verificaButton;
    public RectTransform feedbackTextTransform;
    public Button verificaUIButton;
    public GameObject puzzlePanel; 
    public PuzzleManager2 puzzleManager;

    public GameObject playerController;
    public GameObject minigameUI;

    private bool completato = false;
    private int count = 0;

    public void CheckCode()
    {
        if (completato) return;

        string enteredCode = "";
        foreach (DigitSlot slot in digitSlots)
        {
            enteredCode += slot.GetCurrentNumber().ToString();
        }

        if (enteredCode == correctCode)
        {
            feedbackText.text = "Combinazione esatta!";
            verificaUIButton.interactable = false;
            completato = true;

            StartCoroutine(DelayBeforeOpenTrophy(1f));

        }
        else
        {
            feedbackText.text = "Codice non corretto, riprova.";
        }
    }

    private IEnumerator MostraRisultatoDopoDelay()
    {
        yield return new WaitForSeconds(2f); 

        puzzleManager?.CompleteMinigame(3);
        puzzlePanel.SetActive(true);
    }

    private IEnumerator DelayBeforeOpenTrophy(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        StartCoroutine(OpenTrophy());
    }

    public IEnumerator OpenTrophy()
    {
        Vector3 topStart = trophyTopImage.anchoredPosition;
        Vector3 topEnd = topStart + new Vector3(0, 100f, 0);

        Vector3 bottomStart = trophyBottomImage.anchoredPosition;
        Vector3 bottomEnd = bottomStart + new Vector3(0, -100f, 0);

        Vector3 holderStart = digitHolder.anchoredPosition;
        Vector3 holderEnd = holderStart + new Vector3(0, -100f, 0);

        Vector3 buttonStart = verificaButton.anchoredPosition;
        Vector3 buttonEnd = buttonStart + new Vector3(0, -100f, 0);

        Vector3 textStart = feedbackTextTransform.anchoredPosition;
        Vector3 textEnd = textStart + new Vector3(0, -100f, 0);

        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            trophyTopImage.anchoredPosition = Vector3.Lerp(topStart, topEnd, t);
            trophyBottomImage.anchoredPosition = Vector3.Lerp(bottomStart, bottomEnd, t);
            digitHolder.anchoredPosition = Vector3.Lerp(holderStart, holderEnd, t);
            verificaButton.anchoredPosition = Vector3.Lerp(buttonStart, buttonEnd, t);
            feedbackTextTransform.anchoredPosition = Vector3.Lerp(textStart, textEnd, t);

            yield return null;
        }

        float fadeDuration = 1f;
        float fadeElapsed = 0f;
        ticketCanvasGroup.alpha = 0f;
        ticketCanvasGroup.transform.localScale = Vector3.zero;

        while (fadeElapsed < fadeDuration)
        {
            fadeElapsed += Time.deltaTime;
            float t = fadeElapsed / fadeDuration;

            ticketCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            ticketCanvasGroup.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);

            yield return null;
        }

        ticketCanvasGroup.alpha = 1f;
        ticketCanvasGroup.transform.localScale = Vector3.one;
    }

    public void ResetAll()
    {
        foreach (DigitSlot slot in digitSlots)
        {
            slot.ResetDigit();
        }
        feedbackText.text = "";
    }

    public void OnEsciClicked()
    {
        if (minigameUI != null)
            minigameUI.SetActive(false);

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

    // ======= SALVATAGGIO =======

    public MiniGameData GetSaveData()
    {
        string codiceAttuale = "";
        foreach (var slot in digitSlots)
            codiceAttuale += slot.GetCurrentNumber().ToString();

        return new MiniGameData
        {
            miniGameName = "CodeCheckerMinigioco",
            isActive = minigameUI.activeSelf,
            popupAttivo = puzzlePanel != null && puzzlePanel.activeSelf,
            punteggio = completato ? 1 : 0,
            fraseUtente = codiceAttuale
        };
    }

    public void LoadFromData(MiniGameData data)
    {
        StartCoroutine(LoadRoutine(data));
    }

    private IEnumerator LoadRoutine(MiniGameData data)
    {
        minigameUI.SetActive(true);
        yield return null;

        if (!data.isActive)
            minigameUI.SetActive(false);

        if (data.isActive)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (playerController != null)
                playerController.SetActive(false);
        }

        puzzlePanel.SetActive(data.popupAttivo);
        completato = data.punteggio > 0;

        if (!string.IsNullOrEmpty(data.fraseUtente) && data.fraseUtente.Length == digitSlots.Length)
        {
            for (int i = 0; i < digitSlots.Length; i++)
            {
                digitSlots[i].SetDigit(int.Parse(data.fraseUtente[i].ToString()));
            }
        }

        if (completato)
        {
            feedbackText.text = "Combinazione esatta!";
            verificaUIButton.interactable = false;

            // Salta direttamente all'animazione finale
            ticketCanvasGroup.alpha = 1f;
            ticketCanvasGroup.transform.localScale = Vector3.one;
            puzzleManager?.CompleteMinigame(3);
            if (puzzlePanel != null)
                puzzlePanel.SetActive(true);
        }
        else
        {
            feedbackText.text = "";
            verificaUIButton.interactable = true;
        }
    }
}
