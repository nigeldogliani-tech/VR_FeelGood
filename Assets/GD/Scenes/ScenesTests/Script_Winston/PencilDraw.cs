
using UnityEngine;

public class PencilDraw : MonoBehaviour
{
    public float lineWidth = 0.01f;
    public Color pencilColor = Color.black;

    private LineRenderer currentLine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DrawSurface"))
        {
            GameObject newLine = new GameObject("PencilStroke");

            currentLine = newLine.AddComponent<LineRenderer>();

            currentLine.startWidth = lineWidth;
            currentLine.endWidth = lineWidth;
            currentLine.positionCount = 0;

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
            currentLine.positionCount++;

            currentLine.SetPosition(
                currentLine.positionCount - 1,
                transform.position
            );
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
