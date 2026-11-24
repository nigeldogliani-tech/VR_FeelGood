using UnityEngine;

public class TamperScript : MonoBehaviour
{
    [SerializeField] private GameObject tamper;
    [SerializeField] private MakeCoffee MakeCoffee;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == tamper)
        {
            MakeCoffee.Stamped();
            Debug.Log("je marche youpi");
        }
    }
}
