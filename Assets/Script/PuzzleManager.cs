using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class PuzzleManager : MonoBehaviour
{
    [System.Serializable]
    public class MinigameStep
    {
        public int minigameIndex;             // Indice del minigioco completato
        public BoxCollider colliderToUnlock;  // Collider da attivare per il minigioco successivo
    }

    [Header("Sblocchi Collider")]
    public List<MinigameStep> unlockSteps = new List<MinigameStep>();

    public Image[] puzzlePieces;
    public Button nextCameraButton;
    public GameObject puzzlePanel;               
    public CanvasGroup puzzleCanvasGroup;        
    public Button esciButton;

    public GameObject playerController;

    public List<GameObject> Canvas = new List<GameObject>();
    public Button rotatePhotoButton;
    public Image puzzleBackImage; 
    public float rotationDuration = 1f;  
    public Image fadeImage;
    public float fadeDuration = 1.5f;
    private bool isFading = false;


   

    private bool[] unlockedPieces = new bool[4];
    private bool[] hasAnimatedPiece = new bool[4];  // 🔄 Tiene traccia dei pezzi già animati
    private int lastUnlockedIndex = -1;

    void Start()
    {
        esciButton.onClick.AddListener(ChiudiGioco);

        puzzlePanel.SetActive(false);           
        puzzleCanvasGroup.alpha = 0f;            
        puzzlePanel.transform.localScale = Vector3.one * 0.8f; 
        rotatePhotoButton.onClick.AddListener(RuotaFoto);
        rotatePhotoButton.gameObject.SetActive(false);
        puzzleBackImage.gameObject.SetActive(false);
        if (fadeImage != null)
        {
            fadeImage.color = new Color(0f, 0f, 0f, 0f);
            fadeImage.gameObject.SetActive(false);
        }






        foreach (var piece in puzzlePieces)
        {
            piece.enabled = false;
            piece.transform.localScale = Vector3.one;
        }

        nextCameraButton.gameObject.SetActive(false);
    }

    public void CompleteMinigame(int index)
    {
        if (index >= 0 && index < unlockedPieces.Length)
        {
            // Evita di ripetere l'animazione se già fatto
            if (unlockedPieces[index] && hasAnimatedPiece[index])
                return;

            unlockedPieces[index] = true;
            lastUnlockedIndex = index;
            // 🔓 Sblocca collider del minigioco successivo
            foreach (var step in unlockSteps)
            {
                if (step.minigameIndex == index && step.colliderToUnlock != null)
                {
                    step.colliderToUnlock.enabled = true;
                    Debug.Log($"Sbloccato collider per il minigioco dopo l’indice {index}");
                }
            }

            StartCoroutine(ShowPuzzleWithAnimation(2f));
        }
    }

    private IEnumerator ShowPuzzleWithAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);

        puzzlePanel.SetActive(true);

        for (int j = 0; j < 4; j++)
        {
            if (!Canvas[j].activeSelf)
            {
                playerController.SetActive(false);

            }
        }

        // Reset animazioni canvas
        puzzleCanvasGroup.alpha = 0f;
        puzzlePanel.transform.localScale = Vector3.one * 0.8f;

        float duration = 0.8f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            puzzleCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            puzzlePanel.transform.localScale = Vector3.Lerp(Vector3.one * 0.8f, Vector3.one, t);

            yield return null;
        }

        puzzleCanvasGroup.alpha = 1f;
        puzzlePanel.transform.localScale = Vector3.one;

        // Mostra gli altri pezzi (senza animarli)
        for (int i = 0; i < puzzlePieces.Length; i++)
        {
            if (unlockedPieces[i] && i != lastUnlockedIndex)
            {
                puzzlePieces[i].enabled = true;
                puzzlePieces[i].transform.localScale = Vector3.one;
            }
        }

        // ⏱ Dopo 1s anima solo il nuovo pezzo se non è già stato animato
        yield return new WaitForSeconds(1f);

        if (lastUnlockedIndex >= 0 && lastUnlockedIndex < puzzlePieces.Length && !hasAnimatedPiece[lastUnlockedIndex])
        {
            Image piece = puzzlePieces[lastUnlockedIndex];
            piece.enabled = true;
            piece.transform.localScale = Vector3.one * 0.2f;

            float zoomDuration = 0.4f;
            float zoomTime = 0f;

            while (zoomTime < zoomDuration)
            {
                zoomTime += Time.deltaTime;
                float t = zoomTime / zoomDuration;

                piece.transform.localScale = Vector3.Lerp(Vector3.one * 0.2f, Vector3.one, t);
                yield return null;
            }

            piece.transform.localScale = Vector3.one;
            hasAnimatedPiece[lastUnlockedIndex] = true; // 🔐 Segna come già animato
        }

        UpdatePuzzleUI();
    }

    private void UpdatePuzzleUI()
    {
        if (AllPiecesUnlocked())
        {
            rotatePhotoButton.gameObject.SetActive(true);
            esciButton.gameObject.SetActive(false);
            nextCameraButton.gameObject.SetActive(false); // Mostrato dopo rotazione
        }
    }

    private void RuotaFoto()
    {
        rotatePhotoButton.gameObject.SetActive(false); // Nasconde il pulsante
        StartCoroutine(RotatePuzzle());
    }

    private IEnumerator RotatePuzzle()
{
    float totalDuration = 1.5f;                  // ⏳ Durata totale più lenta
    float halfDuration = totalDuration / 2f;
    Vector3 originalScale = puzzlePanel.transform.localScale;

    // 🔁 Prima metà: scala X da 1 a 0 (come se girasse via)
    float elapsed = 0f;
    while (elapsed < halfDuration)
    {
        elapsed += Time.deltaTime;
        float t = Mathf.SmoothStep(0f, 1f, elapsed / halfDuration);  // più morbido
        float scaleX = Mathf.Lerp(1f, 0f, t);
        puzzlePanel.transform.localScale = new Vector3(scaleX, 1f, 1f);
        yield return null;
    }

    // 🔄 Cambio faccia: mostra retro
    foreach (var piece in puzzlePieces)
        piece.gameObject.SetActive(false);

    puzzleBackImage.gameObject.SetActive(true);

    // 🔁 Seconda metà: scala X da 0 a 1 (apparizione del retro)
    elapsed = 0f;
    while (elapsed < halfDuration)
    {
        elapsed += Time.deltaTime;
        float t = Mathf.SmoothStep(0f, 1f, elapsed / halfDuration);
        float scaleX = Mathf.Lerp(0f, 1f, t);
        puzzlePanel.transform.localScale = new Vector3(scaleX, 1f, 1f);
        yield return null;
    }

    puzzlePanel.transform.localScale = originalScale;

    yield return new WaitForSeconds(2f);  // ⏱️ Aspetta prima di mostrare il bottone

    nextCameraButton.gameObject.SetActive(true);
}








    private bool AllPiecesUnlocked()
    {
        foreach (bool b in unlockedPieces)
        {
            if (!b) return false;
        }
        return true;
    }

    public void GoToNextCameraScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName) && !isFading)
        {
            StartCoroutine(FadeAndLoadScene(sceneName));
        }
        else if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Il nome della scena non è impostato!");
        }
    }




    private IEnumerator FadeAndLoadScene(string scena)
    {
        isFading = true;

        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            Color color = fadeImage.color;
            float timer = 0f;

            while (timer < fadeDuration)
            {
                float alpha = timer / fadeDuration;
                fadeImage.color = new Color(color.r, color.g, color.b, alpha);
                timer += Time.deltaTime;
                yield return null;
            }

            fadeImage.color = new Color(color.r, color.g, color.b, 1f);
        }

        SceneManager.LoadScene(scena);
    }



    public void ChiudiGioco()
    {
         int count = 0;
        for (int i = 0; i < 4; i++)
        {
            if (!Canvas[i].activeSelf)
            {
                count++;
                if (count == 4)
                {
                    playerController.SetActive(true);
                }

            }
        }
        puzzlePanel.SetActive(false);
        puzzleCanvasGroup.alpha = 0f;
    }
}
