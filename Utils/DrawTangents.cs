using UnityEngine;

[ExecuteInEditMode]
public class DrawTangents : MonoBehaviour
{
    [SerializeField] bool drawTangents = true;
    [SerializeField, Range(0.1f, 10f)] float tangentLength = 1f;
    [SerializeField] bool drawNormals = true;
    [SerializeField, Range(0.1f, 10f)] float normalLength = 1f;
    [SerializeField] bool drawBinormals = true;
    [SerializeField, Range(0.1f, 10f)] float binormalLength = 1f;

    private MeshFilter meshFilter;

    private void OnEnable()
    {
        meshFilter = GetComponent<MeshFilter>();
    }

    private void OnDrawGizmos()
    {
        Mesh mesh = meshFilter.sharedMesh;

        if (mesh == null)
        {
            return;
        }

        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        Vector4[] tangents = mesh.tangents; 

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = meshFilter.transform.TransformPoint(vertices[i]);
            Vector3 normal = meshFilter.transform.TransformDirection(normals[i]);
            Vector3 tangent = meshFilter.transform.TransformDirection(new Vector3(tangents[i].x, tangents[i].y, tangents[i].z));

            Vector3 binormal = Vector3.Cross(normal, tangent) * tangents[i].w;

            if (drawTangents)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(vertex, vertex + tangent * tangentLength);
            }

            if (drawBinormals)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(vertex, vertex + binormal * binormalLength);
            }

            if (drawNormals)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(vertex, vertex + normal * normalLength);
            }
        }
    }
}