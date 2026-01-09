using UnityEngine;
using System.Collections.Generic;

public class MapToggle : MonoBehaviour
{
    [Header("UI e Giocatore")]
    public GameObject mapCanvas;           // Il canvas da mostrare
    public GameObject playerController;    // Oggetto con script movimento/camera

    [Header("Canvas da ignorare per il toggle")]
    public List<GameObject> ignoredCanvases = new List<GameObject>();

    private bool mapOpen = false;

    void Start()
    {
        if (mapCanvas != null)
            mapCanvas.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) && CanToggleMap())
        {
            ToggleMap();
        }
    }

    bool CanToggleMap()
    {
        foreach (var canvas in ignoredCanvases)
        {
            if (canvas != null && canvas.activeInHierarchy)
                return false;
        }
        return true;
    }

    void ToggleMap()
    {
        mapOpen = !mapOpen;

        if (mapCanvas != null)
            mapCanvas.SetActive(mapOpen);

        if (playerController != null)
            playerController.SetActive(!mapOpen);

        // Blocca/sblocca il cursore
        Cursor.visible = mapOpen;
        Cursor.lockState = mapOpen ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
