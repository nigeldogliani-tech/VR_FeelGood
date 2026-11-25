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
        rbLever.GetComponent<Collider>().enabled = true;
        rbFilter.GetComponent<Collider>().enabled = true;
        rbCup.GetComponent<Collider>().enabled = true;
        Debug.Log("Svp faites que ça marche");
    }
    public void DesableRigidbodyOfCupLeverAndFilter()
    {
        Debug.Log("Svp dites moi que ça marche");
        rbLever.GetComponent<Collider>().enabled = false;
        rbFilter.GetComponent<Collider>().enabled = false;
        rbCup.GetComponent<Collider>().enabled = false;
        StartCoroutine(DelayToMakeCoffee());
        Debug.Log("Svp je veux que ça marche");
    }
}
