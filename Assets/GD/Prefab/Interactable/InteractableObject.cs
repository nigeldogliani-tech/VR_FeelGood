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
 
        selectEntered.AddListener(_ => OnGrabbed());// detection joueur attrape obj
        selectExited.AddListener(_ => OnReleased());// detection joueur relâche obj
    }


    private void OnGrabbed()
    {
        // si obj dans ce slot = slot libéré 
        if (currentSlot != null)
        {
            currentSlot.ClearSlot(this);

            currentSlot = null;
        }

        IsSnapped = false;

        // obj détaché snappoint
        transform.SetParent(null, true);
    }

    private void OnReleased()
    {
        // On réactive la physique après le Grab.
        rb.isKinematic = false;
    }

    public void SnapToSlot(Transform snapPoint, Slot slot)
    {
        // Empêche de placer un objet encore attrapé.
        if (IsGrabbed || IsSnapped)
        {
            return;
        }

        IsSnapped = true;

        currentSlot = slot;

        // po mvt physique
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // désactive physique
        rb.isKinematic = true;

        // obj fixé à snapppoint
        transform.SetParent(snapPoint, false);

        // même les pos+rot
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
}
