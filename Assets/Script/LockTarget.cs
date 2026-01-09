using UnityEngine;
using UnityEngine.EventSystems;

public class LockTarget : MonoBehaviour, IDropHandler
{
    public bool isCorrectLock;

    public void OnDrop(PointerEventData eventData)
    {
        DraggableKey key = eventData.pointerDrag.GetComponent<DraggableKey>();
        if (key != null)
        {
            if (isCorrectLock)
            {
                key.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
                key.canDrag = false;

                MinigiocoPorta.Instance.CompletaMinigioco();
            }
            else
            {
                MinigiocoPorta.Instance.Fallimento();
            }
        }
    }
}
