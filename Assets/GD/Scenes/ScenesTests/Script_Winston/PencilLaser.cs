



using UnityEngine;

public class PencilLaser : MonoBehaviour
{
    public float laserDistance = 0.2f;

    private LineRenderer laser;

    void Start()
    {
        laser = GetComponent<LineRenderer>();

        laser.positionCount = 2;
        laser.useWorldSpace = true;
        laser.startWidth = 0.003f;
        laser.endWidth = 0.003f;
    }

    void Update()
    {
        Vector3 start = transform.position;
        Vector3 direction = transform.up;

        Vector3 end = start + direction * laserDistance;

        RaycastHit[] hits = Physics.RaycastAll(
            start, direction, laserDistance
        );

        foreach (RaycastHit hit in hits)
        {
            BoxCollider box = hit.collider as BoxCollider;

            if (box == null)
                continue;

            CubeTexturePainter painter =
                box.GetComponent<CubeTexturePainter>();

            if (painter == null)
                continue;

            Vector2 uv = GetBoxUV(box, hit);

            painter.PaintAt(uv);

            end = hit.point;
            break;
        }

        laser.SetPosition(0, start);
        laser.SetPosition(1, end);
    }

    Vector2 GetBoxUV(BoxCollider box, RaycastHit hit)
    {
        Vector3 localPoint =
            box.transform.InverseTransformPoint(hit.point)
            - box.center;

        Vector3 localNormal =
            box.transform.InverseTransformDirection(hit.normal);

        Vector3 halfSize = box.size * 0.5f;

        float u = 0f;
        float v = 0f;
        int face = 0;

        if (Mathf.Abs(localNormal.x) > 0.9f)
        {
            u = (localPoint.z / halfSize.z + 1f) * 0.5f;
            v = (localPoint.y / halfSize.y + 1f) * 0.5f;
            face = localNormal.x > 0 ? 0 : 1;
        }
        else if (Mathf.Abs(localNormal.y) > 0.9f)
        {
            u = (localPoint.x / halfSize.x + 1f) * 0.5f;
            v = (localPoint.z / halfSize.z + 1f) * 0.5f;
            face = localNormal.y > 0 ? 2 : 3;
        }
        else
        {
            u = (localPoint.x / halfSize.x + 1f) * 0.5f;
            v = (localPoint.y / halfSize.y + 1f) * 0.5f;
            face = localNormal.z > 0 ? 4 : 5;
        }

        float faceWidth = 1f / 6f;

        return new Vector2(
            face * faceWidth + u * faceWidth,
            v
        );
    }
}
