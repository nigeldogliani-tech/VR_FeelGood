using UnityEngine;

using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
public class LeverVRController : MonoBehaviour
{
    public XRGrabInteractable grab;
    public HingeJoint hinge;

    [Header("Settings")]
    public float motorForce = 100f;
    public float rotationSpeed = 8f;

    private IXRSelectInteractor currentInteractor;

    private Vector3 initialHandDirection;
    private float initialLeverAngle;

    void Awake()
    {
        if (grab == null)
            grab = GetComponent<XRGrabInteractable>();

        if (hinge == null)
            hinge = GetComponent<HingeJoint>();

        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        currentInteractor = args.interactorObject;
        hinge.useSpring = false;

        Transform hand = currentInteractor.GetAttachTransform(grab);

        Vector3 axis = transform.TransformDirection(hinge.axis);

        Vector3 pivot = transform.TransformPoint(hinge.anchor);

        initialHandDirection = Vector3.ProjectOnPlane(
            hand.position - pivot, axis
        ).normalized;

        initialLeverAngle = hinge.angle;
    }

    void FixedUpdate()
    {
        if (currentInteractor == null)
            return;

        Transform hand = currentInteractor.GetAttachTransform(grab);

        Vector3 axis = transform.TransformDirection(hinge.axis);
        Vector3 pivot = transform.TransformPoint(hinge.anchor);

        Vector3 currentDirection = Vector3.ProjectOnPlane(
            hand.position - pivot, axis
        ).normalized;

        float handAngle = Vector3.SignedAngle(
            initialHandDirection,
            currentDirection,
            axis
        );

        float targetAngle = initialLeverAngle + handAngle;

        JointLimits limits = hinge.limits;

        targetAngle = Mathf.Clamp(
            targetAngle, limits.min, limits.max
        );

        float error = targetAngle - hinge.angle;

        JointMotor motor = hinge.motor;
        motor.force = motorForce;
        motor.targetVelocity = Mathf.Clamp(
            error * rotationSpeed, -100f, 100f
        );

        hinge.motor = motor;
        hinge.useMotor = true;
    }

    void OnRelease(SelectExitEventArgs args)
    {
        currentInteractor = null;
        hinge.useMotor = false;
        hinge.useSpring = true;
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
