using UnityEngine;

public class EnableGraviti : MonoBehaviour
{
    private Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    public void isFixed()
    {
        rb.useGravity = false;
    }
    public void isNotFixed()
    {
        rb.useGravity = true;
    }
}
