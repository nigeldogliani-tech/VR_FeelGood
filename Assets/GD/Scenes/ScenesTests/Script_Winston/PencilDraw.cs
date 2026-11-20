

using UnityEngine;

public class PencilDraw : MonoBehaviour
{
    public float lineWidth = 0.01f;
    public Color pencilColor = Color.black;
    public float surfaceOffset = 0.002f;
    public float rayDistance = 0.1f;

    private LineRenderer currentLine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DrawSurface"))
        {
            GameObject newLine = new GameObject("PencilStroke");

            // Attach the drawing to the cube
            newLine.transform.SetParent(other.transform, false);

            newLine.transform.localPosition = Vector3.zero;
            newLine.transform.localRotation = Quaternion.identity;
            newLine.transform.localScale = Vector3.one;

            // Create the Line Renderer
            currentLine = newLine.AddComponent<LineRenderer>();

            currentLine.useWorldSpace = false;
            currentLine.startWidth = lineWidth;
            currentLine.endWidth = lineWidth;
            currentLine.positionCount = 0;

            // Set drawing material and color
            currentLine.material = new Material(
                Shader.Find("Sprites/Default")
            );

            currentLine.startColor = pencilColor;
            currentLine.endColor = pencilColor;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("DrawSurface") && currentLine != null)
        {
            RaycastHit hit;

            // Cast a ray from the pencil tip
            // Assumes the pencil points along its blue Z axis
            Vector3 direction = -transform.forward;

            if (Physics.Raycast(transform.position, direction, out hit, rayDistance))
            {
                if (hit.collider == other)
                {
                    // Find the exact surface position
                    Vector3 surfacePosition =
                        hit.point + hit.normal * surfaceOffset;

                    // Add a new drawing point
                    currentLine.positionCount++;

                    currentLine.SetPosition(
                        currentLine.positionCount - 1,
                        other.transform.InverseTransformPoint(surfacePosition)
                    );
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("DrawSurface"))
        {
            currentLine = null;
        }
    }
}
