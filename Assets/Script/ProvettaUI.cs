using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ProvettaUI : MonoBehaviour
{
    public float maxVolume = 100f;
    public float step = 12.5f; // 100 / 8 strati
    public Transform contenitoreStrati;
    public Image prefabStrato;
    public Image bordoSelezione; // Nuovo per evidenziazione

    public IEnumerable<Liquido> Liquidi => liquidi;

    private Stack<Liquido> liquidi = new Stack<Liquido>();

    public void Resetta()
    {
        liquidi.Clear();
        foreach (Transform c in contenitoreStrati)
            Destroy(c.gameObject);

        AggiornaVisuale();
        Evidenzia(false);
    }

    public void VersaIn(Liquido l)
    {
        if (liquidi.Count == 0 || liquidi.Peek().colore == l.colore)
        {
            liquidi.Push(new Liquido(l.colore, l.volume));
            
            AggiornaVisuale();
        }
        else
        {
            Debug.Log($"[{gameObject.name}] VersaIn: colore diverso, non aggiunto. Ultimo={liquidi.Peek().colore}, Nuovo={l.colore}");
        }
    }

    public Liquido VersaFuori()
    {
        if (liquidi.Count == 0) return null;

        Liquido top = liquidi.Pop();
        AggiornaVisuale();
        return top;
    }

    public Liquido ControllaCima() => liquidi.Count > 0 ? liquidi.Peek() : null;

    public int ContaStrati() => liquidi.Count;

    public void AggiornaVisuale()
{
    foreach (Transform c in contenitoreStrati)
        Destroy(c.gameObject);

    var listaLiquidi = liquidi.ToArray(); // Manteniamo l’ordine dello stack

   

    foreach (var liquido in listaLiquidi)
    {
        Image nuovo = Instantiate(prefabStrato, contenitoreStrati);
        Color colore = liquido.colore;
        colore.a = 1f;
        nuovo.color = colore;
        
    }
}


    public void Seleziona()
    {
        GameManagerLiquidiUI.Instance.SelezionaProvetta(this);
    }

    public void Evidenzia(bool attivo)
    {
        if (bordoSelezione != null)
            bordoSelezione.enabled = attivo;
    }

    // Forza l'inserimento di un liquido nella provetta, ignorando i controlli
    public void ForzaVersaIn(Liquido l)
    {
        liquidi.Push(new Liquido(l.colore, l.volume));
        AggiornaVisuale();
    }

}
