using UnityEngine;

public class MakeCoffee : MonoBehaviour
{
    private bool isInPlaced = false;
    public bool cupIsPlaced = false;
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
        if(isInPlaced)
        {
            if (cupIsPlaced == true)
            {
                Debug.Log("is Start");
            }
        }
    }
    public void startGrind()
    {
        if (isInPlaced)
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
            }
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
