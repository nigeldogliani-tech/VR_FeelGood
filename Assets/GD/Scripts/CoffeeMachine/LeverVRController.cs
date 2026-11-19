using UnityEngine;
using UnityEngine.UIElements;
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

    [Range(0.1f, 1f)]
    public float sensitivity = 0.4f;
    private IXRSelectInteractor currentInteractor;

    private Vector3 initialHandDirection;
    private float initialLeverAngle;

    private bool validGrab = false;

    void Awake()
    {
        if (grab == null)
            grab = GetComponent<XRGrabInteractable>();

        if (hinge == null)
            hinge = GetComponent<HingeJoint>();

        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);
    }

    bool IsFinite(Vector3 v)
    {
        return IsFinite(v.x) &&
               IsFinite(v.y) &&
               IsFinite(v.z);
    }

    bool IsFinite(float v)
    {
        return !float.IsNaN(v) && !float.IsInfinity(v);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        currentInteractor = args.interactorObject;

        hinge.useSpring = false;
        hinge.useMotor = false;

        Transform hand = currentInteractor.GetAttachTransform(grab);

        Vector3 axis = transform.TransformDirection(hinge.axis).normalized;
        Vector3 pivot = transform.TransformPoint(hinge.anchor);

        Vector3 direction = Vector3.ProjectOnPlane(
            hand.position - pivot, axis
        );

        // Evite les vecteurs nuls ou invalides
        if (!IsFinite(direction) || direction.sqrMagnitude < 0.0001f)
        {
            validGrab = false;
            return;
        }

        initialHandDirection = direction.normalized;
        initialLeverAngle = hinge.angle;

        validGrab = true;
    }

    void FixedUpdate()
    {
        if (currentInteractor == null || !validGrab)
            return;

        Transform hand = currentInteractor.GetAttachTransform(grab);

        Vector3 axis = transform.TransformDirection(hinge.axis).normalized;
        Vector3 pivot = transform.TransformPoint(hinge.anchor);

        Vector3 direction = Vector3.ProjectOnPlane(
            hand.position - pivot, axis
        );

        if (!IsFinite(direction) || direction.sqrMagnitude < 0.0001f)
            return;

        Vector3 currentDirection = direction.normalized;

        float handAngle = Vector3.SignedAngle(
            initialHandDirection,
            currentDirection,
            axis
        );

        float targetAngle = initialLeverAngle + handAngle * sensitivity;

        if (!IsFinite(targetAngle) || !IsFinite(hinge.angle))
            return;

        JointLimits limits = hinge.limits;

        targetAngle = Mathf.Clamp(
            targetAngle,
            limits.min,
            limits.max
        );

        float error = targetAngle - hinge.angle;

        JointMotor motor = hinge.motor;

        motor.force = motorForce;
        motor.targetVelocity = Mathf.Clamp(
            error * rotationSpeed,
            -100f,
            100f
        );

        if (!IsFinite(motor.targetVelocity))
            return;

        hinge.motor = motor;
        hinge.useMotor = true;
    }

    void OnRelease(SelectExitEventArgs args)
    {
        currentInteractor = null;
        validGrab = false;

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
