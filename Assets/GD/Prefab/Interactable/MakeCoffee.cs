using UnityEngine;

public class MakeCoffee : MonoBehaviour
{
    private bool isInPlaced = false;
    public bool cupIsPlaced = false;
    private bool isTamped = false;
    private bool cupIsEmpty = true;
    private bool isEmpty = true;

    [SerializeField] private CrankScript crankScript;

    public void placedEnable()
    {
        isInPlaced = true;
        Debug.Log("is place");
    }
    public void placedDisable() 
    { 
        isInPlaced = false;
        Debug.Log("is not place");

    }
    public void startMachine()
    {
        if(isInPlaced && isTamped == true)
        {
            if (cupIsPlaced && cupIsEmpty == true)
            {
                cupIsEmpty = false;
                isEmpty = true;
                isTamped = false;

                Debug.Log("is Start");
            }
        }
    }
    public void startGrind()
    {
        if (isInPlaced && isEmpty == true)
        {
            if (crankScript.turns == 1)
            {
                Debug.Log("is Start in 1");
            }
            else if (crankScript.turns == 2)
            {
                Debug.Log("is Start in 2");
            }
            else if (crankScript.turns == 3)
            {
                Debug.Log("is Start in 3");
                isEmpty = false;
            }
        }
    }
    public void doStamped()
    {
        if (isEmpty == true && isTamped == false)
        {
            isTamped = true;
            Debug.Log("is Stamped");
        }
    }
}
