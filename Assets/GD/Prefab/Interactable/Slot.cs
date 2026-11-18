using UnityEngine;
public enum SlotType
{
    FreePlacement,  // L'objet garde sa position de dépôt
    FixedPlacement  // L'objet va à un endroit précis
}
public class Slot : MonoBehaviour
{
    [Header("Slot Settings")]
    public SlotType slotType;
    public string acceptedItemID;
    public Transform snapPoint;


    // Obj déjà dans Slot
    private InteractableObject currentObject;


    private void OnTriggerStay(Collider other)
    {
        InteractableObject obj = other.GetComponentInParent<InteractableObject>();

        // po d'obj interactible
        if (obj == null)
        {
            return;
        }

        // slot occupé
        if (currentObject != null)
        {
            return;
        }

        // obj déjà placé
        if (obj.IsSnapped)
        {
            return;
        }

        // joueur tient l'obj
        if (obj.IsGrabbed)
        {
            return;
        }

        // obj compatible ?
        if (obj.itemID != acceptedItemID)
        {
            return;
        }

        // obj placé dans slot 
        obj.SnapToSlot(snapPoint, this, slotType);

        // mémorise obj placé 
        currentObject = obj;
    }


    public void ClearSlot(InteractableObject obj)
    {
        // obj bien présent dans le slot ? 
        if (currentObject == obj)
        {
            currentObject = null;
        }
    }
}
