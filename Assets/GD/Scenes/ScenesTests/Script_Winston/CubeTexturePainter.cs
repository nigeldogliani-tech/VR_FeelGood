
using UnityEngine;

public class CubeTexturePainter : MonoBehaviour
{
    public int textureSize = 1024;
    public int brushSize = 8;
    public Color brushColor = Color.black;

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
        int x = Mathf.RoundToInt(uv.x * textureSize);
        int y = Mathf.RoundToInt(uv.y * textureSize);

        for (int px = -brushSize; px <= brushSize; px++)
        {
            for (int py = -brushSize; py <= brushSize; py++)
            {
                if (px * px + py * py <= brushSize * brushSize)
                {
                    int paintX = x + px;
                    int paintY = y + py;

                    if (paintX >= 0 && paintX < textureSize &&
                        paintY >= 0 && paintY < textureSize)
                    {
                        drawingTexture.SetPixel(
                            paintX,
                            paintY,
                            brushColor
                        );
                    }
                }
            }
        }

        drawingTexture.Apply();
    }
}

