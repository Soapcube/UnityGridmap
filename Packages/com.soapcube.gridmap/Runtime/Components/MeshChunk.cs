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

        /// <summary>
        /// Adds a tile to the chunk
        /// </summary>
        /// <param name="tile">The tile to be set</param>
        /// <param name="pos">The position of the tile in the chunk</param>
        public void SetTileInChunk(MeshTileBase tile, Vector3Int pos)
        {
            //Wrap around to our tilemap, so we don't get out of range exceptions
            //This might be a bad idea but we'll see
            pos.x %= chunkSize.x;
            pos.y %= chunkSize.y;
            pos.z %= chunkSize.z;

            int index = pos.x + (pos.y * chunkSize.x) + (pos.z * chunkSize.x * chunkSize.y);

            TilesInChunk[index] = tile;
        }
    }
}
