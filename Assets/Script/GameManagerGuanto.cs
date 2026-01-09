using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class GameManagerGuanto : MonoBehaviour, IMiniGameSaveable
{
    public static GameManagerGuanto Instance;

    public RectTransform targetArea;

    public TextMeshProUGUI counterTMP;    
    public TextMeshProUGUI feedbackTMP;   

    public GameObject popupImage;  
    
    public GameObject puzzlePanel;
    public PuzzleManager puzzleManager;       

    private int correctCount = 0;
    private const int maxCorrect = 6;
    private Coroutine feedbackCoroutine;

    public GameObject pannelloMinigioco;
    public GameObject playerController;

    public List<ProvettaDragHandler> tutteLeProvette;


    private List<string> provetteTrovate = new List<string>();


    private Color originalColor;

    private CanvasGroup popupCanvasGroup;
    private bool completato = false;
    private int count = 0;

    private void Awake()
    {
        Instance = this;

        if (feedbackTMP != null)
        {
            feedbackTMP.gameObject.SetActive(false);
            originalColor = feedbackTMP.fontMaterial.GetColor("_FaceColor");
        }

        if (popupImage != null)
        {
            popupImage.SetActive(false);

            // Assicuriamoci che popupImage abbia un CanvasGroup per il fade-in
            popupCanvasGroup = popupImage.GetComponent<CanvasGroup>();
            if (popupCanvasGroup == null)
            {
                popupCanvasGroup = popupImage.AddComponent<CanvasGroup>();
            }
            popupCanvasGroup.alpha = 0f; // Inizialmente invisibile
        }
    }

    public bool IsOverTarget(PointerEventData eventData)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(targetArea, eventData.position, eventData.enterEventCamera);
    }

    public void HandleCorrectProvetta(ProvettaDragHandler provetta)
{
    provetta.ReturnToStart();
    provetta.ShowCheck();

    if (!provetteTrovate.Contains(provetta.provettaID))
        provetteTrovate.Add(provetta.provettaID);

    correctCount++;
    UpdateCounter();

    if (correctCount >= maxCorrect)
    {
        completato = true;
        ShowFeedback("Hai trovato tutte le provette", 1.5f, true, true);
    }
}


    public void HandleWrongProvetta(ProvettaDragHandler provetta)
    {
        provetta.ReturnToStart();
        ShowFeedback("Provetta sbagliata!", 2f, false, false);
    }

    private void UpdateCounter()
    {
        counterTMP.text = $"{correctCount}/{maxCorrect}";
    }

    private void ShowFeedback(string message, float duration, bool showPopupAfter = false, bool isPositive = true)
    {
        if (feedbackCoroutine != null)
            StopCoroutine(feedbackCoroutine);

        feedbackCoroutine = StartCoroutine(FeedbackRoutine(message, duration, showPopupAfter, isPositive));
    }

    private IEnumerator FeedbackRoutine(string message, float duration, bool showPopupAfter, bool isPositive)
    {
        feedbackTMP.text = message;
        feedbackTMP.gameObject.SetActive(true);

        Color colorToSet = isPositive ? Color.green : Color.red;
        feedbackTMP.fontMaterial.SetColor("_FaceColor", colorToSet);

        yield return new WaitForSeconds(duration);

        feedbackTMP.gameObject.SetActive(false);
        feedbackTMP.fontMaterial.SetColor("_FaceColor", originalColor);

        if (showPopupAfter && popupImage != null)
        {


            popupImage.SetActive(true);
            StartCoroutine(FadeInPopup(1.5f));
           
            
        }
    }
    private IEnumerator MostraRisultatoDopoDelay()
    {
        yield return new WaitForSeconds(2f); 

        puzzleManager?.CompleteMinigame(0);
        puzzlePanel.SetActive(true);
    }

    private IEnumerator FadeInPopup(float fadeDuration)
    {
        float elapsed = 0f;
        popupCanvasGroup.alpha = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            popupCanvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }

        popupCanvasGroup.alpha = 1f; // assicurati che finisca completamente visibile
    }

    public MiniGameData GetSaveData()
{
    return new MiniGameData
    {
        miniGameName = "GuantoMinigioco",
        isActive = pannelloMinigioco.activeSelf,
        punteggio = correctCount,
        provetteTrovate = new List<string>(provetteTrovate),
        popupAttivo = popupImage != null && popupImage.activeSelf // ⬅️ Salva se è attivo
    };
}




    public void LoadFromData(MiniGameData data)
    {
        pannelloMinigioco.SetActive(true); // << forza attivazione per inizializzare provette
        correctCount = data.punteggio;
        provetteTrovate = new List<string>(data.provetteTrovate);
        UpdateCounter();

        StartCoroutine(ApplyProvettaStatesAfterDelay(data.popupAttivo));

        

        // Se il minigioco non deve restare attivo, disattivalo dopo
        if (!data.isActive)
            StartCoroutine(DisableMinigiocoNextFrame());
    }


    private IEnumerator DisableMinigiocoNextFrame()
    {
        yield return null; // aspetta un frame
        pannelloMinigioco.SetActive(false);
    }




    private IEnumerator ApplyProvettaStatesAfterDelay(bool showPopup)
    {
        yield return null;

        Debug.Log($"[ApplyProvettaStates] Provette assegnate manualmente: {tutteLeProvette.Count}");
        Debug.Log($"[ApplyProvettaStates] Provette salvate: {string.Join(", ", provetteTrovate)}");

        foreach (var provetta in tutteLeProvette)
        {
            if (provetta != null && provetteTrovate.Contains(provetta.provettaID))
            {
                Debug.Log($"→ Applico stato a: {provetta.provettaID}");
                provetta.ShowCheck();
                provetta.isDraggable = false;
            }
        }

        if (showPopup && popupImage != null)
        {
            popupImage.SetActive(true);
            popupCanvasGroup.alpha = 1f; // mostra subito senza fade
            puzzleManager?.CompleteMinigame(0);
            puzzlePanel.SetActive(true);
        }
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
}
