
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OptionsMenu : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject optionPanel;
    public Slider masterVol, musicVol;
    public AudioMixer MainAudioMixer;

    public void ChangeMasterVolume()
    {
        float volume = masterVol.value;
        MainAudioMixer.SetFloat("MasterVol",Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f);
    }

    public void ChangeMusicVolume()
    {
        float volume = musicVol.value;
        MainAudioMixer.SetFloat("MusicVol",Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f);
    }


    public void OpenOptions()
    {
        mainMenuPanel.SetActive(false);
        optionPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        optionPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}
