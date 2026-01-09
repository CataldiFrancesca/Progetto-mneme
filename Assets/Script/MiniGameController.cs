using UnityEngine;

public class MiniGameController : MonoBehaviour
{
    public GameObject miniGameCanvas;
    public GameObject playerController;       // FirstPersonController (padre)
    public GameObject firstPersonCamera;      // FirstPersonCamera (figlio)

    private FirstPersonMovement movementScript;
    private FirstPersonLook lookScript;

    private bool minigiocoAttivo = false;

    private void Start()
    {
        if (playerController != null)
        {
            movementScript = playerController.GetComponent<FirstPersonMovement>();
        }

        if (firstPersonCamera != null)
        {
            lookScript = firstPersonCamera.GetComponent<FirstPersonLook>();
        }

        if (miniGameCanvas != null)
            miniGameCanvas.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnMouseDown()
    {
        ApriMinigioco();
    }

    public void ApriMinigioco()
    {
        if (miniGameCanvas != null)
            miniGameCanvas.SetActive(true);

        if (movementScript != null)
            movementScript.enabled = false;

        if (lookScript != null)
            lookScript.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        minigiocoAttivo = true;
    }

    public void ChiudiMinigioco()
    {
        if (miniGameCanvas != null)
            miniGameCanvas.SetActive(false);

        if (movementScript != null)
            movementScript.enabled = true;

        if (lookScript != null)
        {
            // Riabilita lo script
            lookScript.enabled = true;

            // Se il tuo FirstPersonLook ha una funzione di reset, chiamala qui, ad esempio:
            // lookScript.ResetRotation();

            // Altrimenti, per sicurezza, puoi disabilitare e riabilitare per forzare il reset interno:
            // lookScript.enabled = false;
            // lookScript.enabled = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        minigiocoAttivo = false;
    }

    private void Update()
    {
        if (!minigiocoAttivo)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
