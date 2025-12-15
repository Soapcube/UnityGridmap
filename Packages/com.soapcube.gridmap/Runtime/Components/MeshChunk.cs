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

        public MeshChunk(Vector3Int position, Vector3Int chunkSize)
        {
            this.position = position;
            //This doesn't matter but we always refer to X/Z/Y
            tilesInChunk = new MeshTileBase[chunkSize.x * chunkSize.y * chunkSize.z];

            this.chunkSize = chunkSize;
        }

        public MeshTileBase GetTileAtPosition(Vector3Int pos)
        {
            //Wrap around to our tilemap, so we don't get out of range exceptions
            //This might be a bad idea but we'll see
            pos.x %= chunkSize.x;
            pos.y %= chunkSize.y;
            pos.z %= chunkSize.z;

            int index = pos.x + (pos.y * chunkSize.x) + (pos.z * chunkSize.x * chunkSize.y);

            return TilesInChunk[index];
        }
    }
}
