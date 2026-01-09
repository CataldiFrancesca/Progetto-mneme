using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class LensReceiver : MonoBehaviour, IDropHandler
{
    private GameObject currentLens = null;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;

        if (dropped != null && dropped.GetComponent<DraggableLens>() != null)
        {
            // Se c'è già una lente applicata, rimandala alla sua posizione originale
            if (currentLens != null)
            {
                ReturnToOriginalPosition(currentLens);
            }

            // Posiziona la nuova lente sopra l’occhiale
            dropped.transform.SetParent(transform);
            RectTransform rt = dropped.GetComponent<RectTransform>();
            rt.anchoredPosition = Vector2.zero;
            rt.localScale = new Vector3(1.2f, 1.2f, 1); // effetto zoom

            currentLens = dropped;

            // Ricava la parola in base al nome
            string word = dropped.name switch
            {
                "occhiali blu" => "cerca",
                "occhiali rosa" => " del sapere",
                "occhiali gialli" => "di bianco",
                "occhiali verdi" => "l’abito",
                "occhiali viola" => "si veste",
                "occhiali arancioni" => "dove",
                _ => ""
            };

           if (!string.IsNullOrEmpty(word))
{
    GameManager.Instance.AddWord(word);

    // Visualizza la parola sulla lente
    TextMeshProUGUI textComponent = dropped.GetComponentInChildren<TextMeshProUGUI>(true);
    if (textComponent != null)
    {
        textComponent.text = word;
        textComponent.gameObject.SetActive(true);
    }
}

        }
    }

    private void ReturnToOriginalPosition(GameObject lens)
    {
        DraggableLens draggable = lens.GetComponent<DraggableLens>();
        if (draggable != null)
        {
            lens.transform.SetParent(draggable.originalParent, false);

            RectTransform rt = lens.GetComponent<RectTransform>();
            rt.localScale = draggable.originalScale;
            rt.anchoredPosition = draggable.originalPosition;
        }
	var textComponent = lens.GetComponentInChildren<TextMeshProUGUI>(true);
		if (textComponent != null)
		{
    		textComponent.gameObject.SetActive(false);
		}

    }
}
