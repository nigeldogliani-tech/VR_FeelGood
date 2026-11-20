


using UnityEngine;

public class PencilDraw : MonoBehaviour
{
    public float rayDistance = 0.1f;

    private CubeTexturePainter currentPainter;

    private void Update()
    {
        RaycastHit hit;

        // Cast a ray from the pencil tip
        Vector3 direction = -transform.forward;

        if (Physics.Raycast(transform.position, direction, out hit, rayDistance))
        {
            CubeTexturePainter painter =
                hit.collider.GetComponent<CubeTexturePainter>();

            if (painter != null)
            {
                painter.PaintAt(hit.textureCoord);
            }
        }
    }
}
