using UnityEngine;

public class LeverScript : MonoBehaviour
{
    public HingeJoint leverJoint;
    public float activationAngle = 35f;

    private bool activated = false;

    void Update()
    {
        float angle = Mathf.Abs(leverJoint.angle);

        if (angle >= activationAngle && !activated)
        {
            activated = true;
            Debug.Log("Levier tiré ! Extraction du café.");
        }

        if (angle < 5f)
        {
            activated = false;
        }
    }
}
