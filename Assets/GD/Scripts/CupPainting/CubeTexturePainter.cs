

using UnityEngine;

public class CubeTexturePainter : MonoBehaviour
{
    public int textureSize = 1024;
    public int brushSize = 5;
    public Color brushColor = new Color(0.05f, 0.05f, 0.05f, 1f);

    private Texture2D drawingTexture;
    private Renderer cubeRenderer;

    private void Start()
    {
        cubeRenderer = GetComponent<Renderer>();

        drawingTexture = new Texture2D(
            textureSize,
            textureSize,
            TextureFormat.RGBA32,
            false
        );

        drawingTexture.filterMode = FilterMode.Bilinear;
        drawingTexture.wrapMode = TextureWrapMode.Clamp;

        Color[] pixels = new Color[textureSize * textureSize];

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }

        drawingTexture.SetPixels(pixels);
        drawingTexture.Apply();

        cubeRenderer.material.mainTexture = drawingTexture;
    }

    // Draw one ink dot
    public void PaintAt(Vector2 uv)
    {
        PaintDot(uv);
        drawingTexture.Apply();
    }

    private void PaintDot(Vector2 uv)
    {
        int x = Mathf.Clamp(
            Mathf.RoundToInt(uv.x * (textureSize - 1)),
            0, textureSize - 1
        );

        int y = Mathf.Clamp(
            Mathf.RoundToInt(uv.y * (textureSize - 1)),
            0, textureSize - 1
        );

        for (int px = -brushSize; px <= brushSize; px++)
        {
            for (int py = -brushSize; py <= brushSize; py++)
            {
                float dx = (float)px / brushSize;
                float dy = (float)py / brushSize;

                if (dx * dx + dy * dy <= 1f)
                {
                    int paintX = x + px;
                    int paintY = y + py;

                    if (paintX >= 0 && paintX < textureSize &&
                        paintY >= 0 && paintY < textureSize)
                    {
                        drawingTexture.SetPixel(
                            paintX, paintY, brushColor
                        );
                    }
                }
            }
        }
    }

    // Fill the gaps between two drawing positions
    public void PaintLine(Vector2 startUV, Vector2 endUV)
    {
        float deltaX = (endUV.x - startUV.x) * textureSize;
        float deltaY = (endUV.y - startUV.y) * textureSize;

        float distance = Mathf.Sqrt(
            deltaX * deltaX + deltaY * deltaY
        );

        float spacing = Mathf.Max(1f, brushSize * 0.4f);
        int steps = Mathf.CeilToInt(distance / spacing);

        for (int i = 0; i <= steps; i++)
        {
            float t = steps == 0 ? 0f : (float)i / steps;

            Vector2 point = Vector2.Lerp(startUV, endUV, t);

            PaintDot(point);
        }

        drawingTexture.Apply();
    }
}

