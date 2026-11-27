using UnityEngine;

public class CrankScript : MonoBehaviour
{
    public HingeJoint CrankJoint;
    public float activationAngle = 350f;

    [SerializeField] private MakeCoffee MakeCoffee;

    [SerializeField] private AudioSource crankAudioSource;
    [SerializeField] private AudioClip crankSound;
    [SerializeField] private float movementThreshold = 0.05f;
    [SerializeField] private float stopDelay = 0.2f;

    public float turns;
    private float previousAngle;
    private float totalAngle;
    private float movementTimer;
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

        if (Mathf.Abs(delta) > movementThreshold)// Détection du mouvement
        {
            movementTimer = stopDelay;

            if (crankAudioSource != null &&// Lance le son s'il n'est pas déjà en cours
                crankSound != null &&
                !crankAudioSource.isPlaying)
            {
                crankAudioSource.Play();
            }
        }
        else
        {
            movementTimer -= Time.deltaTime;// Petit délai avant de couper le son

            if (movementTimer <= 0f)
            {
                if (crankAudioSource != null &&
                    crankAudioSource.isPlaying)
                {
                    crankAudioSource.Stop();
                }
            }
        }

        totalAngle += Mathf.Abs(delta);// Comptage des rotations

        if (totalAngle >= 360f)
        {
            totalAngle -= 360f;

            if (MakeCoffee != null &&
                MakeCoffee.isInPlaced &&
                MakeCoffee.isEmpty)
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
    }

    private void OnDisable()
    {
        if (crankAudioSource != null &&
            crankAudioSource.isPlaying)
        {
            crankAudioSource.Stop();
        }
    }
}