using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SnapInGrab : MonoBehaviour
{
    private XRSocketInteractor socket;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        socket = GetComponent<XRSocketInteractor>();
    }

    void OnEnable()
    {
        socket.hoverEntered.AddListener(ForcerLeSnap);
    }

    void OnDisable()
    {
        socket.hoverEntered.RemoveListener(ForcerLeSnap);
    }

    private void ForcerLeSnap(HoverEnterEventArgs args)
    {
        IXRSelectInteractable objet = args.interactableObject as IXRSelectInteractable;
        if(objet != null && objet.isSelected)
        {
            socket.interactionManager.SelectEnter((IXRSelectInteractor)socket, objet);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
