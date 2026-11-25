using System.Collections;
using UnityEngine;

public class CoffeeIsInPreparation : MonoBehaviour
{
    [SerializeField] private GameObject rbFilter;
    public GameObject rbCup;
    [SerializeField] private GameObject rbLever;

    IEnumerator DelayToMakeCoffee()
    {
        yield return new WaitForSeconds(5f);
        rbLever.GetComponent<Collider>().enabled = true;
        rbFilter.GetComponent<Collider>().enabled = true;
        rbCup.GetComponent<Collider>().enabled = true;
    }
    public void DesableRigidbodyOfCupLeverAndFilter()
    {
        rbLever.GetComponent<Collider>().enabled = false;
        rbFilter.GetComponent<Collider>().enabled = false;
        rbCup.GetComponent<Collider>().enabled = false;
        DelayToMakeCoffee();
    }
}
