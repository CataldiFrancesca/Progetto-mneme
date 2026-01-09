using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public GameObject popupImage;              // Immagine popup dentro il Canvas principale
    public GameObject pannelloMinigioco;       // Pannello minigioco (rimane attivo)
    public GameObject playerController;        // Player controller da riattivare
    public GameObject[] uiCanvases;            // Canvases che bloccano l'apertura del popup

    void Update()
    {
        // Se il cursore è bloccato, non aprire il popup
      

        // Premi "I" per aprire o chiudere il popup solo se nessun altro canvas è attivo
        if (Input.GetKeyDown(KeyCode.I) && !IsAnyUICanvasActive())
        {
            TogglePopup();
        }
    }

    // Funzione per mostrare/nascondere il popup
    public void TogglePopup()
    {
        if (popupImage != null)
        {
            bool isActive = popupImage.activeSelf;
            popupImage.SetActive(!isActive);

            // Blocca/sblocca cursore a seconda dello stato del popup
            if (!isActive)
            {
                if (playerController != null)
                    playerController.SetActive(false);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                if (playerController != null)
                    playerController.SetActive(true);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    // Verifica se uno dei canvas è attivo
    private bool IsAnyUICanvasActive()
    {
        foreach (GameObject canvas in uiCanvases)
        {
            if (canvas != null && canvas.activeInHierarchy)
                return true;
        }
        return false;
    }

    // Funzione per "uscire" dal popup, ma NON chiudere il pannello minigioco
    public void EsciMinigioco()
    {
        Debug.Log("Chiudo solo l'immagine popup.");

        if (popupImage != null)
            popupImage.SetActive(false);

        if (playerController != null)
            playerController.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
