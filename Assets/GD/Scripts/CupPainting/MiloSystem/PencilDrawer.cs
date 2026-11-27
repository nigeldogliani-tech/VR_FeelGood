using UnityEngine;

public class PencilDrawer : MonoBehaviour
{
    [Header("Drawing")]
    public float drawingDistance = 0.2f;

    [Header("Laser")]
    public LineRenderer laser;

    private Vector2 previousUV;
    private CupTexturePainter previousPainter;

    void Start()
    {
        if (laser != null)
        {
            laser.positionCount = 2;
            laser.useWorldSpace = true;
            laser.startWidth = 0.003f;
            laser.endWidth = 0.003f;
        }
    }

    void Update()
    {
        Vector3 start = transform.position;
        Vector3 direction = transform.up;

        Vector3 end = start + direction * drawingDistance;

        Ray ray = new Ray(start, direction);

        RaycastHit hit;

        bool isDrawing = false;

        if (Physics.Raycast(ray, out hit, drawingDistance))
        {
            // Recherche du système de dessin
            CupTexturePainter painter =
                hit.collider.GetComponent<CupTexturePainter>();

            if (painter == null)
            {
                painter = hit.collider.GetComponentInParent<CupTexturePainter>();
            }

            if (painter != null && hit.collider is MeshCollider)
            {
                // Récupération des UV
                Vector2 currentUV = hit.textureCoord;

                if (painter == previousPainter)
                {
                    painter.PaintLine(previousUV, currentUV);
                }
                else
                {
                    painter.PaintAt(currentUV);
                }

                previousUV = currentUV;
                previousPainter = painter;

                end = hit.point;
                isDrawing = true;
            }
        }

        if (!isDrawing)
        {
            previousPainter = null;
        }

        // Affichage du laser
        if (laser != null)
        {
            laser.SetPosition(0, start);
            laser.SetPosition(1, end);
        }
    }
}