using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MascherinaManager : MonoBehaviour, IMiniGameSaveable
{
    public GameObject[] immaginiMascherina;  // I 6 GameObject Image mascherina (attiva solo 1 alla volta)
    public Button[] pulsantiMascherina;       // I 6 pulsanti laterali
    public Button buttonConferma;
    public Button buttonEsci;                  // Pulsante Esci
    public TMP_Text textEnigma;
    public TMP_Text textFeedback;
    public GameObject pannelloMinigioco;
    public GameObject playerController;
    public GameObject puzzlePanel;
    public PuzzleManager2 puzzleManager;

    public Image immagineFade;                 // L'immagine che vuoi far comparire con fade-in

    public int indiceMascherinaCorrente = 4;
    public int indiceMascherinaCorretta = 0;  // la prima mascherina (indice 0) è quella giusta

    private bool completato = false;
    private int count = 0;

    void Start()
    {
        textFeedback.text = "";

        MostraMascherina(indiceMascherinaCorrente);

        // Associo a ogni pulsante la funzione per mostrare la mascherina corrispondente
        for (int i = 0; i < pulsantiMascherina.Length; i++)
        {
            int index = i;  // importante per la corretta chiusura della variabile nel listener
            pulsantiMascherina[i].onClick.AddListener(() => MostraMascherina(index));
        }

        // Associo al bottone conferma la verifica della scelta
        buttonConferma.onClick.AddListener(ControllaMascherina);

        // Associo al bottone esci la funzione EsciMinigioco
        if (buttonEsci != null)
            buttonEsci.onClick.AddListener(EsciMinigioco);

        // Imposto immagineFade invisibile all’inizio
        if (immagineFade != null)
        {
            Color c = immagineFade.color;
            c.a = 0f;
            immagineFade.color = c;
            immagineFade.gameObject.SetActive(false);
        }
    }

    void MostraMascherina(int index)
    {
        indiceMascherinaCorrente = index;

        for (int i = 0; i < immaginiMascherina.Length; i++)
        {
            immaginiMascherina[i].SetActive(i == index);
        }

        textFeedback.text = ""; // reset feedback ogni volta che cambio mascherina
    }

    void ControllaMascherina()
    {
        if (indiceMascherinaCorrente == indiceMascherinaCorretta)
        {
            completato = true;
            textFeedback.text = "Bravo! Hai scelto la mascherina giusta.";
            StartCoroutine(MostraFade(1.5f));
            
            
        }
        else
        {
            textFeedback.text = "Sbagliato, riprova.";
        }
    }

    private IEnumerator MostraRisultatoDopoDelay()
    {
        yield return new WaitForSeconds(2f); 

        puzzleManager?.CompleteMinigame(3);
        puzzlePanel.SetActive(true);
    }
    
    private IEnumerator MostraFade(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (immagineFade != null)
        {
            immagineFade.gameObject.SetActive(true);
            yield return StartCoroutine(FadeInImage(immagineFade, 1.5f));
        }
    }

    private IEnumerator FadeInImage(Image img, float durata)
    {
        float elapsed = 0f;
        Color c = img.color;
        c.a = 0f;
        img.color = c;

        while (elapsed < durata)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Clamp01(elapsed / durata);
            img.color = c;
            yield return null;
        }

        c.a = 1f;
        img.color = c;
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

    public MiniGameData GetSaveData()
    {
        return new MiniGameData
        {
            miniGameName = "MascherinaManager",
            punteggio = completato ? 1 : 0,
            popupAttivo = immagineFade != null && immagineFade.gameObject.activeSelf
        };
    }

    public void LoadFromData(MiniGameData data)
    {
        completato = data.punteggio > 0;

        if (completato)
        {
            textFeedback.text = "Bravo! Hai scelto la mascherina giusta.";
            puzzleManager?.CompleteMinigame(3);

            if (puzzlePanel != null)
                puzzlePanel.SetActive(true);

            if (immagineFade != null && data.popupAttivo)
            {
                immagineFade.gameObject.SetActive(true);
                Color c = immagineFade.color;
                c.a = 1f;
                immagineFade.color = c;
            }
        }
        else
        {
            // Reset stato iniziale (opzionale, se ricarichi una partita non completata)
            if (immagineFade != null)
            {
                Color c = immagineFade.color;
                c.a = 0f;
                immagineFade.color = c;
                immagineFade.gameObject.SetActive(false);
            }

            textFeedback.text = "";
            MostraMascherina(0);
        }
    }



}
