using UnityEngine;
using System.Collections.Generic;

public class GameLoader : MonoBehaviour
{
    public static GameLoader Instance;

    public SaveData datiCaricati;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
