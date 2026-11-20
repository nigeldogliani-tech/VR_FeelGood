using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class CrankVRController : MonoBehaviour
{
    public XRGrabInteractable grab;
    public HingeJoint hinge;
    public Rigidbody rb;

    private IXRSelectInteractor interactor;

    private Vector3 previousHandDirection;

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

        Vector3 axis = transform.TransformDirection(
            hinge.axis
        ).normalized;

        Vector3 pivot = transform.TransformPoint(hinge.anchor);

        // Direction initiale de la main par rapport au pivot
        Vector3 direction = Vector3.ProjectOnPlane(
            hand.position - pivot,
            axis
        );

        if (direction.sqrMagnitude < 0.0001f)
        {
            interactor = null;
            return;
        }

        previousHandDirection = direction.normalized;
    }

    void FixedUpdate()
    {
        if (interactor == null)
            return;

        Transform hand = interactor.GetAttachTransform(grab);

        Vector3 axis = transform.TransformDirection(
            hinge.axis
        ).normalized;

        Vector3 pivot = transform.TransformPoint(hinge.anchor);

        // Direction actuelle de la main
        Vector3 direction = Vector3.ProjectOnPlane(
            hand.position - pivot,
            axis
        );

        if (direction.sqrMagnitude < 0.0001f)
            return;

        Vector3 currentHandDirection = direction.normalized;

        // Calcul du petit angle parcouru depuis la dernière frame
        float deltaAngle = Vector3.SignedAngle(
            previousHandDirection,
            currentHandDirection,
            axis
        );

        // Appliquer la rotation au Rigidbody
        Quaternion rotation = Quaternion.AngleAxis(
            deltaAngle,
            axis
        );

        rb.MoveRotation(rotation * rb.rotation);

        // Mémoriser la direction pour la prochaine frame
        previousHandDirection = currentHandDirection;
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
