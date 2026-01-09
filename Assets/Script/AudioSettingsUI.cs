using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider sfxSlider;
    public Toggle sfxToggle; 
    public Slider musicSlider;
    public Toggle musicToggle; 

    private float sfxVolume;
    private float musicVolume;

    private const float MinVolume = 0.0001f; 
    private const float MuteThreshold = 0.001f; 

    void Start()
    {
        audioMixer.GetFloat("SFXVolume", out sfxVolume);
        audioMixer.GetFloat("MusicVolume", out musicVolume);

        sfxSlider.value = Mathf.Pow(10, sfxVolume / 20f);
        musicSlider.value = Mathf.Pow(10, musicVolume / 20f);

        sfxToggle.isOn = sfxSlider.value <= MuteThreshold;
        musicToggle.isOn = musicSlider.value <= MuteThreshold;

        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxToggle.onValueChanged.AddListener(ToggleSFX);
        musicToggle.onValueChanged.AddListener(ToggleMusic);
    }

    public void SetSFXVolume(float volume)
    {
        float db = Mathf.Log10(Mathf.Max(volume, MinVolume)) * 20;
        audioMixer.SetFloat("SFXVolume", db);

        sfxToggle.isOn = volume <= MuteThreshold;
    }

    public void SetMusicVolume(float volume)
    {
        float db = Mathf.Log10(Mathf.Max(volume, MinVolume)) * 20;
        audioMixer.SetFloat("MusicVolume", db);

        musicToggle.isOn = volume <= MuteThreshold;
    }

    public void ToggleSFX(bool mute)
    {
        sfxSlider.value = mute ? 0f : 1f;
    }

    public void ToggleMusic(bool mute)
    {
        musicSlider.value = mute ? 0f : 1f;
    }
}
