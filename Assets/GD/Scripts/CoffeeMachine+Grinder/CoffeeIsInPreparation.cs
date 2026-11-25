using System.Collections;
using UnityEngine;



public class CoffeeIsInPreparation : MonoBehaviour
{
    [SerializeField] private GameObject rbFilter;
    public GameObject rbCup;
    [SerializeField] private GameObject rbLever;

    IEnumerator DelayToMakeCoffee()
    {
        Debug.Log("onegai faites que ça marche");
        yield return new WaitForSeconds(5f);
        foreach (Collider collider in rbLever.GetComponentsInChildren<Collider>(false))
        {
            rbLever.GetComponentInChildren<Collider>().enabled = true;
        }
        foreach (Collider collider in rbFilter.GetComponentsInChildren<Collider>(false))
        {
            rbFilter.GetComponentInChildren<Collider>().enabled = true;
        }
        rbCup.GetComponent<Collider>().enabled = true;
        Debug.Log("Svp faites que ça marche");
    }
    public void DesableRigidbodyOfCupLeverAndFilter()
    {
        Debug.Log("Svp dites moi que ça marche");
        foreach (Collider collider in rbLever.GetComponentsInChildren<Collider>(true))
        {
            rbLever.GetComponentInChildren<Collider>().enabled = false;
        }
        foreach (Collider collider in rbFilter.GetComponentsInChildren<Collider>(true))
        {
            rbFilter.GetComponentInChildren<Collider>().enabled = false;
        }
        rbCup.GetComponent<Collider>().enabled = false;
        StartCoroutine(DelayToMakeCoffee());
        Debug.Log("Svp je veux que ça marche");
    }
}
