using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OptionsMenu : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject mainMenuPanel;
    public GameObject optionPanel;

    [Header("Volume Sliders")]
    public Slider masterVol;
    public Slider musicVol;

    [Header("Audio Mixer")]
    public AudioMixer MainAudioMixer;

    // État initial au lancement de la scène
    private void Start()
    {
        mainMenuPanel.SetActive(true);
        optionPanel.SetActive(false);
    }

    // Gestion du volume général
    public void ChangeMasterVolume()
    {
        float volume = masterVol.value;

        MainAudioMixer.SetFloat(
            "MasterVol",
            Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f
        );
    }

    // Gestion du volume de la musique
    public void ChangeMusicVolume()
    {
        float volume = musicVol.value;

        MainAudioMixer.SetFloat(
            "MusicVol",
            Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f
        );
    }

    // Ouvrir le menu Options
    public void OpenOptions()
    {
        mainMenuPanel.SetActive(false);
        optionPanel.SetActive(true);
    }

    // Retourner au menu principal
    public void CloseOptions()
    {
        optionPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}
