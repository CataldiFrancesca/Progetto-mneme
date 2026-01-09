using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Collections.Generic;

public class PhoneUnlocker : MonoBehaviour, IMiniGameSaveable
{
    public Button clearButton;
    public string correctCode = "48251";

    public GameObject telefonoBloccato;
    public GameObject telefonoSbloccato;
    public GameObject puzzlePanel;
    public PuzzleManager2 puzzleManager;

    public TextMeshProUGUI feedbackText;
    public GameObject pannelloMinigioco;
    public GameObject playerController;

    private string enteredCode = "";
    private bool completato = false;
    private int count = 0;

    public Button[] numberButtons;
    public string[] buttonNumbers;
    public TextMeshProUGUI statusText; // Mostra "Codice corretto" o "Codice errato"


    void Start()
    {
        if (numberButtons.Length != buttonNumbers.Length)
        {
            Debug.LogError("numberButtons e buttonNumbers devono avere la stessa lunghezza!");
            return;
        }

        for (int i = 0; i < numberButtons.Length; i++)
        {
            string num = buttonNumbers[i];
            numberButtons[i].onClick.RemoveAllListeners();
            numberButtons[i].onClick.AddListener(() => PressNumber(num));
        }

        if (clearButton != null)
        {
            clearButton.onClick.RemoveAllListeners();
            clearButton.onClick.AddListener(ClearCode);
            clearButton.gameObject.SetActive(true);
        }

        ResetPhoneUI();
    }

    public void PressNumber(string digit)
{
    if (completato) return;  // Blocca input se già completato
    if (enteredCode.Length >= 5) return;

    enteredCode += digit;

    if (feedbackText != null)
    {
        // Aggiunge uno spazio tra ogni cifra
        feedbackText.text = string.Join(" ", enteredCode.ToCharArray());
    }

    if (enteredCode.Length == 5)
    {
        StartCoroutine(VerifyCode());
    }
}


    void ClearCode()
    {
        if (completato) return;
        enteredCode = "";

        if (feedbackText != null)
            feedbackText.text = "";

        if (statusText != null)
            statusText.text = ""; // Ripulisce il messaggio
    }



    IEnumerator VerifyCode()
    {
        yield return new WaitForSeconds(0.2f);

        if (enteredCode == correctCode)
        {
            completato = true;

            if (statusText != null)
                statusText.text = "<color=green>Codice corretto!</color>";

            if (telefonoBloccato != null) telefonoBloccato.SetActive(false);
            if (telefonoSbloccato != null) telefonoSbloccato.SetActive(true);

           
        }
        else
        {
            if (statusText != null)
                statusText.text = "<color=red>Codice errato</color>";

            yield return new WaitForSeconds(1f);
            ClearCode();
        }
    }



    private IEnumerator MostraRisultatoDopoDelay()
    {
        yield return new WaitForSeconds(2f); 

        puzzleManager?.CompleteMinigame(4);
        puzzlePanel.SetActive(true);
    }

    

    void ResetPhoneUI()
    {
        if (completato)
        {
            telefonoBloccato?.SetActive(false);
            telefonoSbloccato?.SetActive(true);
            puzzlePanel.SetActive(true);
            puzzleManager?.CompleteMinigame(4);
           feedbackText.text = string.Join(" ", correctCode.ToCharArray());

        }
        else
        {
            telefonoBloccato?.SetActive(true);
            telefonoSbloccato?.SetActive(false);
            puzzlePanel?.SetActive(false);
            ClearCode();
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


    // ✅ Salvataggio
    public MiniGameData GetSaveData()
    {
        return new MiniGameData
        {
            miniGameName = "PhoneUnlocker",
            punteggio = completato ? 1 : 0,
            fraseUtente = enteredCode
        };
    }

    // ✅ Caricamento
    public void LoadFromData(MiniGameData data)
    {
        enteredCode = data.fraseUtente ?? "";
        completato = data.punteggio > 0;
        
        ResetPhoneUI();
        
    }
}
