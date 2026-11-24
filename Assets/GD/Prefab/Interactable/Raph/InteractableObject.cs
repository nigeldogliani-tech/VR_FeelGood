using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class InteractableObject : XRGrabInteractable
{
    [Header("Object Settings")]
    public string itemID;
    public bool isFinalDrink;

    public bool IsGrabbed => isSelected;
    public bool IsSnapped { get; private set; }

    private Rigidbody rb;
    private Slot currentSlot;

    private Transform currentSnapPoint;

    private Vector3 localPosition;
    private Quaternion localRotation;


    protected override void Awake()
    {
        base.Awake();

        rb = GetComponent<Rigidbody>();

        // Détect joueur attrappe obj 
        selectEntered.AddListener(_ => OnGrabbed());

        // Détection lorsque le joueur relâche l'objet.
        selectExited.AddListener(_ => OnReleased());
    }


    private void OnGrabbed()
    {
        // Si objet dans slot = libère slot.
        if (currentSlot != null)
        {
            currentSlot.ClearSlot(this);
            currentSlot = null;
        }

        IsSnapped = false;

        // Arrête le suivi du SnapPoint.
        currentSnapPoint = null;

        // Réactive la physique de l'objet.
        rb.isKinematic = false;

        rb.constraints = RigidbodyConstraints.None;
    }


    private void OnReleased()
    {
        if (IsSnapped)
            return;

        // Active la physique lorsque le joueur lâche l'objet.
        rb.isKinematic = false;
    }

    private void LateUpdate()
    {
        if (!IsSnapped || currentSnapPoint == null)
            return;

        // Suit la position sans hériter de la scale.
        transform.position = currentSnapPoint.position + currentSnapPoint.rotation * localPosition;

        // Suit la rotation.
        transform.rotation = currentSnapPoint.rotation * localRotation;
    }


    public void SnapToSlot(Transform snapPoint, Slot slot, SlotType slotType)
    {
        // Empêche de placer un objet déjà attrapé ou placé.
        if (IsGrabbed || IsSnapped)
        {
            return;
        }

        IsSnapped = true;
        currentSlot = slot;

        // Mémorise la position et la rotation avant le placement.
        Vector3 savedPosition = transform.position;
        float savedYRotation = transform.eulerAngles.y;

        // Arrête les mouvements physiques.
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Désactive la simulation physique pendant le placement.
        rb.isKinematic = true;


        if (slotType == SlotType.FreePlacement)
        {
            transform.position = new Vector3(savedPosition.x,snapPoint.position.y,savedPosition.z);

            // Conserve la rotation horizontale du joueur.
            transform.rotation = Quaternion.Euler(0f, savedYRotation,0f);
        }

        else if (slotType == SlotType.FixedPlacement)
        {
            // Aligne l'objet sur le SnapPoint.
            transform.position = snapPoint.position;
            transform.rotation = Quaternion.Euler(0f, savedYRotation, 0f);
        }

        // Actualise les positions des Colliders.
        Physics.SyncTransforms();

        // Récupère tous les Colliders de l'objet et de ses enfants.
        Collider[] objectColliders = GetComponentsInChildren<Collider>();

        // Recherche le point le plus bas de l'objet.
        float lowestPoint = float.PositiveInfinity;

       foreach (Collider objectCollider in objectColliders)
        {
           if (!objectCollider.enabled)
            {
                continue;
            }

            lowestPoint = Mathf.Min(lowestPoint,objectCollider.bounds.min.y);
        }

        // Si un Collider existe, pose sa base sur la surface.
        if (!float.IsPositiveInfinity(lowestPoint))
        {
            float heightCorrection = snapPoint.position.y - lowestPoint;

            transform.position += Vector3.up * heightCorrection;
        }
        // Enregistre le SnapPoint à suivre.
        currentSnapPoint = snapPoint;

        // Mémorise la position relative sans prendre en compte la scale.
        localPosition = Quaternion.Inverse(snapPoint.rotation) * (transform.position - snapPoint.position);

        // Mémorise la rotation relative.
        localRotation = Quaternion.Inverse(snapPoint.rotation) * transform.rotation;
    }
}
