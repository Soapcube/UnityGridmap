/*****************************************************************************
// File Name : MeshDebugger.cs
// Author : 
// Creation Date : 
// Last Modified : 
//
// Brief Description : Debug Script
*****************************************************************************/
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshDebugger : MonoBehaviour
{
    [ContextMenu("Debug Mesh")]
    private void Start()
    {
        MeshFilter filt = GetComponent<MeshFilter>();
        Mesh mesh = filt.sharedMesh;

        LogCollection(mesh.vertices, "Verticies");
        LogCollection(mesh.normals, "Normals");
        //LogCollection(mesh.uv, "UV");
        //LogCollection(mesh.triangles, "Tris");

        // Debug the Mesh

        LogNormals(mesh.vertices, mesh.normals);
    }

    private void LogNormals(Vector3[] verts, Vector3[] normals)
    {
        for(int i = 0; i < verts.Length; ++i)
        {
            Debug.DrawRay(transform.position + verts[i], normals[i], Color.red, 10);
        }
    }

    /// <summary>
    /// Prints a collection of values to the console.
    /// </summary>
    /// <typeparam name="T">The type of the collection.</typeparam>
    /// <param name="collection">The collection to print.</param>
    /// <param name="collectionName">A specific name that can be used to identify the collection.</param>
    public static void LogCollection<T>(IEnumerable<T> collection, string collectionName = "")
    {
        T[] array = collection.ToArray();
        if (collectionName == "")
        {
            collectionName = collection.ToString();
        }
        for (int i = 0; i < array.Length; i++)
        {
            Debug.Log($"Collection {collectionName} contains item {array[i]} at index {i}.");
        }
    }
}
