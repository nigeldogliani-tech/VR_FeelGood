using UnityEngine;

public class TampSFX : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    public void PlaySoundTamp()
    {
        audioSource.Play();
    }
}
