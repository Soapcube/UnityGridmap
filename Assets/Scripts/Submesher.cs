using UnityEngine;
using Gridmap;
using UnityEngine.Rendering;
public class Submesher : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            MeshHelper.SubmeshToMesh(mesh.GetSubMesh(i), mesh.vertices, mesh.triangles);
        }
    }
}
