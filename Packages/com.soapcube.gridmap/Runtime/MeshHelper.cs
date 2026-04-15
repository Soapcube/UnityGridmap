/*****************************************************************************
// File Name : MeshHelper.cs
// Author : Lucas Fehlberg
// Creation Date : 4/15/2026
// Last Modified : 4/15/2026
//
// Brief Description : A helper class for odd mesh related things
*****************************************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Gridmap
{
    /// <summary>
    /// Helper class for odd mesh-related things
    /// </summary>
    public static class MeshHelper 
    {
        public static Mesh SubmeshToMesh(SubMeshDescriptor submesh, Vector3[] vertices, int[] triangles)
        {
            Mesh mesh = new Mesh();
            int firstVertexIndex = submesh.firstVertex;

            List<int> allVerticesIndex = new();
            for(int i = 0; i < submesh.vertexCount; i++)
            {
                allVerticesIndex.Add(i + firstVertexIndex);
            }
            string debugText = "";
            foreach (int vertexIndex in allVerticesIndex)
            {
                debugText += vertexIndex.ToString() + " ";
            }
            Debug.Log(debugText);
            
            return new Mesh();
        }
    }
}
