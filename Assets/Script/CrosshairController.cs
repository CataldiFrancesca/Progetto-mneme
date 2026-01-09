using UnityEngine;

public class CrosshairManager : MonoBehaviour
{
    public GameObject crosshairUI;
    public GameObject[] uiCanvases;
    public bool isPausedExternally = false;

    private bool crosshairToggleEnabled = true;

    void Start()
    {
        ShowCrosshair(true);
    }

    void Update()
    {
        ApplyCrosshairLogic();

        // Accetta input C solo se nessun canvas è attivo e non è in pausa
        if (Input.GetKeyDown(KeyCode.C) && !isPausedExternally && !IsAnyUICanvasActive())
        {
            crosshairToggleEnabled = !crosshairToggleEnabled;
        }
    }

    public void ApplyCrosshairLogic()
    {
        bool isAnyCanvasActive = IsAnyUICanvasActive();

        if (isPausedExternally || isAnyCanvasActive)
        {
            // Pausa o UI attivi: mostra cursore, nascondi mirino
            ShowCrosshair(false);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            // Nessuna UI attiva: mostra mirino se abilitato, nascondi cursore
            ShowCrosshair(crosshairToggleEnabled);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    bool IsAnyUICanvasActive()
    {
        foreach (GameObject canvas in uiCanvases)
        {
            if (canvas != null && canvas.activeInHierarchy)
                return true;
        }
        return false;
    }

    void ShowCrosshair(bool state)
    {
        if (crosshairUI != null)
            crosshairUI.SetActive(state);
    }

    public void ForceCrosshairState(bool visible)
    {
        crosshairToggleEnabled = visible;
        ApplyCrosshairLogic();
    }
}
