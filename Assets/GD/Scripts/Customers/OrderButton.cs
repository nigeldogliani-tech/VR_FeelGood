using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class OrderButton : MonoBehaviour
{
    [SerializeField] private Slot deliverySlot;
    [SerializeField] private CustomerManager customerManager;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip successSound;
    [SerializeField] private AudioClip errorSound;

    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (grabInteractable == null)
        {
            Debug.LogError(" po de XRGrabInteractable sur bouton");
            return;
        }
        if(audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        //valide quand joueur attrape btn 
        grabInteractable.selectEntered.AddListener(OnButtonGrabbed);
    }

    private void OnButtonGrabbed(UnityEngine.XR.Interaction.Toolkit.SelectEnterEventArgs args)
    {
        if (deliverySlot == null)
        {
            Debug.LogWarning("no delivery slot assigned");
            PlayErrorSound();
            return;
        }
        Debug.Log("btn ok");
        bool orderValidated = deliverySlot.ValidateCurrentOrder();

        if (orderValidated)
        {
            PlaySuccessSound();
        }
        else
        {
            PlayErrorSound();

            if (customerManager != null)
            {
                customerManager.ShowWrongDrinkFeedback();
            }
            else
            {
                Debug.LogWarning("CustomerManager non assigné !");
            }
        }
    }

    private void PlaySuccessSound()
    {
        if (audioSource != null && successSound != null)
        {
            audioSource.PlayOneShot(successSound);
        }
        else
        {
            Debug.LogWarning("audiosource/successsound po la");
        }
    }
    private void PlayErrorSound()
    {
        if (audioSource != null && errorSound != null)
        {
            audioSource.PlayOneShot(errorSound);
        }
        else
        {
            Debug.LogWarning("audiosource/errorsound po la");
        }
    }
    private void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnButtonGrabbed);
        }
    }

}