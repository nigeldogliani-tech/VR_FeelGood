
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

        // Create a new white texture
        drawingTexture = new Texture2D(
            textureSize,
            textureSize,
            TextureFormat.RGBA32,
            false
        );

        drawingTexture.filterMode = FilterMode.Bilinear;
        drawingTexture.wrapMode = TextureWrapMode.Clamp;

        // Fill the texture with white pixels
        Color[] pixels = new Color[textureSize * textureSize];

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }

        drawingTexture.SetPixels(pixels);
        drawingTexture.Apply();

        // Give the texture to the cube's material
        cubeRenderer.material.mainTexture = drawingTexture;
    }

    
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

    // Identify which face is being painted
    int column = Mathf.Clamp(Mathf.FloorToInt(uv.x * 3), 0, 2);
    int row = Mathf.Clamp(Mathf.FloorToInt(uv.y * 2), 0, 1);

    int minX = Mathf.FloorToInt(column * textureSize / 3f);
    int maxX = Mathf.CeilToInt((column + 1) * textureSize / 3f) - 1;

    int minY = Mathf.FloorToInt(row * textureSize / 2f);
    int maxY = Mathf.CeilToInt((row + 1) * textureSize / 2f) - 1;

    // Compensate for the rectangular texture cells
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

    drawingTexture.Apply();
}
}