using UnityEngine;

public class CrankScript : MonoBehaviour
{
    public HingeJoint CrankJoint;
    public float activationAngle = 350f;

    [SerializeField] private MakeCoffee MakeCoffee;
    [SerializeField] private CrankVRController crankVRController;

    [SerializeField] private AudioSource crankAudioSource;
    [SerializeField] private AudioClip crankSound;
    [SerializeField] private float movementThreshold = 0.05f;
    [SerializeField] private float stopDelay = 0.2f;

    [SerializeField] private float hapticAmplitude = 0.3f;
    [SerializeField] private float hapticDuration = 0.08f;
    [SerializeField] private float hapticInterval = 0.12f;

    public float turns;

    private float previousAngle;
    private float totalAngle;
    private float movementTimer;
    private float hapticTimer;

    void Start()
    {
        if (CrankJoint == null)
        {
            Debug.LogError("CrankJoint non assigné !");
            return;
        }

        previousAngle = CrankJoint.angle;

        if (crankAudioSource != null && crankSound != null)
        {
            crankAudioSource.clip = crankSound;
            crankAudioSource.loop = true;
            crankAudioSource.playOnAwake = false;
        }
        else
        {
            Debug.LogWarning("AudioSource ou crankSound non assigné !");
        }
    }

    void Update()
    {
        if (CrankJoint == null)
            return;

        float currentAngle = CrankJoint.angle;

        float delta = Mathf.DeltaAngle(previousAngle, currentAngle);

        previousAngle = currentAngle;

        totalAngle += Mathf.Abs(delta);// Comptage de la rotation

        if (totalAngle >= 360f)
        {
            totalAngle -= 360f;

            if (MakeCoffee != null && MakeCoffee.isInPlaced && MakeCoffee.isEmpty)
            {
                turns++;

                MakeCoffee.startGrind(turns);

                if (turns >= 3)
                {
                    turns = 0;
                }
            }

            Debug.Log("Nombre de tours : " + turns);
        }

        // Détection du mouvement
        if (Mathf.Abs(delta) > movementThreshold)
        {
            movementTimer = stopDelay;

            // Gestion des vibrations
            hapticTimer -= Time.deltaTime;

            if (hapticTimer <= 0f)
            {
                if (crankVRController != null)
                {
                    crankVRController.TriggerHaptic(hapticAmplitude,hapticDuration);
                }
                hapticTimer = hapticInterval;
            }
            if (crankAudioSource != null && crankSound != null && !crankAudioSource.isPlaying)// Lecture du son
            {
                crankAudioSource.Play();
            }
        }
        else
        {
            movementTimer -= Time.deltaTime;// Délai avant arrêt du son

            if (movementTimer <= 0f)
            {
                if (crankAudioSource != null && crankAudioSource.isPlaying)
                {
                    crankAudioSource.Stop();
                }

                hapticTimer = 0f;
            }
        }
    }
    private void OnDisable()
    {
        if (crankAudioSource != null &&
            crankAudioSource.isPlaying)
        {
            crankAudioSource.Stop();
        }

        hapticTimer = 0f;
        movementTimer = 0f;
    }
}
