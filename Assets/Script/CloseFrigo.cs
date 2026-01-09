using UnityEngine;

public class CloseFrigo : MonoBehaviour
{
    private Animator animator;
    private bool isOpen = true;

    // Nomi dei trigger (puoi cambiarli qui facilmente)
    private readonly string closeTrigger = "TrClose";

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnMouseDown()
    {
        if (animator == null) return;

        if (isOpen)
        {
            animator.SetTrigger(closeTrigger);
            isOpen = false;
        }
    }
}