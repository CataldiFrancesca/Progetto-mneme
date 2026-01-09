using UnityEngine;

public class OpenClose : MonoBehaviour
{
    private Animator animator;
    private bool isOpen = false;

    // Nomi dei trigger (puoi cambiarli qui facilmente)
    private readonly string openTrigger = "TrOpen";
    private readonly string closeTrigger = "TrClose";

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnMouseDown()
    {
        if (animator == null) return;

        if (!isOpen)
        {
            animator.ResetTrigger(closeTrigger); // Previene conflitti
            animator.SetTrigger(openTrigger);
            isOpen = true;
        }
        else
        {
            animator.ResetTrigger(openTrigger);  // Previene conflitti
            animator.SetTrigger(closeTrigger);
            isOpen = false;
        }
    }
}