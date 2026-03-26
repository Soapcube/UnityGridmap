/*****************************************************************************
// File Name : MeshTileBase.cs
// Author :Lucas Fehlberg
// Creation Date : 12/22/2025
// Last Modified : 3/25/2026
//
// Brief Description : Base class for mesh tiles that contain data about a mesh to paint on the gridmap.
*****************************************************************************/

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gridmap
{
    public abstract class MeshTileBase : ScriptableObject
    {
        [SerializeField] private Mesh mesh;

        /// <summary>
        /// Mesh of the object
        /// </summary>
        public Mesh Mesh { get => mesh; }

        /// <summary>
        /// Get the loop values in order to find duplicates
        /// </summary>
        //public Dictionary<List<Vector3>, List<int>> LoopConnections { get => loopConnections; }

        //private Dictionary<List<Vector3>, List<int>> loopConnections = new();

        //private Dictionary<int, List<int>> vertexConnections = new();

        //I'm not sure if we're gonna do this...maybe for optimization in the future
        ///// <summary>
        ///// Sets up loop connections between vertices. We can use this to check if any match
        ///// </summary>
        //internal void SetupLoopConnections()
        //{
        //    //First, we'll get all of the triangles. Since vertices in a triangle are always connected to each other
        //    //We'll always know that each part of this triangle is a connection
        //    List <int[]> triangles = new();
        //    for (int index = 0; index < Mesh.triangles.Length; index++)
        //    {
        //        if(index % 3 == 0)
        //        {
        //            triangles.Add(new int[3]);
        //        }

        //        triangles[^1][index % 3] = Mesh.triangles[index];
        //    }

        //    for(int i = 0; i < Mesh.vertices.Length; i++)
        //    {
        //        vertexConnections.Add(i, new());
        //        foreach (int[] triangle in triangles)
        //        {
        //            if(!triangle.Contains(i)) continue;

        //            foreach(int vertex in triangle)
        //            {
        //                if(vertex == i) continue;
        //                if(vertexConnections[i].Contains(vertex)) continue;

        //                vertexConnections[i].Add(vertex);
        //            }
        //        }

        //        foreach(int vertex in vertexConnections[i])
        //        {
        //            Debug.Log(i.ToString() + " : " + vertex.ToString());
        //        }
        //        Debug.Log(i.ToString() + " : " + vertexConnections[i].Count.ToString() + " : Count");
        //    }

        //    ////We're not all the way there just yet, but now we can set up all of the triangles in the mapping
        //    //foreach (int[] triangle in triangles)
        //    //{
        //    //    List<Vector3> vertexPositions = new List<Vector3>()
        //    //    {
        //    //        Mesh.vertices[triangle[0]],
        //    //        Mesh.vertices[triangle[1]],
        //    //        Mesh.vertices[triangle[2]],
        //    //    };

        //    //    loopConnections.Add(vertexPositions, triangle.ToList());
        //    //}

            
        //}
        /// <summary>
        /// Gets the mesh to paint on the gridmap, taking into account any vertex/face removal or rules.
        /// </summary>
        /// <returns></returns>
        public abstract Mesh GetMesh();
    }
}
