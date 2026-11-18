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

        // Mémorise la position, la rotation et la taille globales.
        Vector3 savedPosition = transform.position;
        Vector3 currentEuler = transform.eulerAngles;
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
            // Conserve la position de dépôt.
            transform.position = savedPosition;

            // Conserve uniquement la rotation horizontale.
            transform.rotation = Quaternion.Euler(
                0f,
                currentEuler.y,
                0f
            );
        }
        else if (slotType == SlotType.FixedPlacement)
        {
            // Aligne l'objet sur la rotation du SnapPoint.
            transform.rotation = snapPoint.rotation;

            // Récupère les dimensions du collider.
            Collider objectCollider = GetComponentInChildren<Collider>();

            if (objectCollider != null)
            {
                // Calcule la hauteur de l'objet.
                float objectHeight = objectCollider.bounds.size.y;

                // Remonte le pivot de la moitié de la hauteur.
                transform.position = snapPoint.position
                                   + Vector3.up * (objectHeight / 2f);
            }
            else
            {
                // Si aucun collider n'est trouvé.
                transform.position = snapPoint.position;
            }
        }
        Vector3 parentScale = snapPoint.lossyScale;

        transform.localScale = new Vector3(savedScale.x / parentScale.x,savedScale.y / parentScale.y,savedScale.z / parentScale.z);
    }
}
