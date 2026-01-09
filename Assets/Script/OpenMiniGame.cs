using UnityEngine;

public class OpenMiniGame : MonoBehaviour
{
    public GameObject miniGameCanvas;
    public GameObject playerController; // deve essere "First Person Controller"

    private FirstPersonMovement movementScript;
    private FirstPersonLook lookScript;

    void OnMouseDown()
    {
        if (miniGameCanvas != null)
            miniGameCanvas.SetActive(true);

        if (playerController != null)
        {
            // Disattiva movimento
            movementScript = playerController.GetComponent<FirstPersonMovement>();
            if (movementScript != null)
                movementScript.enabled = false;

            // Disattiva visuale (la camera è figlia, la cerchiamo direttamente)
            lookScript = playerController.GetComponentInChildren<FirstPersonLook>();
            if (lookScript != null)
                lookScript.enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
