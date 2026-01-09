using UnityEngine;
using UnityEngine.UI;

public class PadlockNumber : MonoBehaviour
{
    public Text numberText;  // Il testo che mostra il numero
    private int currentNumber = 0;

    private const int minNumber = 0;
    private const int maxNumber = 9;

    // Chiamato dal bottone "up"
    public void IncreaseNumber()
    {
        currentNumber++;
        if (currentNumber > maxNumber)
            currentNumber = minNumber;

        UpdateDisplay();
    }

    // Chiamato dal bottone "down"
    public void DecreaseNumber()
    {
        currentNumber--;
        if (currentNumber < minNumber)
            currentNumber = maxNumber;

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        numberText.text = currentNumber.ToString();
    }

    public int GetCurrentNumber()
    {
        return currentNumber;
    }
}
