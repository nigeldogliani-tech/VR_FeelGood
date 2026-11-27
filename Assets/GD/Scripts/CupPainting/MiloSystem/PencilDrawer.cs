using UnityEngine;

public class PencilDrawer : MonoBehaviour
{
    [Header("Drawing")]
    [SerializeField] private Transform tip;
    [SerializeField] private float tipLocalOffset = 1f;
    [Min(0f)] public float drawingDistance = 0.004f;
    [SerializeField, Min(0f)] private float penetrationTolerance = 0.008f;

    [Header("Laser")]
    public LineRenderer laser;

    private CupTexturePainter previousPainter;
    private CupTexturePainter.Contact previousContact;
    private Vector3 previousLocalTip;
    private Vector3 previousLocalDirection;

    private void Start()
    {
        if (laser == null)
            return;

        laser.positionCount = 2;
        laser.useWorldSpace = true;
        laser.startWidth = 0.003f;
        laser.endWidth = 0.003f;
    }

    private void LateUpdate()
    {
        if (tip == null)
            return;

        Vector3 point = tip.TransformPoint(Vector3.up * tipLocalOffset);
        Vector3 direction = tip.up;
        Ray ray = MakeContactRay(point, direction);
        float range = penetrationTolerance + drawingDistance;
        Vector3 end = point + direction * drawingDistance;

        float hitDistance = range;
        if (CupTexturePainter.FindContact(ray, ref hitDistance, out var painter, out var contact))
        {
            ContinueStroke(painter, contact, point, direction, range);
            end = ray.GetPoint(hitDistance);
        }
        else
        {
            previousPainter = null;
        }

        if (laser != null)
        {
            laser.SetPosition(0, point);
            laser.SetPosition(1, end);
        }
    }

    private void OnDisable()
    {
        previousPainter = null;
    }

    private Ray MakeContactRay(Vector3 point, Vector3 direction)
    {
        return new Ray(point - direction * penetrationTolerance, direction);
    }

    private void ContinueStroke(CupTexturePainter painter, CupTexturePainter.Contact contact,
        Vector3 point, Vector3 direction, float range)
    {
        Transform surface = painter.transform;
        CupTexturePainter.Contact? last = null;
        if (previousPainter == painter)
        {
            Vector3 start = surface.TransformPoint(previousLocalTip);
            Vector3 startDirection = surface.TransformDirection(previousLocalDirection);
            float distance = Vector3.Distance(start, point);
            int steps = Mathf.Clamp(Mathf.CeilToInt(distance / 0.001f), 1, 64);
            last = previousContact;
            for (int i = 1; i < steps; i++)
            {
                float t = (float)i / steps;
                Vector3 sampleDirection = Vector3.Slerp(startDirection, direction, t);
                Ray sample = MakeContactRay(Vector3.Lerp(start, point, t), sampleDirection);
                float sampleRange = range;
                if (CupTexturePainter.FindContact(sample, ref sampleRange, out var samplePainter, out var sampleContact)
                    && samplePainter == painter)
                {
                    painter.Draw(last, sampleContact);
                    last = sampleContact;
                }
                else
                {
                    last = null;
                }
            }
        }

        painter.Draw(last, contact);
        painter.ApplyStroke();
        previousPainter = painter;
        previousContact = contact;
        previousLocalTip = surface.InverseTransformPoint(point);
        previousLocalDirection = surface.InverseTransformDirection(direction);
    }
}
