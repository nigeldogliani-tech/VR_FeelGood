using UnityEngine;

public class MakeCoffee : MonoBehaviour
{
    public bool isInPlaced = false;
    public bool cupIsPlaced = false;
    public bool isTamped = false;
    public bool cupIsEmpty = true;
    public bool isEmpty = true;

    //[SerializeField] private CrankScript crankScript;
    [SerializeField] private Slot slot;

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
        if(isInPlaced == true && isTamped == true)
        {
            if (cupIsPlaced == true && cupIsEmpty == true)
            {
                cupIsEmpty = false;
                isEmpty = true;
                isTamped = false;
                slot.FillCup();
                Debug.Log("is Start");
            }
        }
    }
    public void startGrind(float turnsRecept)
    {
        if (isInPlaced == true && isEmpty == true)
        {
            if (turnsRecept == 1)
            {
                Debug.Log("is Start in 1");
            }
            else if (turnsRecept == 2)
            {
                Debug.Log("is Start in 2");
            }
            else if (turnsRecept == 3)
            {
                Debug.Log("is Start in 3");
                isEmpty = false;
            }
        }
    }
    public void Stamped()
    {
        if (isEmpty  == false && isTamped == false)
        {
            isTamped = true;
            Debug.Log("is Stamped");
        }
    }
}
