using UnityEngine;
using System.Collections.Generic;

public class MinigameImageDatabase : MonoBehaviour
{
    public static MinigameImageDatabase Instance;

    [Tooltip("Lista di prefab delle immagini, in ordine corrispondente agli indici.")]
    public List<GameObject> imagePrefabs;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }

    public GameObject GetImageByIndex(int index)
    {
        if (index >= 0 && index < imagePrefabs.Count)
        {
            return imagePrefabs[index];
        }

        Debug.LogWarning("Indice immagine non valido: " + index);
        return null;
    }
}
