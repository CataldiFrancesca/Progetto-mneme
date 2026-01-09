using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public string scenaCorrente;
    public float playerX;
    public float playerY;
    public float playerZ;

    // Assicurati che questa lista sia presente e inizializzata
    public List<MiniGameData> miniGames = new List<MiniGameData>();

}
