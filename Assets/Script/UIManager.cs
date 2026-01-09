using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject borsaCanvas;  // assegna il canvas della borsa qui

    // Nasconde la borsa
    public void NascondiBorsa()
    {
        if (borsaCanvas != null)
            borsaCanvas.SetActive(false);
    }

    // Mostra la borsa
    public void MostraBorsa()
    {
        if (borsaCanvas != null)
            borsaCanvas.SetActive(true);
    }
}
