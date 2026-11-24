using UnityEngine;

public class FilterClickSFX : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    public void PlaySoundClick()
    {
        audioSource.Play();
     
    }

   
}
