using UnityEngine;

public class MinigameTrigger : MonoBehaviour
{
    public GameObject minigameUI;         // Il pannello del minigioco
    public GameObject playerController;   // Oggetto con script di movimento

    void OnMouseDown()
    {
        if (minigameUI.activeSelf) return;

        // Attiva il pannello del minigioco
        minigameUI.SetActive(true);

        // Disattiva il controller del giocatore (blocco movimento)
        if (playerController != null)
            playerController.SetActive(false);

        // Mostra il cursore per l’interfaccia
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}

