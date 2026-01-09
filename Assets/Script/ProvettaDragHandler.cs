using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ProvettaDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool isCorrect = false;          // Impostato da Inspector
    public GameObject checkMark;  
    public string provettaID; 

    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 initialPosition;

    public bool isDraggable = true;        // ✨ Nuovo: flag per bloccare il drag

    private Vector2 initialAnchoredPosition;

private void Awake()
{
    rectTransform = GetComponent<RectTransform>();
    canvasGroup = GetComponent<CanvasGroup>();
    canvas = GetComponentInParent<Canvas>();
    initialAnchoredPosition = rectTransform.anchoredPosition; // usa anchoredPosition, non transform.position
}

public void ReturnToStart()
{
    rectTransform.anchoredPosition = initialAnchoredPosition; // ripristina anchoredPosition
}

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;

        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;

        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        if (GameManagerGuanto.Instance.IsOverTarget(eventData))
        {
            if (isCorrect)
            {
                GameManagerGuanto.Instance.HandleCorrectProvetta(this);
            }
            else
            {
                GameManagerGuanto.Instance.HandleWrongProvetta(this);
            }
        }
        else
        {
            ReturnToStart();
        }
    }

    

    public void ShowCheck()
{
    if (checkMark != null)
        checkMark.SetActive(true);

    isDraggable = false;
}
}
