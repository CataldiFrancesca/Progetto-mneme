using UnityEngine;
using TMPro;

public class DigitSlot : MonoBehaviour
{
    public TMP_Text numberText;

    private int currentNumber = 0;

    private void Start()
    {
        UpdateDisplay();
    }

    public void Increment()
    {
        currentNumber = (currentNumber + 1) % 10;
        UpdateDisplay();
    }

    public void Decrement()
    {
        currentNumber = (currentNumber - 1 + 10) % 10;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (numberText != null)
        {
            numberText.text = currentNumber.ToString();
        }
        else
        {
            Debug.LogWarning($"numberText non assegnato su {gameObject.name}!");
        }
    }

    public int GetCurrentNumber()
    {
        return currentNumber;
    }

    public void ResetDigit()
    {
        currentNumber = 0;
        UpdateDisplay();
    }

    public void SetDigit(int number)
    {
        currentNumber = Mathf.Clamp(number, 0, 9);
        UpdateDisplay();
    }
}
