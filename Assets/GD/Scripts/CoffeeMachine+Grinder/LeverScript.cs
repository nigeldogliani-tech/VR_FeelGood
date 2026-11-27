using UnityEngine;
using System.Collections;

public class LeverScript : MonoBehaviour
{
    public HingeJoint leverJoint;
    public float activationAngle = 35f;

    [SerializeField] private MakeCoffee MakeCoffee;
    [SerializeField] private CoffeeMachineSFX CoffeeMachineSounds;

    [SerializeField] private AudioSource leverAudioSource;
    [SerializeField] private AudioClip leverSound;

    private bool activated = false;
    void Update()
    {
        float angle = Mathf.Abs(leverJoint.angle);

        if (angle >= activationAngle && !activated)
        {
            activated = true;
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

        if (leverAudioSource != null && leverSound != null)// Joue le son du levier
        {
            leverAudioSource.PlayOneShot(leverSound);
            yield return new WaitForSeconds(leverSound.length);// Attend la fin du son du levier
        }

        MakeCoffee.startMachine();// Lance ensuite la machine

        Debug.Log("Extraction du café.");

        if (CoffeeMachineSounds != null)
        {
            CoffeeMachineSounds.PlaySoundMachine();
        }
    }
}