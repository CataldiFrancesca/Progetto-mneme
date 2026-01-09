using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableImage : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform originalParent;
    private Vector2 originalPosition;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    [HideInInspector]
    public DropSlot currentSlot;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        originalParent = transform.parent;
        originalPosition = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        transform.SetParent(transform.root); // bring to top level UI
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / transform.root.GetComponent<Canvas>().scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        if (transform.parent == transform.root)
        {
            ReturnToOriginalPosition();
        }
    }

    public void ReturnToOriginalPosition()
    {
        if (currentSlot != null)
        {
            currentSlot.currentImage = null;
            currentSlot = null;
        }

        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = originalPosition;
    }

    public void EnableDragging()
{
    enabled = true;
}

}
