using UnityEngine;

public class MakeCoffee : MonoBehaviour
{
    private bool isInPlaced = false;
    public bool cupIsPlaced = false;

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
        if(isInPlaced == true && cupIsPlaced == true)
        {
            Debug.Log("is Start");
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
