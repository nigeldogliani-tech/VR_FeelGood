using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class LeverScript : MonoBehaviour
{
    public HingeJoint leverJoint;
    public float activationAngle = 35f;

    [SerializeField] private MakeCoffee MakeCoffee;
    [SerializeField] private CoffeeMachineSFX CoffeeMachineSounds;

    [SerializeField] private AudioSource leverAudioSource;
    [SerializeField] private AudioClip leverSound;

    [SerializeField] private float vibrationAmplitude = 0.7f;
    [SerializeField] private float vibrationDuration = 0.15f;

    private bool activated = false;
    private XRBaseInputInteractor interactor;

    private XRGrabInteractable grabInteractable;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrab);
            grabInteractable.selectExited.AddListener(OnRelease);
        }
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        interactor = args.interactorObject as XRBaseInputInteractor;
    }

    void OnRelease(SelectExitEventArgs args)
    {
        interactor = null;
    }

    void Update()
    {
        if (leverJoint == null)
            return;

        float angle = Mathf.Abs(leverJoint.angle);

        if (angle >= activationAngle && !activated)
        {
            activated = true;

            // Vibration
            if (interactor != null)
            {
                interactor.SendHapticImpulse(vibrationAmplitude,vibrationDuration);
            }

            StartCoroutine(ActivationRoutine());
        }

        if (angle < 5f)
        {
            activated = false;
        }
    }

    private IEnumerator ActivationRoutine()
    {
        Debug.Log("Levier tiré !");

        if (leverAudioSource != null && leverSound != null)
        {
            leverAudioSource.PlayOneShot(leverSound);
            yield return new WaitForSeconds(leverSound.length);
        }

        MakeCoffee.startMachine();

        Debug.Log("Extraction du café.");

        if (CoffeeMachineSounds != null)
        {
            CoffeeMachineSounds.PlaySoundMachine();
        }
    }

    void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrab);
            grabInteractable.selectExited.RemoveListener(OnRelease);
        }
    }
}
