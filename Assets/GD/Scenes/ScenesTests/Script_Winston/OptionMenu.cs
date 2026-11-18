
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject optionPanel;

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
