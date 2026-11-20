
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
        MainAudioMixer.SetFloat("MasterVol", masterVol.value);           
    }

    public void ChangeMusicVolume()
    {
        MainAudioMixer.SetFloat("MusicVol", musicVol.value);           
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
