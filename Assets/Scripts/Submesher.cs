using UnityEngine;
using Gridmap;
using UnityEngine.Rendering;
public class Submesher : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        CombineInstance[] combineInstances = new CombineInstance[mesh.subMeshCount];

        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            Mesh newMesh = MeshHelper.SubmeshToMesh(mesh.GetSubMesh(i), mesh.vertices, mesh.triangles);
            CombineInstance newInstance = new CombineInstance()
            {
                mesh = newMesh,
                transform = Matrix4x4.identity
            };
            //GetComponent<MeshFilter>().mesh = newMesh;
            combineInstances[i] = newInstance;
        }

        mesh.CombineMeshes(combineInstances, false, false, false);
        GetComponent<MeshFilter>().mesh = mesh;
    }
}
