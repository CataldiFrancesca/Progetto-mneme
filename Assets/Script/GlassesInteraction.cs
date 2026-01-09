using UnityEngine;

public class GlassesInteraction : MonoBehaviour
{
    public GlassesManager manager;

    void OnMouseDown()
    {
        manager.ToggleGlasses();
    }
}
