using UnityEngine;

public class SmoothMesh : MonoBehaviour
{
    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = new Vector3[vertices.Length];

        // Average normals for smoothing
        for (int i = 0; i < vertices.Length; i++)
        {
            normals[i] = Vector3.zero;
        }
        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            int index0 = mesh.triangles[i];
            int index1 = mesh.triangles[i + 1];
            int index2 = mesh.triangles[i + 2];

            Vector3 normal = Vector3.Cross(vertices[index1] - vertices[index0], vertices[index2] - vertices[index0]);
            normals[index0] += normal;
            normals[index1] += normal;
            normals[index2] += normal;
        }
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i].Normalize();
        }
        mesh.normals = normals;
    }
}
