using UnityEngine;
using UnityEngine.UI;

public class GlassesManager : MonoBehaviour
{
    [Header("Riferimenti agli oggetti UI")]
    public GameObject filterCanvas;      // GlassesFilterCanvas
    public GameObject hiddenNumbers;     // HiddenNumbers
    public Button esciButton;            // ExitButton

    private bool glassesOn = false;

    void Start()
    {
        esciButton.onClick.AddListener(ChiudiFiltro);

        // Avvio: occhiali non attivi
        filterCanvas.SetActive(false);
        hiddenNumbers.SetActive(false);
    }

    void Update()
    {
        // Se premi il tasto T, chiudi filtro (togli occhiali)
        if (Input.GetKeyDown(KeyCode.T))
        {
            ChiudiFiltro();
        }
    }

    public void ToggleGlasses()
    {
        glassesOn = !glassesOn;

        filterCanvas.SetActive(glassesOn);
        hiddenNumbers.SetActive(glassesOn);
    }

    public void ChiudiFiltro()
    {
        glassesOn = false;

        filterCanvas.SetActive(false);
        hiddenNumbers.SetActive(false);
    }
}
