using UnityEngine;
using UnityEngine.EventSystems;

public class DropSlot : MonoBehaviour, IDropHandler
{
    public DraggableImage currentImage;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            DraggableImage dragged = eventData.pointerDrag.GetComponent<DraggableImage>();

            // Libera il vecchio slot dell'immagine
            if (dragged.currentSlot != null)
            {
                dragged.currentSlot.currentImage = null;
            }

            // Se questo slot ha già un'immagine, rimettila al suo posto
            if (currentImage != null && currentImage != dragged)
            {
                currentImage.ReturnToOriginalPosition();
            }

            // Assegna la nuova immagine a questo slot
            dragged.transform.SetParent(transform);
            dragged.transform.localPosition = Vector3.zero;

            currentImage = dragged;
            dragged.currentSlot = this;
        }
    }

    public int GetImageIndex()
    {
        if (currentImage != null)
        {
            string name = currentImage.name.Replace("Image", "");
            if (int.TryParse(name, out int index))
                return index;
        }
        return -1;
    }

    public void Clear()
    {
        if (currentImage != null)
        {
            currentImage.ReturnToOriginalPosition();
            currentImage = null;
        }
    }

    
}
