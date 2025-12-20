/*****************************************************************************
// File Name : MeshCombiner.cs
// Author : 
// Creation Date : 
// Last Modified : 
//
// Brief Description : 
*****************************************************************************/
using System.Collections.Generic;
using UnityEngine;

public class MeshCombiner : MonoBehaviour
{
    [SerializeField] private MeshFilter[] meshes;

    private void Awake()
    {
        MeshFilter mf = GetComponent<MeshFilter>();

        Mesh targetMesh = new Mesh();

        List<CombineInstance> combine = new List<CombineInstance>();
        for (int i = 0; i < meshes.Length; i++)
        {
            Mesh currentMesh = meshes[i].sharedMesh;
            combine.Add(new CombineInstance() { mesh = currentMesh, transform = meshes[i].transform.localToWorldMatrix });

            for (int j = 0; j < currentMesh.subMeshCount; j++)
            {
                combine.Add(new CombineInstance() { mesh = currentMesh, transform = meshes[i].transform.localToWorldMatrix, subMeshIndex = j});
            }

            meshes[i].gameObject.SetActive(false);
        }

        targetMesh.CombineMeshes(combine.ToArray());
        mf.sharedMesh = targetMesh;
    }
}
