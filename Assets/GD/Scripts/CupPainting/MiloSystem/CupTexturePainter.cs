using System.Collections.Generic;
using UnityEngine;

public class CupTexturePainter : MonoBehaviour
{
    [Header("Texture")]
    [Min(2)] public int textureSize = 1024;

    [Header("Brush")]
    [Min(1)] public int brushSize = 10;
    public Color brushColor = Color.black;

    private static readonly List<CupTexturePainter> ActiveSurfaces = new();
    private static readonly int BaseMap = Shader.PropertyToID("_BaseMap");
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");

    private MeshRenderer surfaceRenderer;
    private Vector3[] vertices;
    private Vector2[] uv;
    private int[][] triangles;
    private Material[] materials;
    private Texture2D[] textures;
    private bool[] dirtyTextures;
    private MaterialPropertyBlock properties;

    internal readonly struct Contact
    {
        public readonly Vector2 UV;
        public readonly int MaterialIndex;

        public Contact(Vector2 uv, int materialIndex)
        {
            UV = uv;
            MaterialIndex = materialIndex;
        }
    }

    private void Awake()
    {
        surfaceRenderer = GetComponent<MeshRenderer>();
        var mesh = GetComponent<MeshFilter>()?.sharedMesh;
        if (surfaceRenderer == null || mesh == null || !mesh.isReadable)
        {
            Debug.LogWarning("Cup painting needs a renderer and a readable mesh on this surface.", this);
            return;
        }

        uv = mesh.uv;
        if (uv.Length != mesh.vertexCount)
        {
            Debug.LogWarning("Cup painting needs UV0 coordinates on this mesh.", this);
            return;
        }

        properties = new MaterialPropertyBlock();
        vertices = mesh.vertices;
        materials = surfaceRenderer.sharedMaterials;
        textures = new Texture2D[materials.Length];
        dirtyTextures = new bool[materials.Length];
        triangles = new int[Mathf.Min(mesh.subMeshCount, materials.Length)][];
        for (int i = 0; i < triangles.Length; i++)
            triangles[i] = mesh.GetTriangles(i);
    }

    private void OnEnable()
    {
        if (triangles != null)
            ActiveSurfaces.Add(this);
    }

    private void OnDisable()
    {
        ActiveSurfaces.Remove(this);
    }

    private void OnDestroy()
    {
        if (textures == null)
            return;

        foreach (var texture in textures)
            Destroy(texture);
    }

    internal static bool FindContact(Ray ray, ref float maxDistance, out CupTexturePainter painter, out Contact contact)
    {
        painter = null;
        contact = default;
        foreach (var surface in ActiveSurfaces)
        {
            if (surface.TryContact(ray, ref maxDistance, out var candidate))
            {
                painter = surface;
                contact = candidate;
            }
        }

        return painter != null;
    }

    internal bool TryContact(Ray ray, ref float maxDistance, out Contact contact)
    {
        contact = default;
        if (!surfaceRenderer.enabled || !surfaceRenderer.bounds.IntersectRay(ray, out float boundsDistance)
            || boundsDistance > maxDistance)
            return false;

        // Keep the local direction unnormalized so distances stay in world units.
        Matrix4x4 worldToLocal = transform.worldToLocalMatrix;
        Vector3 origin = worldToLocal.MultiplyPoint3x4(ray.origin);
        Vector3 direction = worldToLocal.MultiplyVector(ray.direction);
        bool found = false;
        for (int slot = 0; slot < triangles.Length; slot++)
        {
            int[] indices = triangles[slot];
            for (int i = 0; i < indices.Length; i += 3)
            {
                int a = indices[i];
                int b = indices[i + 1];
                int c = indices[i + 2];
                Vector3 edgeAB = vertices[b] - vertices[a];
                Vector3 edgeAC = vertices[c] - vertices[a];
                Vector3 cross = Vector3.Cross(direction, edgeAC);
                float determinant = Vector3.Dot(edgeAB, cross);
                if (Mathf.Abs(determinant) <= 1e-9f)
                    continue;

                float inverse = 1f / determinant;
                Vector3 offset = origin - vertices[a];
                float weightB = Vector3.Dot(offset, cross) * inverse;
                if (weightB < 0f || weightB > 1f)
                    continue;

                Vector3 offsetCross = Vector3.Cross(offset, edgeAB);
                float weightC = Vector3.Dot(direction, offsetCross) * inverse;
                if (weightC < 0f || weightB + weightC > 1f)
                    continue;

                float distance = Vector3.Dot(edgeAC, offsetCross) * inverse;
                if (distance < 0f || distance > maxDistance)
                    continue;

                maxDistance = distance;
                contact = new Contact(uv[a] * (1f - weightB - weightC) + uv[b] * weightB + uv[c] * weightC, slot);
                found = true;
            }
        }

        return found;
    }

    public void PaintAt(Vector2 uv)
    {
        Draw(null, new Contact(uv, 0));
        ApplyStroke();
    }

    public void PaintLine(Vector2 startUV, Vector2 endUV)
    {
        Draw(new Contact(startUV, 0), new Contact(endUV, 0), stopAtUVSeams: false);
        ApplyStroke();
    }

    internal void Draw(Contact? previous, Contact current, bool stopAtUVSeams = true)
    {
        int slot = current.MaterialIndex;
        if (materials == null || slot >= materials.Length || materials[slot] == null)
            return;

        Material material = materials[slot];
        int map = material.HasProperty(BaseMap) ? BaseMap : MainTex;
        if (!material.HasProperty(map))
            return;

        var texture = textures[slot];
        if (texture == null)
            texture = CreateTexture(slot, material, map);

        Vector2 scale = material.GetTextureScale(map);
        Vector2 offset = material.GetTextureOffset(map);
        Vector2Int end = ToPixel(texture, Vector2.Scale(current.UV, scale) + offset);
        if (previous.HasValue && previous.Value.MaterialIndex == slot)
        {
            Vector2Int start = ToPixel(texture, Vector2.Scale(previous.Value.UV, scale) + offset);
            if (start == end)
                return;

            float distance = Vector2.Distance(start, end);
            // Neighboring points on the mesh can be on unrelated sides of a UV seam.
            if (!stopAtUVSeams || distance <= Mathf.Max(brushSize * 4f, texture.width / 32f))
            {
                int steps = Mathf.CeilToInt(distance / Mathf.Max(1f, brushSize * 0.5f));
                for (int i = 0; i < steps; i++)
                    Stamp(texture, Vector2Int.RoundToInt(Vector2.Lerp(start, end, (float)i / steps)));
            }
        }

        Stamp(texture, end);
        dirtyTextures[slot] = true;
    }

    internal void ApplyStroke()
    {
        if (dirtyTextures == null)
            return;

        for (int i = 0; i < dirtyTextures.Length; i++)
        {
            if (!dirtyTextures[i])
                continue;
            textures[i].Apply(false);
            dirtyTextures[i] = false;
        }
    }

    private Texture2D CreateTexture(int slot, Material material, int map)
    {
        int size = Mathf.Max(2, textureSize);
        var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp
        };
        Texture source = material.GetTexture(map);
        if (source != null)
        {
            var previousTarget = RenderTexture.active;
            var target = RenderTexture.GetTemporary(size, size);
            try
            {
                Graphics.Blit(source, target);
                RenderTexture.active = target;
                texture.ReadPixels(new Rect(0, 0, size, size), 0, 0, false);
            }
            finally
            {
                RenderTexture.active = previousTarget;
                RenderTexture.ReleaseTemporary(target);
            }
            texture.wrapModeU = source.wrapModeU;
            texture.wrapModeV = source.wrapModeV;
        }
        else
        {
            var pixels = new Color32[size * size];
            System.Array.Fill(pixels, new Color32(255, 255, 255, 255));
            texture.SetPixels32(pixels);
        }

        textures[slot] = texture;
        dirtyTextures[slot] = true;
        surfaceRenderer.GetPropertyBlock(properties, slot);
        properties.SetTexture(map, texture);
        surfaceRenderer.SetPropertyBlock(properties, slot);
        return texture;
    }

    private static Vector2Int ToPixel(Texture2D texture, Vector2 uv)
    {
        return new Vector2Int(
            Mathf.RoundToInt(WrapUV(uv.x, texture.wrapModeU) * (texture.width - 1)),
            Mathf.RoundToInt(WrapUV(uv.y, texture.wrapModeV) * (texture.height - 1)));
    }

    private static float WrapUV(float value, TextureWrapMode wrap)
    {
        switch (wrap)
        {
            case TextureWrapMode.Repeat: return Mathf.Repeat(value, 1f);
            case TextureWrapMode.Mirror: return Mathf.PingPong(value, 1f);
            case TextureWrapMode.MirrorOnce: return Mathf.Clamp01(Mathf.Abs(value));
            default: return Mathf.Clamp01(value);
        }
    }

    private void Stamp(Texture2D texture, Vector2Int center)
    {
        int radius = Mathf.Max(1, brushSize);
        int minX = Mathf.Max(0, center.x - radius);
        int maxX = Mathf.Min(texture.width - 1, center.x + radius);
        int minY = Mathf.Max(0, center.y - radius);
        int maxY = Mathf.Min(texture.height - 1, center.y + radius);
        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                int dx = x - center.x;
                int dy = y - center.y;
                if (dx * dx + dy * dy <= radius * radius)
                    texture.SetPixel(x, y, brushColor);
            }
        }
    }
}
