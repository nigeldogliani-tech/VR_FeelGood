using UnityEngine;

public class ObjectRecoveryZone : MonoBehaviour
{
    [Header("Recovery Settings")]

    // Slot sur lequel les objets vont réapparaître.
    public Slot recoverySlot;


    private void OnTriggerEnter(Collider other)
    {
        InteractableObject obj = other.GetComponentInParent<InteractableObject>();

        // Aucun objet interactif détecté.
        if (obj == null)
        {
            return;
        }

        // Ignore les objets sans itemID.
        //if (string.IsNullOrEmpty(obj.itemID))
        //{
            //return;
        //}

        // Ne récupère pas un objet encore tenu par le joueur.
        if (obj.IsGrabbed)
        {
            return;
        }

        // Vérifie que le Slot de récupération existe.
        if (recoverySlot == null)
        {
            Debug.LogWarning("Aucun Slot de récupération assigné !");
            return;
        }

        // Demande au Slot de récupérer l'objet.
        bool recovered = recoverySlot.TryPlaceObject(obj);

        if (recovered)
        {
            Debug.Log("Objet récupéré : " + obj.itemID);
        }
    }
}