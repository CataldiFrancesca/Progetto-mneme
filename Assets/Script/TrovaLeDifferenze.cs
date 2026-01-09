using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TrovaLeDifferenze : MonoBehaviour, IMiniGameSaveable
{
    public GameObject panelGioco;
    public TextMeshProUGUI counterText;
    public TextMeshProUGUI feedbackText;
    public Button esciButton;

    public GameObject playerLookScript; 
    public GameObject indizioPanel;
    public GameObject puzzlePanel;
    public PuzzleManager puzzleManager;
    public GameObject[] oggettiDifferenza; // Inseriscili da Inspector


    private int differenzeTrovate = 0;
    private int differenzeTotali = 6;
    private bool[] differenzeIndovinate;
    private bool completato = false;
    private int count = 0;

    void Start()
    {
        differenzeIndovinate = new bool[differenzeTotali];
        esciButton.onClick.AddListener(ChiudiGioco);
        AggiornaTesto();
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

    public void ClickDifferenza(int indice)
    {
        if (!differenzeIndovinate[indice])
        {
            differenzeIndovinate[indice] = true;
            differenzeTrovate++;
            feedbackText.text = "Bravo, hai trovato una differenza!";
            AggiornaTesto();

            if (differenzeTrovate == differenzeTotali)
            {
                feedbackText.text = "Bravo, hai trovato tutte le differenze!";
                Invoke("MostraIndizioPanel", 1.5f);
                completato = true;
                
                
            }
        }
        else
        {
            feedbackText.text = "Hai già cliccato questa differenza.";
        }
    }
private IEnumerator MostraRisultatoDopoDelay()
    {
        yield return new WaitForSeconds(2f); 

        puzzleManager?.CompleteMinigame(1);
        puzzlePanel.SetActive(true);
    }
    

    private void MostraIndizioPanel()
    {
        indizioPanel.SetActive(true);
    }

    private void AggiornaTesto()
    {
        counterText.text = differenzeTrovate + "/" + differenzeTotali;
    }

    // ===== SALVATAGGIO =====

    public MiniGameData GetSaveData()
{
    return new MiniGameData
    {
        miniGameName = "TrovaLeDifferenze",
        isActive = panelGioco.activeSelf,
        popupAttivo = indizioPanel.activeSelf,
        differenzeTrovate = differenzeTrovate,
        differenzeIndovinate = new List<bool>(differenzeIndovinate)
    };
}




    public void LoadFromData(MiniGameData data)
{
    StartCoroutine(LoadRoutine(data));
}

    private IEnumerator LoadRoutine(MiniGameData data)
    {
        // Imposta stato interno
        differenzeIndovinate = new bool[differenzeTotali];

        if (data.differenzeIndovinate != null && data.differenzeIndovinate.Count == differenzeTotali)
        {
            differenzeIndovinate = data.differenzeIndovinate.ToArray();
            differenzeTrovate = data.differenzeTrovate;
        }
        else
        {
            differenzeTrovate = 0;
            differenzeIndovinate = new bool[differenzeTotali];
        }

        AggiornaTesto();

        // Attiva pannello almeno per un frame (UI pronta)
        panelGioco.SetActive(true);

        yield return null;  // aspetta 1 frame

        // Ora aggiorna UI in base allo stato salvato
        if (!data.isActive)
            panelGioco.SetActive(false);

        if (data.isActive)
            ApriGioco();
        else
            ChiudiGioco();

        indizioPanel.SetActive(data.popupAttivo);

        if (data.popupAttivo)
        {
            puzzleManager?.CompleteMinigame(1);
            puzzlePanel.SetActive(true);
        }

        feedbackText.text = "";

        for (int i = 0; i < differenzeTotali; i++)
        {
            if (differenzeIndovinate[i] && oggettiDifferenza[i] != null)
            {
                oggettiDifferenza[i].SetActive(false);
            }
        }
    }



}
