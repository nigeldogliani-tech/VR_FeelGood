using UnityEngine;

public class CrankScript : MonoBehaviour
{
    public HingeJoint CrankJoint;
    public float activationAngle = 350f;
    [SerializeField] private MakeCoffee MakeCoffee;
    public float turns;
    private float previousAngle;
    private float totalAngle;


    void Start()
    {
        previousAngle = CrankJoint.angle;
    }

    void Update()
    {
        float currentAngle = CrankJoint.angle;

        float delta = Mathf.DeltaAngle(previousAngle, currentAngle);

        previousAngle = currentAngle;

        totalAngle += Mathf.Abs(delta);

        if (totalAngle >= 360f)
        {
            totalAngle -= 360f;
            if (MakeCoffee.isInPlaced == true && MakeCoffee.isEmpty == true)
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
}
