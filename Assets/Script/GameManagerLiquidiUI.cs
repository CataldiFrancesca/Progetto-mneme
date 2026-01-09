using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class GameManagerLiquidiUI : MonoBehaviour, IMiniGameSaveable
{
    public static GameManagerLiquidiUI Instance;

    public ProvettaUI[] provette;
    public Color[] coloriPossibili;
    public float step = 12.5f;

    public GameObject victoryPanel;

    public GameObject pannelloMinigioco;
    public GameObject playerController;

    public TMP_Text provettaSelezionataText;

    public GameObject puzzlePanel;
    public PuzzleManager puzzleManager;
    

    

    private ProvettaUI provettaSelezionata;

    private CanvasGroup victoryCanvasGroup;
    private bool completato = false;
    private int count = 0;



    void Awake()
    {
        Instance = this;

        if (victoryPanel != null)
        {
            victoryCanvasGroup = victoryPanel.GetComponent<CanvasGroup>();
            if (victoryCanvasGroup == null)
            {
                victoryCanvasGroup = victoryPanel.AddComponent<CanvasGroup>();
            }
            victoryCanvasGroup.alpha = 0f;
            victoryPanel.SetActive(false);
        }
    }

    void Start()
    {
            AvviaGioco();
    }

    public void ResetGioco()
    {
        AvviaGioco();
    }

    public void ContinuaDopoVittoria()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
            victoryCanvasGroup.alpha = 0f;
        }
    }

    public void SelezionaProvetta(ProvettaUI cliccata)
    {
        if (provettaSelezionata != null)
            provettaSelezionata.Evidenzia(false);

        if (provettaSelezionata == null)
        {
            if (cliccata.ContaStrati() > 0)
            {
                provettaSelezionata = cliccata;
                provettaSelezionata.Evidenzia(true);
                if (provettaSelezionataText != null)
                {
                    provettaSelezionataText.text = "Selezionata: " + cliccata.name;
                    provettaSelezionataText.fontMaterial.SetColor("_FaceColor", Color.green);
                }
            }
        }
        else if (provettaSelezionata != cliccata)
        {
            Versa(provettaSelezionata, cliccata);
            provettaSelezionata = null;
            if (provettaSelezionataText != null)
            {
                provettaSelezionataText.text = "Selezionata: -";
                provettaSelezionataText.fontMaterial.SetColor("_FaceColor", Color.green);
            }
        }
        else
        {
            // Deseleziona cliccando due volte
            provettaSelezionata = null;
            if (provettaSelezionataText != null)
            {
                provettaSelezionataText.text = "Selezionata: -";
                provettaSelezionataText.fontMaterial.SetColor("_FaceColor", Color.green);
            }
        }
    }

    void Versa(ProvettaUI da, ProvettaUI a)
    {
        var liquido = da.ControllaCima();
        if (liquido == null || a.ContaStrati() >= 8) return;

        var cimaDest = a.ControllaCima();
        if (cimaDest != null && cimaDest.colore != liquido.colore) return;

        while (da.ControllaCima() != null &&
               a.ContaStrati() < 8 &&
               (a.ControllaCima() == null || a.ControllaCima().colore == da.ControllaCima().colore))
        {
            a.VersaIn(da.VersaFuori());
        }

        ControllaVittoria();
    }

    public void AvviaGioco()
    {
        System.Random rnd = new System.Random();

        foreach (var p in provette)
            p.Resetta();

        int provettePiene = 4;
        int totProvette = provette.Length;

        for (int i = 0; i < provettePiene; i++)
        {
            List<Liquido> strati = new List<Liquido>();

            foreach (Color colore in coloriPossibili)
            {
                strati.Add(new Liquido(colore, step));
                strati.Add(new Liquido(colore, step));
            }

            strati = strati.OrderBy(x => rnd.Next()).ToList();

            foreach (var liquido in strati)
            {
                provette[i].ForzaVersaIn(liquido);
            }
        }

        for (int i = provettePiene; i < totProvette; i++)
        {
            provette[i].Resetta();
        }
    }

    void ControllaVittoria()
    {
        int complete = 0;

        foreach (var p in provette)
        {
            if (p.ContaStrati() == 8)
            {
                var liquidi = p.Liquidi.ToArray();
                if (liquidi.All(l => l.colore == liquidi[0].colore))
                    complete++;
            }
        }

        if (complete >= 4)
        {
            completato = true;
            if (victoryPanel != null)
                StartCoroutine(FadeInVictoryPanel(1.5f));
            
            
        }
    }

    private IEnumerator MostraRisultatoDopoDelay()
    {
        yield return new WaitForSeconds(2f); 

        puzzleManager?.CompleteMinigame(3);
        puzzlePanel.SetActive(true);
    }

    IEnumerator FadeInVictoryPanel(float duration)
    {
        victoryPanel.SetActive(true);
        victoryCanvasGroup.alpha = 0f;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            victoryCanvasGroup.alpha = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        victoryCanvasGroup.alpha = 1f;
    }

    public void EsciMinigioco()
    {
        Debug.Log("Uscita dal minigioco.");

        pannelloMinigioco.SetActive(false);

        if (playerController != null)
            playerController.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (completato && count == 0)
        {
            count++;
            StartCoroutine(MostraRisultatoDopoDelay());
        }
    }

    public MiniGameData GetSaveData()
    {
        MinigiocoLiquidiData data = new MinigiocoLiquidiData();

        foreach (var p in provette)
        {
            ProvettaData pd = new ProvettaData();
            var liquidiArray = p.Liquidi.ToArray();

            Array.Reverse(liquidiArray);

            foreach (var l in liquidiArray)
            {
                pd.colori.Add(ColorUtility.ToHtmlStringRGBA(l.colore));
                pd.volumi.Add(l.volume);
            }

            data.provette.Add(pd);
        }

        string jsonData = JsonUtility.ToJson(data);
        return new MiniGameData()
        {
            miniGameName = "MinigiocoLiquidi",
            jsonData = jsonData,
            victoryPanelActive = victoryPanel != null && victoryPanel.activeSelf,
            punteggio = (victoryPanel != null && victoryPanel.activeSelf) ? 1 : 0
        };
    }


    public void LoadFromData(MiniGameData data)
    {
        MinigiocoLiquidiData loadedData = JsonUtility.FromJson<MinigiocoLiquidiData>(data.jsonData);

        for (int i = 0; i < provette.Length; i++)
        {
            provette[i].Resetta();

            if (i >= loadedData.provette.Count)
                continue;

            var pd = loadedData.provette[i];
            for (int j = 0; j < pd.colori.Count; j++)
            {
                Color colore;
                if (ColorUtility.TryParseHtmlString("#" + pd.colori[j], out colore))
                {
                    Liquido l = new Liquido(colore, pd.volumi[j]);
                    provette[i].ForzaVersaIn(l);
                }
                else
                {
                    Debug.LogWarning("Colore non valido nel salvataggio: " + pd.colori[j]);
                }
            }
        }

        if ((data.punteggio > 0 || data.victoryPanelActive) && victoryPanel != null)
        {
            StartCoroutine(RipristinaStatoVittoria());
        }
    }
    private IEnumerator RipristinaStatoVittoria()
    {
        yield return null; // aspetta un frame per sicurezza

        victoryPanel.SetActive(true);
        if (victoryCanvasGroup != null)
            victoryCanvasGroup.alpha = 1f;

        // Ripristina il puzzle
        puzzleManager?.CompleteMinigame(3);
        if (puzzlePanel != null)
            puzzlePanel.SetActive(true);
    }





}
