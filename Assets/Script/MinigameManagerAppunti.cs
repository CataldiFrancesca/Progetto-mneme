using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;


public class MinigameManagerAppunti : MonoBehaviour, IMiniGameSaveable
{
    public DropSlot[] slots;             // Associa slot0, slot1, ..., slot5
    public GameObject popupSuccesso;
    public GameObject popupErrore;

    public Button verificaButton;
    public Button esciButton;

    public GameObject playerController;  // Riferimento al controller del giocatore
    public GameObject minigameUI;         // Riferimento al pannello UI del minigioco
    public GameObject puzzlePanel;
    public PuzzleManager2 puzzleManager;

    private int[] correctSequence = { 1, 3, 4, 2, 0, 5 }; // Ordine corretto per Image0 ... Image5

    private bool completato = false;
    private int count = 0;

    void Start()
    {
        verificaButton.onClick.AddListener(CheckSequence);
        esciButton.onClick.AddListener(OnEsciClicked);
        popupSuccesso.SetActive(false);
        popupErrore.SetActive(false);
    }

    void CheckSequence()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            int imageIndex = slots[i].GetImageIndex();

            if (imageIndex != correctSequence[i])
            {
                StartCoroutine(ShowError());
                return;
            }
        }

        ShowSuccess();
        

    }

    private IEnumerator MostraRisultatoDopoDelay()
    {
        yield return new WaitForSeconds(2f); 

        puzzleManager?.CompleteMinigame(1);
        puzzlePanel.SetActive(true);
    }

    void ShowSuccess()
    {
        DisableDragging();
        completato = true;
        popupSuccesso.SetActive(true);
    }

    System.Collections.IEnumerator ShowError()
    {
        popupErrore.SetActive(true);



        yield return new WaitForSeconds(1.5f);

        popupErrore.SetActive(false);

        foreach (var slot in slots)
        {
            slot.Clear();
        }
    }

    void DisableDragging()
    {
        foreach (var slot in slots)
        {
            if (slot.currentImage != null)
            {
                DraggableImage draggable = slot.currentImage.GetComponent<DraggableImage>();
                if (draggable != null)
                {
                    draggable.enabled = false;
                }
            }
        }
    }

    void OnEsciClicked()
    {
        // Nascondi la UI del minigioco
        if (minigameUI != null)
            minigameUI.SetActive(false);

        // Riattiva il controller del giocatore
        if (playerController != null)
            playerController.SetActive(true);

        // Nascondi e blocca il cursore
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
        MinigiocoAppuntiData data = new MinigiocoAppuntiData();

        string jsonData = JsonUtility.ToJson(data);

        return new MiniGameData()
        {
            miniGameName = "MinigiocoAppunti",
            jsonData = jsonData,
            popupAttivo = popupSuccesso != null && popupSuccesso.activeSelf,
            punteggio = (popupSuccesso != null && popupSuccesso.activeSelf) ? 1 : 0
        };
    }

    public void LoadFromData(MiniGameData data)
    {
        if (string.IsNullOrEmpty(data.jsonData))
            return;

        MinigiocoAppuntiData loadedData = JsonUtility.FromJson<MinigiocoAppuntiData>(data.jsonData);

        if (data.punteggio > 0 || data.popupAttivo)
        {
            popupSuccesso.SetActive(true);
            DisableDragging();
            puzzleManager?.CompleteMinigame(1);
            puzzlePanel.SetActive(true);
        }
    }

}
