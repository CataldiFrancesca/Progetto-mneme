using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableLens : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform originalParent;
    public Vector2 originalPosition;
    public Vector3 originalScale;

    private CanvasGroup canvasGroup;
    private bool isInitialized = false;
    private RectTransform rectTransform;

    // Nuove variabili per la trasparenza e l'ingrandimento
    public float transparencyAfterDrag = 0.8f;  // Trasparenza meno trasparente dopo il drag
    public float scaleOnDrag = 1.2f;            // Fattore di scala durante il drag
    private float originalTransparency;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        InitializeOriginalTransform();

        // Salva la trasparenza originale
        if (canvasGroup != null)
        {
            originalTransparency = canvasGroup.alpha;
        }
    }

    private void InitializeOriginalTransform()
    {
        if (!isInitialized)
        {
            originalParent = transform.parent;
            originalPosition = rectTransform.anchoredPosition;
            originalScale = rectTransform.localScale;
            isInitialized = true;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        transform.SetParent(originalParent.parent); // alza il livello per evitare mascheramenti

        // Modifica la scala al momento del drag
        rectTransform.localScale = originalScale * scaleOnDrag;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        // Modifica la trasparenza dopo il drag
        if (canvasGroup != null)
        {
            canvasGroup.alpha = transparencyAfterDrag;
        }

        // Ripristina la scala al termine del drag
        rectTransform.localScale = originalScale;
    }
}
