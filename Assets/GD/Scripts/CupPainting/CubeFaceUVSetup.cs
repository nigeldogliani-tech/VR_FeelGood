

using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class CubeFaceUVSetup : MonoBehaviour
{
    void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;

        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        Vector2[] uv = new Vector2[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 p = vertices[i];
            Vector3 n = normals[i];

            float u = 0f;
            float v = 0f;
            int face = 0;

            if (Mathf.Abs(n.x) > 0.9f)
            {
                u = p.z + 0.5f;
                v = p.y + 0.5f;
                face = n.x > 0 ? 0 : 1;
            }
            else if (Mathf.Abs(n.y) > 0.9f)
            {
                u = p.x + 0.5f;
                v = p.z + 0.5f;
                face = n.y > 0 ? 2 : 3;
            }
            else
            {
                u = p.x + 0.5f;
                v = p.y + 0.5f;
                face = n.z > 0 ? 4 : 5;
            }

            int column = face % 3;
            int row = face / 3;

            uv[i] = new Vector2(
                (column + u) / 3f,
                (row + v) / 2f
            );
        }

        mesh.uv = uv;

        Debug.Log("Cube UVs changed to a 3x2 grid!");
    }
}
