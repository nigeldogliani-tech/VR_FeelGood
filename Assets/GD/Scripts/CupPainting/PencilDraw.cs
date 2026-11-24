


using UnityEngine;

public class PencilDraw : MonoBehaviour
{
    public float rayDistance = 0.2f;

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
                Debug.Log("Hit: " + hit.collider.name);
                Debug.Log("UV: " + hit.textureCoord);

                Debug.Log("UV coordinates: " + hit.textureCoord);
                painter.PaintAt(hit.textureCoord);
            }
        }
    }
}
