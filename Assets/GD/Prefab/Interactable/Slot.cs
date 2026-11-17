using UnityEngine;

public class Slot : MonoBehaviour
{
    [Header("Slot Settings")]
    public string acceptedItemID;
    public Transform snapPoint;
    private InteractableObject currentObject;


    private void OnTriggerStay(Collider other)
    {
        // Recherche notre objet interactif.
        InteractableObject obj = other.GetComponentInParent<InteractableObject>();

        // po d'obj détecté
        if (obj == null)
        {
            return;
        }

        // slot déjà occupé
        if (currentObject != null)
        {
            return;
        }

        // obj déjà placé ailleurs ou ici
        if (obj.IsSnapped)
        {
            return;
        }

        // joueur tient encore obj
        if (obj.IsGrabbed)
        {
            return;
        }

        // vérifie obj compatible
        if (obj.itemID != acceptedItemID)
        {
            return;
        }

        // place obj dans slot
        obj.SnapToSlot(snapPoint, this);

        // mémorise obj placé
        currentObject = obj;
    }


    public void ClearSlot(InteractableObject obj)
    {
        // vérifie obj présent dans ce slot
        if (currentObject == obj)
        {
            currentObject = null;
        }
    }
}
