using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    private bool isPaused = false;

    public Transform player;

    [Header("Canvas / Popup")]
    public GameObject opzioniCanvas;          // Pannello opzioni
    public GameObject confermaUscitaCanvas;

    public GameObject Menu;

    [Header("Minigiochi da salvare")]
    public MonoBehaviour[] miniGameObjects;
    public CrosshairManager crosshairManager;
    private IMiniGameSaveable[] miniGamesSaveables;

    private string folderPath;

    void Start()
    {
        folderPath = Path.Combine(Application.persistentDataPath, "Salvataggi");

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        // Inizializza array di interfacce da MonoBehaviour
        miniGamesSaveables = new IMiniGameSaveable[miniGameObjects.Length];
        for (int i = 0; i < miniGameObjects.Length; i++)
        {
            miniGamesSaveables[i] = miniGameObjects[i] as IMiniGameSaveable;

            if (miniGamesSaveables[i] == null)
            {
                Debug.LogWarning($"L'oggetto '{miniGameObjects[i].name}' non implementa IMiniGameSaveable.");
            }
        }
        folderPath = Path.Combine(Application.persistentDataPath, "Salvataggi");
        // ...

        // Caricamento automatico se si proviene da una partita salvata
        if (GameLoader.Instance != null && GameLoader.Instance.datiCaricati != null)
        {
            LoadFromData(GameLoader.Instance.datiCaricati);
            GameLoader.Instance.datiCaricati = null; // pulizia
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }



    public void ResumeGame()
    {
        isPaused = false;
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;

        // Forza aggiornamento cursore e crosshair
        if (crosshairManager != null)
            crosshairManager.ApplyCrosshairLogic();
    }


    public void SaveGame()
    {
        SaveData dati = new SaveData();
        dati.scenaCorrente = SceneManager.GetActiveScene().name;

        if (player != null)
        {
            dati.playerX = player.position.x;
            dati.playerY = player.position.y;
            dati.playerZ = player.position.z;
        }

        dati.miniGames.Clear();

        foreach (IMiniGameSaveable mini in miniGamesSaveables)
        {
            if (mini != null)

                dati.miniGames.Add(mini.GetSaveData());

        }

        string json = JsonUtility.ToJson(dati, true);
        string savePath = Path.Combine(folderPath, "salvataggio.json");

        File.WriteAllText(savePath, json);
        Debug.Log("Gioco salvato in: " + savePath);
        Debug.Log("Contenuto JSON salvato:\n" + json);

    }

    public void SaveGameQuit()
    {
        SaveData dati = new SaveData();
        dati.scenaCorrente = SceneManager.GetActiveScene().name;

        if (player != null)
        {
            dati.playerX = player.position.x;
            dati.playerY = player.position.y;
            dati.playerZ = player.position.z;
        }

        dati.miniGames.Clear();

        foreach (IMiniGameSaveable mini in miniGamesSaveables)
        {
            if (mini != null)

                dati.miniGames.Add(mini.GetSaveData());

        }

        string json = JsonUtility.ToJson(dati, true);
        string savePath = Path.Combine(folderPath, "salvataggio.json");

        File.WriteAllText(savePath, json);
        Debug.Log("Gioco salvato in: " + savePath);
        Application.Quit();
    }

    void LoadFromData(SaveData dati)
    {
        if (player != null)
        {
            player.position = new Vector3(dati.playerX, dati.playerY, dati.playerZ);
        }

        foreach (MiniGameData miniGameData in dati.miniGames)
        {
            foreach (IMiniGameSaveable mg in miniGamesSaveables)
            {
                if (mg != null && miniGameData.miniGameName == GetMiniGameName(mg))
                {
                    MonoBehaviour mb = mg as MonoBehaviour;
                    if (mb != null && !mb.gameObject.activeInHierarchy)
                        mb.gameObject.SetActive(true);

                    mg.LoadFromData(miniGameData);
                    break;
                }
            }
        }

        Debug.Log("Gioco caricato da GameLoader.");


    }


    string GetMiniGameName(IMiniGameSaveable mg)
    {
        // Evita la chiamata a GetSaveData() che crea un oggetto nuovo.
        if (mg is GameManagerLiquidiUI)
            return "MinigiocoLiquidi";

        if (mg is MinigiocoFrigoManager)
            return "MinigiocoFrigo";

        if (mg is PhraseChecker)
            return "FraseMinigioco";

        if (mg is GameManagerGuanto)
            return "GuantoMinigioco";

        if (mg is MinigameManager)
            return "FrasePuzzleMinigioco";

        if (mg is TrovaLeDifferenze)
            return "TrovaLeDifferenze";

        if (mg is GameManager)
            return "OcchialiPuzzleMinigioco";

        if (mg is CamiceGame)
            return "CamiceMinigioco";

        if (mg is HangmanGame)
            return "HangmanMinigioco";

        if (mg is CodeChecker)
            return "CodeCheckerMinigioco";

        if (mg is MinigameManagerAppunti)
            return "MinigiocoAppunti";

        if (mg is MinigiocoPorta)
            return "MinigiocoPorta";

        if (mg is Giornale)
            return "Giornale";

        if (mg is TestTubeMixer)
            return "Minigiocoprovette";

        if (mg is PhoneUnlocker)
            return "PhoneUnlocker"; // Stesso nome usato in GetSaveData()

        if (mg is LockManager)
            return "LockManager";

        if (mg is MascherinaManager)
            return "MascherinaManager";

        if (mg is MicroscopioGame)
            return "MicroscopioGame";

        // Aggiungi altri minigiochi se necessario:
        // if (mg is GameManagerAltro)
        //     return "MinigiocoAltro";

        return mg.GetType().Name;  // fallback
    }


    public void MostraOpzioni(GameObject panel)
    {
        if (opzioniCanvas != null)
            opzioniCanvas.SetActive(true);
        if (panel != null)
            panel.SetActive(false);
    }

    public void MostraConfermaUscita(GameObject panel)
    {
        if (confermaUscitaCanvas != null)
            confermaUscitaCanvas.SetActive(true);
        if (panel != null)
            panel.SetActive(false);
    }

    // ✅ Chiude il gioco
    public void EsciGioco()
    {
        Debug.Log("Chiusura del gioco...");
        Application.Quit();
    }

    // ✅ Chiude un panel passato come parametro (Torna indietro)
    public void TornaIndietro(GameObject panel)
    {
        if (panel != null)
            panel.SetActive(false);
        if (Menu != null)
            Menu.SetActive(true);
    }
    
    public void ChiudiMenu()
    {
        ResumeGame();
    }
}
