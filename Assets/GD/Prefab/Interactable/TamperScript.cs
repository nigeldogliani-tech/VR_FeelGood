using UnityEngine;

public class TamperScript : MonoBehaviour
{
    [SerializeField] private GameObject tamper;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == tamper)
        {
            Debug.Log("je marche youpi");
        }
    }
}
