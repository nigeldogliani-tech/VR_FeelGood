using UnityEngine;

public class CrankScript : MonoBehaviour
{
    public HingeJoint CrankJoint;
    public float activationAngle = 350f;
    

   

    void Update()
    {
        float angle = Mathf.Abs(CrankJoint.angle);

        if (angle >= activationAngle )
        {
            // event de la machine à moudre à foutre ici
            
            Debug.Log("Crank");
            activationAngle = 0f;
        }

       

    }
}
