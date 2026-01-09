using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections;

public class Giornale : MonoBehaviour, IMiniGameSaveable
{
    public TMP_InputField inputField;
    public TMP_Text feedbackText;

    public GameObject pannelloMinigioco;
    public GameObject playerController;
    public GameObject puzzlePanel;
    public PuzzleManager2 puzzleManager;

    private string fraseCorretta = "cerca dove il pensiero si fa inchiostro e la memoria si fissa";

    private bool completato = false;
    private int count = 0;
    private void Start()
    {
        // All'avvio, potremmo voler disattivare cursor/feedback se necessario
    }

    public void ControllaFrase()
    {
        string inputUtente = inputField.text.Trim().ToLower();

        if (inputUtente == fraseCorretta)
        {
            feedbackText.text = "Frase corretta! Cosa può significare?";
            inputField.text = fraseCorretta;
            completato = true;
            
        }
        else
        {
            feedbackText.text = "Frase errata, riprova.";
        }
    }
    private IEnumerator MostraRisultatoDopoDelay()
    {
        yield return new WaitForSeconds(2f); 

        puzzleManager?.CompleteMinigame(0);
        puzzlePanel.SetActive(true);
    }

    public void EsciMinigioco()
    {
        Debug.Log("Uscita dal minigioco.");

        pannelloMinigioco?.SetActive(false);
        playerController?.SetActive(true);

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
            miniGameName = "Giornale",
            fraseUtente = inputField.text,
            isActive = true,
            popupAttivo = pannelloMinigioco != null && pannelloMinigioco.activeSelf,
        };
    }

    public void LoadFromData(MiniGameData data)
    {
        if (!string.IsNullOrEmpty(data.fraseUtente))
        {
            inputField.text = data.fraseUtente.Trim().ToLower();

            if (inputField.text == fraseCorretta)
            {
                feedbackText.text = "Frase corretta! Cosa può significare?";
                

                puzzleManager?.CompleteMinigame(0);
                puzzlePanel?.SetActive(true);
            }
        }

        // Ripristina visibilità se necessario
        if (pannelloMinigioco != null)
            pannelloMinigioco.SetActive(data.popupAttivo);

        if (playerController != null)
            playerController.SetActive(!data.popupAttivo);
    }
}
