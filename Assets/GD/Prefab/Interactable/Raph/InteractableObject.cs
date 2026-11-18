using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class InteractableObject : XRGrabInteractable
{
    [Header("Object Settings")]
    public string itemID;

    public bool IsGrabbed => isSelected;
    public bool IsSnapped { get; private set; }

    private Rigidbody rb;
    private Slot currentSlot;


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
        // si obj dans slot = libere slot 
        if (currentSlot != null)
        {
            currentSlot.ClearSlot(this);

            currentSlot = null;
        }

        IsSnapped = false;

        // obj out of snap point 
        transform.SetParent(null, true);
    }


    private void OnReleased()
    {
        // après grab = physique activated 
        rb.isKinematic = false;
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

        // Mémorise la taille globale de l'objet.
        Vector3 savedScale = transform.lossyScale;

        // Arrête les mouvements physiques.
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Désactive la physique.
        rb.isKinematic = true;

        // Attache l'objet au SnapPoint.
        transform.SetParent(snapPoint, true);


        if (slotType == SlotType.FreePlacement)
        {
            transform.position = new Vector3(
                savedPosition.x,
                snapPoint.position.y,
                savedPosition.z
            );

            // Conserve la rotation horizontale du joueur.
            transform.rotation = Quaternion.Euler(
                0f,
                savedYRotation,
                0f
            );
        }

        else if (slotType == SlotType.FixedPlacement)
        {
            // Aligne l'objet sur le SnapPoint.
            transform.position = snapPoint.position;
            transform.rotation = snapPoint.rotation;
        }

        Vector3 parentScale = snapPoint.lossyScale;

        // Évite une division par zéro.
        if (Mathf.Abs(parentScale.x) < 0.0001f ||
            Mathf.Abs(parentScale.y) < 0.0001f ||
            Mathf.Abs(parentScale.z) < 0.0001f)
        {
            Debug.LogWarning("Le SnapPoint possède une échelle nulle.");
            return;
        }

        transform.localScale = new Vector3(
            savedScale.x / parentScale.x,
            savedScale.y / parentScale.y,
            savedScale.z / parentScale.z
        );

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

            lowestPoint = Mathf.Min(
                lowestPoint,
                objectCollider.bounds.min.y
            );
        }

        // Si un Collider existe, pose sa base sur la surface.
        if (!float.IsPositiveInfinity(lowestPoint))
        {
            float heightCorrection = snapPoint.position.y - lowestPoint;

            transform.position += Vector3.up * heightCorrection;
        }
    }
}
