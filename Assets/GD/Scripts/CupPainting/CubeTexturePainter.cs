

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
        int x = Mathf.Clamp(
            Mathf.RoundToInt(uv.x * textureSize),
            0, textureSize - 1
        );

        int y = Mathf.Clamp(
            Mathf.RoundToInt(uv.y * textureSize),
            0, textureSize - 1
        );

        int column = Mathf.Clamp(Mathf.FloorToInt(uv.x * 3), 0, 2);
        int row = Mathf.Clamp(Mathf.FloorToInt(uv.y * 2), 0, 1);

        int minX = Mathf.FloorToInt(column * textureSize / 3f);
        int maxX = Mathf.CeilToInt((column + 1) * textureSize / 3f) - 1;

        int minY = Mathf.FloorToInt(row * textureSize / 2f);
        int maxY = Mathf.CeilToInt((row + 1) * textureSize / 2f) - 1;

        int radiusX = Mathf.Max(1, Mathf.RoundToInt(brushSize * 2f / 3f));
        int radiusY = brushSize;

        for (int px = -radiusX; px <= radiusX; px++)
        {
            for (int py = -radiusY; py <= radiusY; py++)
            {
                float dx = (float)px / radiusX;
                float dy = (float)py / radiusY;

                if (dx * dx + dy * dy <= 1f)
                {
                    int paintX = x + px;
                    int paintY = y + py;

                    if (paintX >= minX && paintX <= maxX &&
                        paintY >= minY && paintY <= maxY)
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
        float deltaX = (endUV.x - startUV.x) * textureSize * 3f;
        float deltaY = (endUV.y - startUV.y) * textureSize * 2f;

        float distance = Mathf.Sqrt(
            deltaX * deltaX + deltaY * deltaY
        );

        float spacing = Mathf.Max(1f, brushSize * 0.4f);
        int steps = Mathf.CeilToInt(distance / spacing);

        for (int i = 0; i <= steps; i++)
        {
            float t = steps == 0 ? 0f : (float)i / steps;

            Vector2 point = Vector2.Lerp(startUV, endUV, t);

            PaintAt(point);
        }

        drawingTexture.Apply();
    }
}
