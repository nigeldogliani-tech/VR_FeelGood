

using UnityEngine;

public class PencilLaser : MonoBehaviour
{
    public float laserDistance = 0.2f;

    [Header("Pencil")]
    public Renderer pencilTip;
    public CubeTexturePainter cubePainter;

    private LineRenderer laser;
    private Vector2 previousUV;
    private CubeTexturePainter previousPainter;

    private Color currentColor = Color.black;

    void Start()
    {
        laser = GetComponent<LineRenderer>();

        laser.positionCount = 2;
        laser.useWorldSpace = true;
        laser.startWidth = 0.003f;
        laser.endWidth = 0.003f;

        UpdatePencilColor();
    }

    void Update()
    {
        Vector3 start = transform.position;
        Vector3 direction = transform.up;

        Vector3 end = start + direction * laserDistance;

        bool hitCube = false;

        RaycastHit[] hits = Physics.RaycastAll(
            start, direction, laserDistance
        );

        foreach (RaycastHit hit in hits)
        {
            // Detect a palette sphere
            PaletteColor palette = hit.collider.GetComponent<PaletteColor>();

            if (palette != null)
            {
                currentColor = palette.color;
                UpdatePencilColor();
                previousPainter = null;
                continue;
            }

            // Detect the painting cube
            BoxCollider box = hit.collider as BoxCollider;

            if (box == null)
                continue;

            CubeTexturePainter painter =
                box.GetComponent<CubeTexturePainter>();

            if (painter == null)
                continue;

            // Apply selected colour
            painter.brushColor = currentColor;

            Vector2 uv = GetBoxUV(box, hit);

            if (previousPainter == painter)
            {
                painter.PaintLine(previousUV, uv);
            }
            else
            {
                painter.PaintAt(uv);
            }

            previousUV = uv;
            previousPainter = painter;

            end = hit.point;
            hitCube = true;

            break;
        }

        if (!hitCube)
        {
            previousPainter = null;
        }

        laser.SetPosition(0, start);
        laser.SetPosition(1, end);
    }

    void UpdatePencilColor()
    {
        if (pencilTip != null)
        {
            pencilTip.material.color = currentColor;
        }

        if (laser != null)
        {
            laser.startColor = currentColor;
            laser.endColor = currentColor;
        }
    }

    Vector2 GetBoxUV(BoxCollider box, RaycastHit hit)
    {
        Vector3 localPoint =
            box.transform.InverseTransformPoint(hit.point)
            - box.center;

        Vector3 localNormal =
            box.transform.InverseTransformDirection(hit.normal);

        Vector3 halfSize = box.size * 0.5f;

        float u;
        float v;
        int face;

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

        int column = face % 3;
        int row = face / 3;

        return new Vector2(
            (column + u) / 3f,
            (row + v) / 2f
        );
    }
}
