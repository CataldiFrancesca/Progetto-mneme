using UnityEngine;

public class MinigiocoLauncher : MonoBehaviour
{
    public GameObject minigiocoUI;
    public GameObject playerCamera;
    public MonoBehaviour cameraControlScript; // es: MouseLook, o altro
    public MonoBehaviour playerControlScript; // es: FirstPersonController (opzionale)
    public GameObject hangmanGameObject;


    private bool minigiocoAttivo = false;

    void OnMouseDown()
    {
        if (!minigiocoAttivo)
        {
            AvviaMinigioco();
        }
    }

    void AvviaMinigioco()
    {
        if (minigiocoUI != null)
        {
            minigiocoUI.SetActive(true);
            minigiocoAttivo = true;

            if (cameraControlScript != null)
                cameraControlScript.enabled = false;

            if (playerControlScript != null)
                playerControlScript.enabled = false;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

   public void ExitGame()
{
    if (hangmanGameObject != null)
    {
        var hangmanScript = hangmanGameObject.GetComponent<HangmanGame>();
        if (hangmanScript != null)
        {
            hangmanScript.ChiudiMinigioco();
        }
    }
}

}


