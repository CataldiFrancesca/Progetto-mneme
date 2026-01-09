using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class MicroscopioGame : MonoBehaviour, IMiniGameSaveable
{
    public GameObject[] visteVetrino;     // 6 elementi - pannelli o immagini da mostrare/nascondere
    public Button[] bottoniVetrino;       // bottoni UI da assegnare in Inspector

    public TMP_InputField fraseInput;
    public TMP_Text feedbackText;
    public TMP_Text titoloText;
    public GameObject puzzlePanel;
    public PuzzleManager2 puzzleManager;
    



    public string fraseCorretta = "I virus non sono cellule viventi ma entità biologiche uniche";

    public Image immagineFade;

    public GameObject pannelloMinigioco;
    public GameObject playerController;

    public Button buttonEsci;

    private bool minigiocoCompletato = false;
    private int count = 0;


    void Start()
    {
        feedbackText.text = "";
        titoloText.text = "Trova la frase corretta";

        if (immagineFade != null)
        {
            Color c = immagineFade.color;
            c.a = 0f;
            immagineFade.color = c;
            immagineFade.gameObject.SetActive(false);
        }

        if (buttonEsci != null)
            buttonEsci.onClick.AddListener(Esci);

        // Attiva solo il primo vetrino all’inizio
        MostraVistaVetrino(0);

        // Assicura che i bottoni siano attivi e assegna il listener corretto
        for (int i = 0; i < bottoniVetrino.Length; i++)
        {
            int index = i; // importante per closure
            if (bottoniVetrino[i] != null)
            {
                bottoniVetrino[i].gameObject.SetActive(true);

                bottoniVetrino[i].onClick.AddListener(() => MostraVistaVetrino(index));
            }
        }
    }

    public void MostraVistaVetrino(int index)
    {
        for (int i = 0; i < visteVetrino.Length; i++)
        {
            if (visteVetrino[i] != null)
            {
                bool daMostrare = (i == index);
                visteVetrino[i].SetActive(daMostrare);

                var cg = visteVetrino[i].GetComponent<CanvasGroup>();
                if (cg != null)
                {
                    cg.alpha = daMostrare ? 1f : 0f;
                    cg.blocksRaycasts = daMostrare;
                    cg.interactable = daMostrare;
                }
            }
        }
    }


    public void ControllaFrase()
    {
        StartCoroutine(ControllaDopoFrame());
    }

    private IEnumerator ControllaDopoFrame()
    {
        yield return null;

        string fraseUtente = NormalizeString(fraseInput.text);
        string fraseCorrettaNorm = NormalizeString(fraseCorretta);

        if (string.IsNullOrEmpty(fraseUtente))
        {
            feedbackText.text = "Inserisci una frase.";
            yield break;
        }

        if (fraseUtente.Equals(fraseCorrettaNorm, System.StringComparison.OrdinalIgnoreCase))
        {
            feedbackText.text = "Frase corretta!";
            fraseInput.interactable = false;

            yield return new WaitForSeconds(1.5f);
             minigiocoCompletato = true;


            if (immagineFade != null)
            {
                immagineFade.gameObject.SetActive(true);
                yield return StartCoroutine(FadeInImage(immagineFade, 1.5f));
               

            }
        }
        else
        {
            feedbackText.text = "Frase errata. Riprova.";
        }
    }

    private IEnumerator MostraRisultatoDopoDelay()
    {
        yield return new WaitForSeconds(2f); 

        puzzleManager?.CompleteMinigame(2);
        puzzlePanel.SetActive(true);
    }

    private string NormalizeString(string s)
    {
        return Regex.Replace(s.Trim(), @"\s+", " ");
    }

    private IEnumerator FadeInImage(Image img, float durata)
    {
        img.color = new Color(img.color.r, img.color.g, img.color.b, 0f);

        float intervallo = 0.05f;
        int stepCount = Mathf.CeilToInt(durata / intervallo);

        for (int i = 0; i <= stepCount; i++)
        {
            float alpha = (float)i / stepCount;
            img.color = new Color(img.color.r, img.color.g, img.color.b, alpha);
            yield return new WaitForSeconds(intervallo);
        }
    }

    public void Esci()
    {
        Debug.Log("Uscita dal minigioco.");

        if (pannelloMinigioco != null)
            pannelloMinigioco.SetActive(false);

        if (playerController != null)
            playerController.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (minigiocoCompletato && count == 0)
        {
            count++;
            StartCoroutine(MostraRisultatoDopoDelay());
        }
    }
    
    public MiniGameData GetSaveData()
{
    return new MiniGameData
    {
        miniGameName = "MicroscopioGame",
        fraseUtente = fraseInput.text,
        popupAttivo = minigiocoCompletato,
        punteggio = minigiocoCompletato ? 1 : 0,
        jsonData = JsonUtility.ToJson(new MicroscopioGameStatusData
        {
            completato = minigiocoCompletato
        })
    };
}
public void LoadFromData(MiniGameData data)
{
    if (!string.IsNullOrEmpty(data.fraseUtente))
    {
        fraseInput.text = data.fraseUtente;
    }

    if (!string.IsNullOrEmpty(data.jsonData))
    {
        MicroscopioGameStatusData stato = JsonUtility.FromJson<MicroscopioGameStatusData>(data.jsonData);

        if (stato.completato)
        {
            minigiocoCompletato = true;
            fraseInput.interactable = false;
            feedbackText.text = "Frase corretta!";
            immagineFade?.gameObject.SetActive(true);
            puzzleManager?.CompleteMinigame(2);
            puzzlePanel?.SetActive(true);
        }
    }
}

}
