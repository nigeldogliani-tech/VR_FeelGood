
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
            newLine.transform.SetParent(other.transform, false);
            newLine.transform.localPosition = Vector3.zero;
            newLine.transform.localRotation = Quaternion.identity;
            newLine.transform.localScale = Vector3.one;

            currentLine = newLine.AddComponent<LineRenderer>();
            currentLine.useWorldSpace = false;

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
    other.transform.InverseTransformPoint(transform.position)
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
