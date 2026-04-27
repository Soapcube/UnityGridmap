/*****************************************************************************
// File Name : MeshHelper.cs
// Author : Lucas Fehlberg
// Creation Date : 4/15/2026
// Last Modified : 4/15/2026
//
// Brief Description : A helper class for odd mesh related things
*****************************************************************************/

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace Gridmap
{
    /// <summary>
    /// Helper class for odd mesh-related things
    /// </summary>
    public static class MeshHelper 
    {
        public static Mesh BakeMesh(GridTileBase[] tilesInChunk, BoundsInt meshBounds, IGridmapEditable gridmap,
            out List<Material> returnedMaterials)
        {
            Mesh bakeTarget = new Mesh();
            BakeMesh(bakeTarget, tilesInChunk, meshBounds, gridmap, out returnedMaterials);
            return bakeTarget;
        }

        /// <summary>
        /// Bakes the mesh in a simple way
        /// </summary>
        /// <returns>The baked mesh also saves the baked mesh to the MeshChunk's Mesh property</returns>
        public static Mesh BakeMesh(Mesh masterMesh, GridTileBase[] tilesInChunk, BoundsInt meshBounds, IGridmapEditable gridmap,
            out List<Material> returnedMaterials)
        {
            masterMesh.Clear();
            Dictionary<Material, List<CombineInstance>> instances = new();
            for (int i = 0; i < tilesInChunk.Length; i++)
            {
                //Get the mesh and add it to our mesh
                if (tilesInChunk[i] == null) { continue; }
                Mesh tileMesh = tilesInChunk[i].GetMesh();
                if (tileMesh == null || tileMesh.vertices.Length == 0)
                {
                    continue;
                }

                Vector3 offset = gridmap.GridToCenteredPosition(GridmapUtilities.IndexToPos(i, meshBounds.size)
                    + meshBounds.position) + tilesInChunk[i].Offset;
                Material[] materials = tilesInChunk[i].GetMaterials();
                foreach (Material material in materials)
                {
                    if (instances.ContainsKey(material)) continue;

                    instances.Add(material, new List<CombineInstance>());
                }
                //if (!instances.ContainsKey(materials[0]))
                //{
                //    instances.Add(materials[0], new());
                //}

                //So much math...This feels inefficient. I'll have to find a better way
                //I found a better way
                //Vector3[] offsetVertices = new Vector3[tileMesh.vertexCount];
                //for(int j = 0; j < tileMesh.vertexCount; j++)
                //{
                //    offsetVertices[j] = tileMesh.vertices[j] + offset;
                //}
                //tileMesh.vertices = offsetVertices;
                if (tileMesh.subMeshCount == 1)
                {
                    CombineInstance newInstance = new()
                    {
                        mesh = tileMesh,
                        transform = Matrix4x4.Translate(offset),
                    };

                    instances[materials[0]].Add(newInstance);
                }
                else
                {
                    for (int j = 0; j < tileMesh.subMeshCount; j++)
                    {
                        Mesh submesh = MeshHelper.SubmeshToMesh(tileMesh.GetSubMesh(j), tileMesh.vertices, tileMesh.triangles);
                        CombineInstance newInstance = new()
                        {
                            mesh = submesh,
                            transform = Matrix4x4.Translate(offset),
                        };

                        instances[materials[j]].Add(newInstance);
                    }
                }

            }
            if (instances.Count == 0)
            {
                returnedMaterials = null;
                return null;
            }
            List<CombineInstance> finalInstance = new();
            foreach (List<CombineInstance> instance in instances.Values)
            {
                Mesh newInstance = new();
                newInstance.CombineMeshes(instance.ToArray(), true);

                CombineInstance nextInstance = new()
                {
                    mesh = newInstance,
                    transform = Matrix4x4.identity,
                };
                finalInstance.Add(nextInstance);
            }
            //masterMesh = finalInstance[0].mesh;
            //masterMesh.CombineMeshes(instances[instances.Keys.First()].ToArray(), true, true);
            masterMesh.CombineMeshes(finalInstance.ToArray(), false);
            //mesh = masterMesh;
            returnedMaterials = instances.Keys.ToList();

            return masterMesh;
        }

        public static Mesh SubmeshToMesh(SubMeshDescriptor submesh, Vector3[] vertices, int[] triangles)
        {
            Mesh mesh = new();
            int firstVertexIndex = submesh.firstVertex;

            List<int> allVerticesIndex = new();
            for(int i = 0; i < submesh.vertexCount; i++)
            {
                allVerticesIndex.Add(i + firstVertexIndex);
            }
            string debugText = "";
            List<int> newTriangles = new();
            foreach (int vertexIndex in triangles)
            {
                if (allVerticesIndex.Contains(vertexIndex))
                {
                    newTriangles.Add(vertexIndex - firstVertexIndex);
                    debugText += vertexIndex.ToString() + " ";
                }
            }
            //Debug.Log(debugText);
            Vector3[] newVertices = vertices[firstVertexIndex..(firstVertexIndex + submesh.vertexCount)];

            mesh.vertices = newVertices;
            mesh.triangles = newTriangles.ToArray();

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            return mesh;
        }
    }
}
