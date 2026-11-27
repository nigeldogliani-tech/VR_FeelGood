
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class WhoIsGrab : MonoBehaviour
{
    private NearFarInteractor interactor;
    public GameObject grabbedObject;

    private void Awake()
    {
        interactor = GetComponent<NearFarInteractor>();
    }

    private void OnEnable()
    {
        if (interactor != null)
        {
            interactor.selectEntered.AddListener(OnObjectGrabbed);
            interactor.selectExited.AddListener(OnObjectReleased);
        }
    }

    private void OnDisable()
    {
        if (interactor != null)
        {
            interactor.selectEntered.RemoveListener(OnObjectGrabbed);
            interactor.selectExited.RemoveListener(OnObjectReleased);
        }
    }

    private void OnObjectGrabbed(SelectEnterEventArgs args)
    {
        grabbedObject = args.interactableObject.transform.gameObject;
        Debug.Log("Objet attrapé : " + grabbedObject.name);
    }

    private void OnObjectReleased(SelectExitEventArgs args)
    {
        if (grabbedObject == args.interactableObject.transform.gameObject)
        {
            Debug.Log("Objet relâché : " + grabbedObject.name);
            grabbedObject = null;
        }
    }
}
