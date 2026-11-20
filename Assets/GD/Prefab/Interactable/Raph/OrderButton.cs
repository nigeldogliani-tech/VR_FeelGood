using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class OrderButton : MonoBehaviour
{
    [SerializeField] private Slot deliverySlot;

    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (grabInteractable == null)
        {
            Debug.LogError(" po de XRGrabInteractable sur bouton");
            return;
        }

        //valide quand joueur attrape btn 
        grabInteractable.selectEntered.AddListener(_ => OnButtonGrabbed());
    }

    private void OnButtonGrabbed()
    {
        if (deliverySlot == null)
        {
            Debug.LogWarning("no delivcery slot 4 btn");
            return;
        }

        Debug.Log("btn ok");

        deliverySlot.ValidateCurrentOrder();
    }
}
