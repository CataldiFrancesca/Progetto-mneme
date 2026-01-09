using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableKey : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    public Canvas canvas; // Assegna nel Inspector
    private Vector2 initialPosition;

    public bool canDrag = true;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        initialPosition = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!canDrag) return;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!canDrag) return;
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!canDrag) return;
        canvasGroup.blocksRaycasts = true;

        if (!MinigiocoPorta.Instance.keyDroppedCorrectly)
        {
            rectTransform.anchoredPosition = initialPosition;
        }
    }
}
