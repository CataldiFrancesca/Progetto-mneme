using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip buttonClickClip;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void PlayButtonClick()
    {
        if (buttonClickClip != null && audioSource != null)
            audioSource.PlayOneShot(buttonClickClip);
    }
}
    