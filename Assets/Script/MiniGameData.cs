using System.Collections.Generic;
    

[System.Serializable]
public class ProvettaData
{
    public List<string> colori = new List<string>();   // inizializza
    public List<float> volumi = new List<float>();     // inizializza
}

[System.Serializable]
public class MinigiocoLiquidiData
{
    public List<ProvettaData> provette = new List<ProvettaData>();
}

[System.Serializable]
public class MiniGameData
{
    public string miniGameName;
    public bool isActive;
    public int punteggio;

    public string jsonData;
    public List<string> provetteTrovate = new List<string>();
    public bool popupAttivo;
    public bool victoryPanelActive;

    public int differenzeTrovate;
    public List<bool> differenzeIndovinate = new List<bool>();
    public string fraseUtente;

     public List<int> slotImageIndexes = new List<int>();

}
[System.Serializable]
public class MinigiocoAppuntiData
{
    public List<int> imageIndices = new List<int>();
}

[System.Serializable]
public class MiniGiocoPortaDataa
{
    public bool completato = false;
}
[System.Serializable]
public class TestTubeMixerData
{
    public bool combinazioneCorretta;
    public List<string> selectedColorsHex = new List<string>();
}

[System.Serializable]
public class MicroscopioGameStatusData
{
    public bool completato;
}
