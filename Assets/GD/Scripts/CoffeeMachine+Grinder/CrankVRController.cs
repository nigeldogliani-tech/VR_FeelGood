using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class CrankVRController : MonoBehaviour
{
    public XRGrabInteractable grab;
    public HingeJoint hinge;
    public Rigidbody rb;

    [Header("Grab Line")]
    public Transform rodStart;
    public Transform rodEnd;

    private IXRSelectInteractor interactor;

    private Vector3 initialGrabDirection;
    private Quaternion initialLeverRotation;
    private float initialHingeAngle;

    void Awake()
    {
        if (grab == null)
            grab = GetComponent<XRGrabInteractable>();

        if (hinge == null)
            hinge = GetComponent<HingeJoint>();

        if (rb == null)
            rb = GetComponent<Rigidbody>();

        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        interactor = args.interactorObject;

        Transform hand = interactor.GetAttachTransform(grab);
        initialHingeAngle = hinge.angle;

        Vector3 axis = transform.TransformDirection(hinge.axis).normalized;
        Vector3 pivot = transform.TransformPoint(hinge.anchor);

        // Ligne de la tige
        Vector3 rod = rodEnd.position - rodStart.position;

        // Point le plus proche sur la ligne
        float t = Vector3.Dot(hand.position - rodStart.position, rod)
                  / rod.sqrMagnitude;

        t = Mathf.Clamp01(t);

        Vector3 closestPoint = rodStart.position + t * rod;

        // Direction du point de grab depuis le pivot
        initialGrabDirection = Vector3.ProjectOnPlane(
        hand.position - pivot, axis
        ).normalized;

        initialLeverRotation = rb.rotation;
        initialHingeAngle = hinge.angle;
    }

    void FixedUpdate()
    {
        if (interactor == null)
            return;

        Transform hand = interactor.GetAttachTransform(grab);

        Vector3 axis = transform.TransformDirection(hinge.axis).normalized;
        Vector3 pivot = transform.TransformPoint(hinge.anchor);

        // Direction actuelle entre pivot et main
        Vector3 currentDirection = Vector3.ProjectOnPlane(
            hand.position - pivot, axis
        ).normalized;

        // Angle parcouru par la main
        float deltaAngle = Vector3.SignedAngle(
            initialGrabDirection,
            currentDirection,
            axis
        );

        // Angle désiré
        float targetAngle = initialHingeAngle + deltaAngle;

        // Respect des limites
        if (hinge.useLimits)
        {
            targetAngle = Mathf.Clamp(
                targetAngle,
                hinge.limits.min,
                hinge.limits.max
            );
        }

        float appliedAngle = targetAngle - initialHingeAngle;

        // Rotation autour de l'axe du Hinge
        Quaternion targetRotation =
            Quaternion.AngleAxis(appliedAngle, axis)
            * initialLeverRotation;

        rb.MoveRotation(targetRotation);
    }

    void OnRelease(SelectExitEventArgs args)
    {
        interactor = null;
    }

    void OnDestroy()
    {
        if (grab != null)
        {
            grab.selectEntered.RemoveListener(OnGrab);
            grab.selectExited.RemoveListener(OnRelease);
        }
    }
}

