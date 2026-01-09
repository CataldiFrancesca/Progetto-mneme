using UnityEngine;

public class IngredientClick : MonoBehaviour
{
    public GameObject[] lettereDaMostrare;

    public void RivelaLettere()
    {
        foreach (GameObject lettera in lettereDaMostrare)
        {
            lettera.SetActive(true);
        }
    }
}
