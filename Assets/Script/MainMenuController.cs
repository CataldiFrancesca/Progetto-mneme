using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [Header("Pulsanti")]
    public Button nuovaPartitaButton;
    public Button caricaPartitaButton;
    public Button opzioniButton;
    public Button esciButton;

    [Header("Canvas / Popup")]
    public GameObject opzioniCanvas;          
    public GameObject confermaUscitaCanvas;

    [Header("Scene")]
    [Tooltip("Nome esatto della scena da caricare per iniziare il gioco.")]
    public string nomeScenaGioco = "IntroIniziale";

    [Header("Salvataggio")]
    public string saveFileName = "salvataggio.json"; 
    
    public Image fadeImage;
    public float fadeDuration = 2f; 
    private bool isFading = false;



    private void Start()
    {
        string folderPath = Path.Combine(Application.persistentDataPath, "Salvataggi");
        string path = Path.Combine(folderPath, saveFileName);

        caricaPartitaButton.interactable = File.Exists(path);

        nuovaPartitaButton.onClick.AddListener(NuovaPartita);
        caricaPartitaButton.onClick.AddListener(CaricaPartita);
        opzioniButton.onClick.AddListener(MostraOpzioni);
        esciButton.onClick.AddListener(EsciGioco);

        if (opzioniCanvas != null)
            opzioniCanvas.SetActive(false);

        if (confermaUscitaCanvas != null)
            confermaUscitaCanvas.SetActive(false);
    }

    

public void NuovaPartita()
{
    if (!string.IsNullOrEmpty(nomeScenaGioco) && !isFading)
    {
        StartCoroutine(FadeAndLoadScene(nomeScenaGioco));
    }
    else if (string.IsNullOrEmpty(nomeScenaGioco))
    {
        Debug.LogError("Nome della scena di gioco non impostato.");
    }
}

    private IEnumerator FadeAndLoadScene(string scena)
    {
        isFading = true;

        float timer = 0f;
        Color color = fadeImage.color;

        while (timer < fadeDuration)
        {
            float alpha = timer / fadeDuration;
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            timer += Time.deltaTime;
            yield return null;
        }

        // Assicura alpha a 1
        fadeImage.color = new Color(color.r, color.g, color.b, 1f);

        SceneManager.LoadScene(scena);
    }

    public void CaricaPartita()
    {
        string folderPath = Path.Combine(Application.persistentDataPath, "Salvataggi");
        string savePath = Path.Combine(folderPath, "salvataggio.json");

        if (!File.Exists(savePath))
        {
            Debug.LogWarning("Nessun file di salvataggio trovato.");
            return;
        }

        string json = File.ReadAllText(savePath);
        SaveData dati = JsonUtility.FromJson<SaveData>(json);

        if (GameLoader.Instance == null)
        {
            GameObject loaderGO = new GameObject("GameLoader");
            loaderGO.AddComponent<GameLoader>();
        }

        GameLoader.Instance.datiCaricati = dati;

        if (!string.IsNullOrEmpty(dati.scenaCorrente) && !isFading)
        {
            StartCoroutine(FadeAndLoadScene(dati.scenaCorrente));
        }
        else if (string.IsNullOrEmpty(dati.scenaCorrente))
        {
            Debug.LogError("La scena salvata è vuota o nulla.");
        }
    }



    // ✅ Mostra pannello opzioni
    public void MostraOpzioni()
    {
        if (opzioniCanvas != null)
            opzioniCanvas.SetActive(true);
    }

    // ✅ Mostra pannello conferma uscita
    public void MostraConfermaUscita()
    {
        if (confermaUscitaCanvas != null)
            confermaUscitaCanvas.SetActive(true);
    }

    // ✅ Chiude il gioco
    public void EsciGioco()
    {
        Debug.Log("Chiusura del gioco...");
        Application.Quit();
    }

    // ✅ Chiude un panel passato come parametro (Torna indietro)
    public void TornaIndietro(GameObject panel)
    {
        if (panel != null)
            panel.SetActive(false);
    }

}
