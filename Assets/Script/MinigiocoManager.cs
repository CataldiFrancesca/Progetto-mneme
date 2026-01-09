using UnityEngine;

public class MinigiocoManager : MonoBehaviour
{
    public GameObject minigiocoCanvas;
    public GameObject player;

    private FirstPersonMovement playerMovement;
    private FirstPersonLook playerLook;

    void Start()
    {
        playerMovement = player.GetComponent<FirstPersonMovement>();
        playerLook = player.GetComponentInChildren<FirstPersonLook>();

        minigiocoCanvas.SetActive(false);
        EnablePlayer(true);
        LockCursor(true);
    }

    public void StartMinigioco()
    {
        minigiocoCanvas.SetActive(true);
        EnablePlayer(false);
        LockCursor(false);
    }

    public void ExitMinigioco()
    {
        minigiocoCanvas.SetActive(false);
        EnablePlayer(true);
        LockCursor(true);
    }

    private void EnablePlayer(bool enable)
    {
        playerMovement.enabled = enable;
        playerLook.enabled = enable;
    }

    private void LockCursor(bool locked)
    {
        if (locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
