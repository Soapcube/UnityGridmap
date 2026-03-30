/*****************************************************************************
// File Name : MeshChunk.cs
// Author : Lucas Fehlberg
// Creation Date : 12/22/2025
// Last Modified : 3/25/2026
//
// Brief Description : Stores data about the tiles in a certain chunk in the gridmap.
*****************************************************************************/
using NUnit.Framework;
using UnityEngine;

namespace Gridmap
{
    /// <summary>
    /// The chunks used in the Gridmap Class
    /// </summary>
    internal class MeshChunk : MonoBehaviour
    {
        [SerializeField, ReadOnly] private Vector3Int position;
        [SerializeField, HideInInspector] private GridTileBase[] tilesInChunk;

        /// <summary>
        /// Size of the chunk
        /// </summary>
        [SerializeField, ReadOnly] private Vector3Int chunkSize;
        private Mesh mesh;

        /// <summary>
        /// Position of the Mesh Chunk
        /// </summary>
        public Vector3Int Position { get => position; set => position = value; }

        /// <summary>
        /// All the tiles within the chunk
        /// </summary>
        public Mesh Mesh { get => mesh; }
        public GridTileBase[] TilesInChunk { get => tilesInChunk; set => tilesInChunk = value; }

        /// <summary>
        /// Create a new MeshChunk
        /// </summary>
        /// <param name="position">Position of the Chunk, also the position of index 0</param>
        /// <param name="chunkSize">Size of the chunk, constant for all chunks. All values must be greater than 0</param>
        //public MeshChunk(Vector3Int position, Vector3Int chunkSize)
        //{
        //    this.position = position;
        //    //This doesn't matter but we always refer to X/Z/Y
        //    tilesInChunk = new GridTileBase[chunkSize.x * chunkSize.y * chunkSize.z];

        //    this.chunkSize = chunkSize;
        //}

        internal void Initialize(Vector3Int position, Vector3Int chunkSize)
        {
            this.position = position;
            transform.localPosition = position;
            //This doesn't matter but we always refer to X/Z/Y
            tilesInChunk = new GridTileBase[chunkSize.x * chunkSize.y * chunkSize.z];

            this.chunkSize = chunkSize;
            mesh = new Mesh();
        }

        /// <summary>
        /// Gets the tile in this chunk position
        /// </summary>
        /// <param name="pos">The position of the mesh inside the chunk</param>
        /// <returns></returns>
        public GridTileBase GetTile(Vector3Int pos)
        {
            int index = GetTileIndex(pos, chunkSize);
            return TilesInChunk[index];
        }

        /// <summary>
        /// Adds a tile to the chunk
        /// </summary>
        /// <param name="tile">The tile to be set</param>
        /// <param name="pos">The position of the tile in gridmap space. (Not relative to the chunk)</param>
        public void SetTile(GridTileBase tile, Vector3Int pos)
        {
            int index = GetTileIndex(pos, chunkSize);

            // Debug to prove that adding tiles works.
            Debug.Log("Set the tile at position " + pos + " in chunk position " + position + " to the tile  " + tile);
            TilesInChunk[index] = tile;

            ////If we have no loop connections, set some up
            //if (TilesInChunk[index].LoopConnections.Count == 0)
            //{
            //    TilesInChunk[index].SetupLoopConnections();
            //}
        }

        /// <summary>
        /// Gets the relative position of a certain grid position witin a given chunk.
        /// </summary>
        /// <param name="gridPos"></param>
        /// <returns></returns>
        internal static Vector3Int GetChunkRelativePos(Vector3Int pos, Vector3Int chunkSize)
        {
            //Wrap around to our tilemap, so we don't get out of range exceptions
            //This might be a bad idea but we'll see
            pos.x = GridmapUtilities.Mod(pos.x, chunkSize.x);
            pos.y = GridmapUtilities.Mod(pos.y, chunkSize.y);
            pos.z = GridmapUtilities.Mod(pos.z, chunkSize.z);
            return pos;
        }

        /// <summary>
        /// Gets the index of a certain position in a chunk given the chunk's size.
        /// </summary>
        /// <param name="pos">The position within the chunk.</param>
        /// <param name="chunkSize">The size of teh chunk.</param>
        /// <returns>The index of that cell in the chunk.</returns>
        private static int GetTileIndex(Vector3Int pos, Vector3Int chunkSize)
        {
            //Wrap around to our tilemap, so we don't get out of range exceptions
            //This might be a bad idea but we'll see
            pos.x %= chunkSize.x;
            pos.y %= chunkSize.y;
            pos.z %= chunkSize.z;

            int index = pos.x + (pos.y * chunkSize.x) + (pos.z * chunkSize.x * chunkSize.y);

            return index;
        }

        private Vector3 GetPositionFromIndex(int index)
        {
            Vector3 offset = Vector3.zero;
            

            return offset;
        }

        /// <summary>
        /// Bakes the mesh in a simple way
        /// </summary>
        /// <returns>The baked mesh also saves the baked mesh to the MeshChunk's Mesh property</returns>
        public Mesh BakeMesh()
        {
            Mesh masterMesh = new();
            CombineInstance[] instances = new CombineInstance[TilesInChunk.Length];
            for (int i = 0; i < tilesInChunk.Length; i++)
            {
                //Get the mesh and add it to our mesh
                Mesh tileMesh = tilesInChunk[i].GetMesh();
                Vector3 offset = GetPositionFromIndex(i);

                //So much math...This feels inefficient. I'll have to find a better way
                //I found a better way
                //Vector3[] offsetVertices = new Vector3[tileMesh.vertexCount];
                //for(int j = 0; j < tileMesh.vertexCount; j++)
                //{
                //    offsetVertices[j] = tileMesh.vertices[j] + offset;
                //}
                //tileMesh.vertices = offsetVertices;
                instances[i] = new CombineInstance
                {
                    mesh = tileMesh,
                    transform = Matrix4x4.Translate(offset),
                };
            }

            masterMesh.CombineMeshes(instances);
            mesh = masterMesh;

            return masterMesh;
        }
    }
}
