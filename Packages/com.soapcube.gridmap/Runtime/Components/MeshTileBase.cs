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
        public Dictionary<List<Vector3>, List<int>> LoopConnections { get => loopConnections; }

        private Dictionary<List<Vector3>, List<int>> loopConnections = new();

        /// <summary>
        /// Sets up loop connections between vertices. We can use this to check if any match
        /// </summary>
        internal void SetupLoopConnections()
        {
            //First, we'll get all of the triangles. Since vertices in a triangle are always connected to each other
            //We'll always know that each part of this triangle is a connection
            List <int[]> triangles = new();
            for (int index = 0; index < Mesh.triangles.Length; index++)
            {
                if(index % 3 == 0)
                {
                    triangles.Add(new int[3]);
                }

                triangles[^1][index % 3] = Mesh.triangles[index];
            }

            //We're not all the way there just yet, but now we can set up all of the triangles in the mapping
            foreach (int[] triangle in triangles)
            {
                List<Vector3> vertexPositions = new List<Vector3>()
                {
                    Mesh.vertices[triangle[0]],
                    Mesh.vertices[triangle[1]],
                    Mesh.vertices[triangle[2]],
                };

                loopConnections.Add(vertexPositions, triangle.ToList());
            }

            foreach(Vector3 vertex in mesh.vertices)
            {
                Debug.Log(vertex);
            }
        }
    }
}
