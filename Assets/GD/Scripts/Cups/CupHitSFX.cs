using UnityEngine;
using System.Collections;

public class CupHitSFX : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private AudioSource audioSource;

    private void OnCollisionEnter(Collision collision)
    {
        audioSource.Play();
        
    }

 

}
