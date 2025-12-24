using System.Collections.Generic;
using UnityEngine;

namespace Gridmap
{
    public class Gridmap : MonoBehaviour
    {
        [SerializeField] private MeshTileBase tile;
        /// <summary>
        /// Size of the chunks
        /// </summary>
        private readonly Vector3Int chunkSize = new(16, 16, 16);

        /// <summary>
        /// The chunks in this map
        /// </summary>
        private List<MeshChunk> chunks = new();

        private void Start()
        {
            PlaceTileAtPoint(tile, Vector3Int.zero);
            foreach (var chunk in chunks)
            {
                chunk.BakeMesh();
                GetComponent<MeshFilter>().mesh = chunk.Mesh;
            }
        }

        /// <summary>
        /// Places a tile at this point, generating a new chunk if there isn't one already
        /// </summary>
        /// <param name="tile">The tile going into the chunk</param>
        /// <param name="point">The point in grid space where the tile is going</param>
        public void PlaceTileAtPoint(MeshTileBase tile, Vector3Int point)
        {
            Vector3Int chunkPosition = point;

            //Suprisingly, you can get the index of a Vector3
            //This function floors this to the position of Tile0 in a chunk
            for (int index = 0; index < 3; index++)
            {
                float thingToFloor = chunkPosition[index] /= chunkSize[index];
                chunkPosition[index] = Mathf.FloorToInt(thingToFloor) * chunkSize[index];
            }

            MeshChunk chunk = GetChunkByPosition(chunkPosition);
            //If the chunk is null, we'll simply make a new chunk and add it to the list
            if (chunk == null)
            {
                chunk = new(chunkPosition, chunkSize);
                chunks.Add(chunk);
            }

            //Add the tile to the chunk
            chunk.SetTileInChunk(tile, point - chunkPosition);
        }

        /// <summary>
        /// Gets the chunk at this position
        /// </summary>
        /// <param name="position">The position of the chunk</param>
        /// <returns>The requested MeshChunk</returns>
        private MeshChunk GetChunkByPosition(Vector3Int position)
        {
            return chunks.Find(c => c.Position == position);
        }

        /// <summary>
        /// Bakes the tile mesh information 
        /// </summary>
        /// <param name="editedBounds">Not sure if this is necessary, but this will have info on the edited tiles so 
        /// it could be used to know what chunks to bake.</param>
        public void BakeMesh(BoundsInt editedBounds)
        {

        }
    }
}
