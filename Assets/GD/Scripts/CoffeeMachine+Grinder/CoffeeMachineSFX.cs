using UnityEngine;

public class CoffeeMachineSFX : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    public void PlaySoundMachine()
    {
        audioSource.Play();
     
    }

}
