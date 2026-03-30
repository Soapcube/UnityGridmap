/*****************************************************************************
// File Name : Gridmap.cs
// Author : Lucas Fehlberg
// Creation Date : 12/14/2025
// Last Modified : 12/28/2025
//
// Brief Description : 3D tile based system for creating 3D envirobnments from multiple mesh tiles.
*****************************************************************************/
using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

namespace Gridmap
{
    [RequireComponent(typeof(Tilemap))]
    public class Gridmap : MonoBehaviour
    {
        [SerializeField] private MeshTileBase tile;
        /// <summary>
        /// Size of the chunks
        /// </summary>
        private static readonly Vector3Int chunkSize = new(16, 16, 16);

        /// <summary>
        /// The chunks in this map
        /// </summary>
        [SerializeField] private List<MeshChunk> chunks = new();

        

        private void Start()
        {
            PlaceTileAtPoint(tile, Vector3Int.zero);
            foreach (var chunk in chunks)
            {
                chunk.BakeMesh();
                GetComponent<MeshFilter>().mesh = chunk.Mesh;
            }
        }
        #region Component References
        [SerializeReference, ShowIfNull] private Tilemap tilemap;
        /// <summary>
        /// Get Component references automatically on component reset.
        /// </summary>
        [ContextMenu("Get Component References")]
        private void Reset()
        {
            tilemap = GetComponent<Tilemap>();
        }
        #endregion

        /// <summary>
        /// Places a tile at this point, generating a new chunk if there isn't one already
        /// </summary>
        /// <param name="tile">The tile going into the chunk</param>
        /// <param name="point">The point in grid space where the tile is going</param>
        public void PlaceTileAtPoint(GridTileBase tile, Vector3Int point)
        {
            Vector3Int chunkPosition = GetChunkPos(point);

            MeshChunk chunk = GetChunkByPosition(chunkPosition);
            //If the chunk is null, we'll simply make a new chunk and add it to the list
            if (chunk == null)
            {
                //chunk = new(chunkPosition, chunkSize);
                chunk = CreateNewChunk(transform, chunkPosition);
                chunks.Add(chunk);
            }

            //Add the tile to the chunk
            chunk.SetTile(tile, point - chunkPosition);
        }

        /// <summary>
        /// Gets the tile at a given position.
        /// </summary>
        /// <param name="pos">The position to get the tile at.</param>
        /// <returns>The tile at that position.  Null if no tile.</returns>
        public GridTileBase GetTileAtPoint(Vector3Int pos)
        {
            MeshChunk chunk = GetChunkByPosition(pos);
            if (chunk == null) { return null; }
            return chunk.GetTile(pos);
        }

        #region Chunks
        /// <summary>
        /// Creates a new chunk with a given parent Gridmap
        /// </summary>
        /// <param name="parent">The transform of thet GridMap to child this chunk to.</param>
        /// <param name="chunkPosition">The position to spawn the chunk at.</param>
        /// <returns>The created MeshChunk</returns>
        private static MeshChunk CreateNewChunk(Transform parent, Vector3Int chunkPosition)
        {
            GameObject chunkGo = new GameObject($"Chunk ({chunkPosition.x}, {chunkPosition.y}, {chunkPosition.z}) ");
            chunkGo.transform.SetParent(parent);
            MeshChunk chunk = chunkGo.AddComponent<MeshChunk>();
            MeshFilter mFilter = chunkGo.AddComponent<MeshFilter>();
            MeshRenderer mRend = chunkGo.AddComponent<MeshRenderer>();

            chunk.Initialize(chunkPosition, chunkSize);
            return chunk;
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
        /// Gets the position of the chunk that contains a given grid position.
        /// </summary>
        private Vector3Int GetChunkPos(Vector3Int gridPos)
        {
            Vector3Int chunkPosition = gridPos;
            //Suprisingly, you can get the index of a Vector3
            //This function floors this to the position of Tile0 in a chunk
            for (int index = 0; index < 3; index++)
            {
                float thingToFloor = chunkPosition[index] / (float)chunkSize[index];
                chunkPosition[index] = Mathf.FloorToInt(thingToFloor) * chunkSize[index];
            }
            return chunkPosition;
        }
        #endregion

        #region Position Conversions
        // Need to double check these.

        /// <summary>
        /// Gets the world position of a given cell in the grid.
        /// </summary>
        public Vector3 GridToWorldPosition(Vector3 gridPos)
        {
            //Vector3 worldPos = tilemap.CellToWorld(gridPos);
            Vector3 worldPos = tilemap.LocalToWorld(gridPos);
            // Scale the world position based on cell size and gap.
            for (int i = 0; i < 3; i++)
            {
                worldPos[i] = (worldPos[i] * tilemap.layoutGrid.cellSize[i]) + (worldPos[i] * tilemap.layoutGrid.cellGap[i]);
            }
            return worldPos;
        }

        /// <summary>
        /// Gets the position at the center of the grid cell in grid space.
        /// </summary>
        /// <remarks>
        /// Used for finding the position to center a certain tile's mesh at.
        /// </remarks>
        public Vector3 GridToCenteredPosition(Vector3Int gridPos)
        {
            // gridPos is always the bottom-left-back corner of the cell.
            // Uses the tileAnchor parameter of the tilemap, but we can make a custom parameter if needed.
            Vector3 centeredPosition = gridPos;
            for (int i = 0; i < 3; i++)
            {
                float cellSize = tilemap.layoutGrid.cellSize[i];
                float startPos = gridPos[i] * cellSize;
                centeredPosition[i] = Mathf.LerpUnclamped(startPos, startPos + cellSize, 
                    tilemap.tileAnchor[i]) + (gridPos[i] * tilemap.layoutGrid.cellGap[i]);
            }
            return centeredPosition;
        }

        /// <summary>
        /// Gets the world position of the center of a given cell in the grid.
        /// </summary>
        public Vector3 GridToCenteredWorldPosition(Vector3Int gridPos)
        {
            return GridToWorldPosition(GridToCenteredPosition(gridPos));
        }
        #endregion

        /// <summary>
        /// Bakes the tile mesh information 
        /// </summary>
        /// <param name="editedBounds">Not sure if this is necessary, but this will have info on the edited tiles so 
        /// it could be used to know what chunks to bake.</param>
        public void BakeMesh(BoundsInt editedBounds)
        {
            List<Vector3Int> rebakedChunks = new List<Vector3Int>();
            // Loop through all filled positions
            foreach (Vector3Int pos in editedBounds.allPositionsWithin)
            {
                // The position is always in XYZ notation relative to the orientation of the grid.  Convert to the world
                // ortientation.
                Vector3Int swizzPos = GridmapUtilities.ConvertSwizzleSpace(pos, tilemap.layoutGrid.cellSwizzle);

                Vector3Int chunkPosition = GetChunkPos(swizzPos);
                if (!rebakedChunks.Contains(chunkPosition))
                {
                    rebakedChunks.Add(chunkPosition);
                }

                // Also check for rebaking adjacent chunks later when we implement rule tiles.
            }

            foreach(Vector3Int pos in rebakedChunks)
            {
                //GetChunkByPosition(pos).BakeChunk();
            }
        }
    }
}
