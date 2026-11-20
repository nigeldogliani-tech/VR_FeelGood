using UnityEngine;

public class CrankScript : MonoBehaviour
{
    public HingeJoint CrankJoint;
    public float activationAngle = 350f;
    [SerializeField] private MakeCoffee MakeCoffee;
    public float turns;



    void Update()
    {
        float angle = Mathf.Abs(CrankJoint.angle);

        if (angle >= activationAngle )
        {
            // event de la machine à moudre à foutre ici
            
            Debug.Log("Crank");
            activationAngle = 0f;
            turns = turns+1;
            MakeCoffee.startGrind();
            if (turns >= 3)
            {
                turns = 0f;
            }
        }

       

    }
}
