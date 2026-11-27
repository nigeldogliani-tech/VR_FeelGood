using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class CupTexturePainter : MonoBehaviour
{
    [Header("Texture")]
    public int textureSize = 1024;

    [Header("Brush")]
    public int brushSize = 10;
    public Color brushColor = Color.black;

    private Texture2D drawingTexture;
    private Renderer cupRenderer;

    void Start()
    {
        cupRenderer = GetComponentInChildren<Renderer>();
        if (cupRenderer == null)
        {
            Debug.LogError("Aucun Renderer trouvé sur " + gameObject.name);
            return;
        }

        drawingTexture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
        drawingTexture.filterMode = FilterMode.Bilinear;
        drawingTexture.wrapMode = TextureWrapMode.Clamp;

        Color[] pixels = new Color[textureSize * textureSize];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.white;
        drawingTexture.SetPixels(pixels);
        drawingTexture.Apply();

        foreach (Material mat in cupRenderer.materials)
        {
            if (mat.HasProperty("_BaseMap")) mat.SetTexture("_BaseMap", drawingTexture);
            if (mat.HasProperty("_MainTex")) mat.SetTexture("_MainTex", drawingTexture);
        }
    }

    public void PaintAt(Vector2 uv)
    {
        if (drawingTexture == null)
            return;

        int centerX = Mathf.RoundToInt(uv.x * textureSize);
        int centerY = Mathf.RoundToInt(uv.y * textureSize);

        int radius = brushSize;

        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                if (x * x + y * y > radius * radius)
                    continue;

                int pixelX = centerX + x;
                int pixelY = centerY + y;

                if (pixelX < 0 || pixelX >= textureSize ||
                    pixelY < 0 || pixelY >= textureSize)
                    continue;

                drawingTexture.SetPixel(pixelX, pixelY, brushColor);
            }
        }

        drawingTexture.Apply();
    }

    public void PaintLine(Vector2 startUV, Vector2 endUV)
    {
        if (drawingTexture == null)
            return;

        float distance = Vector2.Distance(
            startUV * textureSize,
            endUV * textureSize
        );

        int steps = Mathf.Max(1, Mathf.CeilToInt(distance / (brushSize * 0.5f)));

        for (int i = 0; i <= steps; i++)
        {
            float t = (float)i / steps;
            Vector2 uv = Vector2.Lerp(startUV, endUV, t);

            PaintAtWithoutApply(uv);
        }

        drawingTexture.Apply();
    }

    private void PaintAtWithoutApply(Vector2 uv)
    {
        int centerX = Mathf.RoundToInt(uv.x * textureSize);
        int centerY = Mathf.RoundToInt(uv.y * textureSize);

        for (int x = -brushSize; x <= brushSize; x++)
        {
            for (int y = -brushSize; y <= brushSize; y++)
            {
                if (x * x + y * y > brushSize * brushSize)
                    continue;

                int px = centerX + x;
                int py = centerY + y;

                if (px < 0 || px >= textureSize ||
                    py < 0 || py >= textureSize)
                    continue;

                drawingTexture.SetPixel(px, py, brushColor);
            }
        }
    }
}
