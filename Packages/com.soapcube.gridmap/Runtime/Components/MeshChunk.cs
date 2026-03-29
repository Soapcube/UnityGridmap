/*****************************************************************************
// File Name : MeshChunk.cs
// Author : Lucas Fehlberg
// Creation Date : 12/22/2025
// Last Modified : 12/23/2025
//
// Brief Description : Stores data about the tiles in a certain chunk in the gridmap.
*****************************************************************************/
using UnityEngine;

namespace Gridmap
{
    /// <summary>
    /// The chunks used in the Gridmap Class
    /// </summary>
    internal class MeshChunk
    {
        private Vector3Int position;
        private MeshTileBase[] tilesInChunk;

        /// <summary>
        /// Size of the chunk
        /// </summary>
        private Vector3Int chunkSize;

        /// <summary>
        /// Position of the Mesh Chunk
        /// </summary>
        public Vector3Int Position { get => position; set => position = value; }

        /// <summary>
        /// All the tiles within the chunk
        /// </summary>
        public MeshTileBase[] TilesInChunk { get => tilesInChunk; set => tilesInChunk = value; }

        /// <summary>
        /// Create a new MeshChunk
        /// </summary>
        /// <param name="position">Position of the Chunk, also the position of index 0</param>
        /// <param name="chunkSize">Size of the chunk, constant for all chunks. All values must be greater than 0</param>
        public MeshChunk(Vector3Int position, Vector3Int chunkSize)
        {
            this.position = position;
            //This doesn't matter but we always refer to X/Z/Y
            tilesInChunk = new MeshTileBase[chunkSize.x * chunkSize.y * chunkSize.z];

            this.chunkSize = chunkSize;
        }

        /// <summary>
        /// Gets the tile in this chunk position
        /// </summary>
        /// <param name="pos">The position of the mesh inside the chunk</param>
        /// <returns></returns>
        public MeshTileBase GetTile(Vector3Int pos)
        {
            int index = GetTileIndex(pos, chunkSize);
            return TilesInChunk[index];
        }

        /// <summary>
        /// Adds a tile to the chunk
        /// </summary>
        /// <param name="tile">The tile to be set</param>
        /// <param name="pos">The position of the tile in gridmap space. (Not relative to the chunk)</param>
        public void SetTile(MeshTileBase tile, Vector3Int pos)
        {
            int index = GetTileIndex(pos, chunkSize);

            // Debug to prove that adding tiles works.
            //Debug.Log("Set the tile at position " + pos + " in chunk position " + position + " to the tile  " + tile);
            TilesInChunk[index] = tile;
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
            pos.x = GridmapHelpers.Mod(pos.x, chunkSize.x);
            pos.y = GridmapHelpers.Mod(pos.y, chunkSize.y);
            pos.z = GridmapHelpers.Mod(pos.z, chunkSize.z);
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
            pos = GetChunkRelativePos(pos, chunkSize);
            return pos.x + (pos.y * chunkSize.x) + (pos.z * chunkSize.x * chunkSize.y);
        }
    }
}
