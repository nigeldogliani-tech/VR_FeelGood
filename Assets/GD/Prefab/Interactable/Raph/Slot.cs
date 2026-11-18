using UnityEngine;

public enum SlotType
{
    FreePlacement,
    FixedPlacement
}

public class Slot : MonoBehaviour
{
    [Header("Slot Settings")]
    public SlotType slotType;

    public string acceptedItemID;
    public Transform snapPoint;

    private InteractableObject currentObject;


    private void OnTriggerStay(Collider other)
    {
        InteractableObject obj =
            other.GetComponentInParent<InteractableObject>();

        if (obj == null)
        {
            return;
        }

        // Demande au Slot de placer l'objet.
        TryPlaceObject(obj);
    }


    // Méthode utilisable par le Slot et par d'autres scripts.
    public bool TryPlaceObject(InteractableObject obj)
    {
        if (obj == null)
        {
            return false;
        }

        // Vérifie que le Slot est libre.
        if (currentObject != null)
        {
            return false;
        }

        // Vérifie que l'objet n'est pas déjà placé.
        if (obj.IsSnapped)
        {
            return false;
        }

        // Vérifie que le joueur ne tient pas l'objet.
        if (obj.IsGrabbed)
        {
            return false;
        }

        // Vérifie l'ID uniquement si un ID est renseigné.
        // Un champ vide accepte tous les objets.
        if (!string.IsNullOrEmpty(acceptedItemID)
            && obj.itemID != acceptedItemID)
        {
            return false;
        }

        if (snapPoint == null)
        {
            Debug.LogWarning("Le SnapPoint du Slot n'est pas renseigné !");
            return false;
        }

        // Place l'objet selon le type de Slot.
        obj.SnapToSlot(snapPoint, this, slotType);

        // Mémorise l'objet placé.
        currentObject = obj;

        return true;
    }


    public void ClearSlot(InteractableObject obj)
    {
        if (currentObject == obj)
        {
            currentObject = null;
        }
    }
}