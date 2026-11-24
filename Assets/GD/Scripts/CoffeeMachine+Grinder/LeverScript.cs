using UnityEditorInternal;
using UnityEngine;

public class LeverScript : MonoBehaviour
{
    public HingeJoint leverJoint;
    public float activationAngle = 35f;
    [SerializeField] private MakeCoffee MakeCoffee;

    private bool activated = false;

    void Update()
    {
        float angle = Mathf.Abs(leverJoint.angle);

        if (angle >= activationAngle && !activated)
        {
            activated = true;
            MakeCoffee.startMachine();
            Debug.Log("Levier tiré ! Extraction du café.");
        }

        if (angle < 5f)
        {
            activated = false;
        }
    }
}
