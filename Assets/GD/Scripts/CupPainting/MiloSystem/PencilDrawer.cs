using UnityEngine;

public class PencilDrawer : MonoBehaviour
{
    [Header("Drawing")]
    public float drawingDistance = 0.2f;

    [Header("Laser")]
    public LineRenderer laser;

    private Vector2 previousUV;
    private CupTexturePainter previousPainter;
    private bool wasDrawing = false;

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

            Debug.Log("Objet touché : " + hit.collider.name);
            Debug.Log("Painter trouvé : " + (painter != null));

            if (painter != null)
            {
                Debug.Log("Painter sur : " + painter.gameObject.name);
            }

            if (painter == null)
            {
                painter = hit.collider.GetComponentInChildren<CupTexturePainter>();
            }

            Debug.Log("Painter trouvé : " + (painter != null));

            if (painter != null)
            {
                Debug.Log("Painter situé sur : " + painter.gameObject.name);
            }

            if (painter != null && hit.collider is MeshCollider)
{
                Vector2 currentUV = hit.textureCoord;

                painter.brushColor = Color.red;

                // Si on dessine déjà sur cette tasse
                if (wasDrawing && previousPainter == painter)
                {
                    painter.PaintLine(previousUV, currentUV);
                }
                else
                {
                    painter.PaintAt(currentUV);
                }

                // Sauvegarde de la position
                previousUV = currentUV;
                previousPainter = painter;

                end = hit.point;
                isDrawing = true;
                wasDrawing = true;
            }
        }

        if (!isDrawing)
        {
            previousPainter = null;
            wasDrawing = false;
        }

        // Affichage du laser
        if (laser != null)
        {
            laser.SetPosition(0, start);
            laser.SetPosition(1, end);
        }



    }
}