
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(HingeJoint))]
[RequireComponent(typeof(Rigidbody))]
public class VRLeverSpring : MonoBehaviour
{
    [Header("Components")]
    public HingeJoint hinge;
    public XRGrabInteractable grab;

    [Header("Spring")]
    public float springForce = 100f;
    public float springDamper = 10f;

    [Header("Hand Control")]
    public float controlStrength = 500f;
    public float controlDamper = 30f;

    private Transform hand;
    private Vector3 startHandDirection;
    private float startLeverAngle;
    private float targetAngle;

    private Rigidbody rb;
    private bool isGrabbed;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (hinge == null)
            hinge = GetComponent<HingeJoint>();

        if (grab == null)
            grab = GetComponent<XRGrabInteractable>();

        hinge.useSpring = true;

        JointSpring spring = hinge.spring;
        spring.spring = springForce;
        spring.damper = springDamper;
        spring.targetPosition = 0f;
        hinge.spring = spring;

        grab.trackPosition = false;
        grab.trackRotation = false;

        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        hand = args.interactorObject.GetAttachTransform(grab);

        Vector3 pivot = transform.TransformPoint(hinge.anchor);

        Vector3 axis = transform.TransformDirection(hinge.axis);

        Vector3 direction = Vector3.ProjectOnPlane(
            hand.position - pivot, axis
        ).normalized;

        startHandDirection = direction;
        startLeverAngle = hinge.angle;
        targetAngle = startLeverAngle;

        hinge.useSpring = false;
        isGrabbed = true;
    }

    void FixedUpdate()
    {
        if (!isGrabbed || hand == null)
            return;

        Vector3 pivot = transform.TransformPoint(hinge.anchor);
        Vector3 axis = transform.TransformDirection(hinge.axis);

        Vector3 direction = Vector3.ProjectOnPlane(
            hand.position - pivot, axis
        ).normalized;

        float movement = Vector3.SignedAngle(
            startHandDirection, direction, axis
        );

        targetAngle = startLeverAngle + movement;

        float error = Mathf.DeltaAngle(
            hinge.angle, targetAngle
        );

        float angularVelocity =
            Vector3.Dot(rb.angularVelocity, axis) * Mathf.Rad2Deg;

        float torque =
            error * controlStrength -
            angularVelocity * controlDamper;

        rb.AddTorque(axis * torque, ForceMode.Force);
    }

    void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed = false;
        hand = null;

        hinge.useSpring = true;
    }
}
