using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;



public class CoffeeIsInPreparation : MonoBehaviour
{
    [SerializeField] private GameObject rbFilter;
    public GameObject rbCup;
    [SerializeField] private GameObject rbLever;

    IEnumerator DelayToMakeCoffee()
    {
        Debug.Log("onegai faites que ça marche");
        yield return new WaitForSeconds(5f);
        rbLever.GetComponentInChildren<InteractableObject>().enabled = true;
        rbFilter.GetComponentInChildren<XRGrabInteractable>().enabled = true;
        rbCup.GetComponent<InteractableObject>().enabled = true;
        Debug.Log("Svp faites que ça marche");
    }
    public void DesableRigidbodyOfCupLeverAndFilter()
    {
        Debug.Log("Svp dites moi que ça marche");
        rbLever.GetComponentInChildren<InteractableObject>().enabled = false;
        rbFilter.GetComponentInChildren<XRGrabInteractable>().enabled = false;
        rbCup.GetComponent<InteractableObject>().enabled = false;
        StartCoroutine(DelayToMakeCoffee());
        Debug.Log("Svp je veux que ça marche");
    }
}
