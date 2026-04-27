/*****************************************************************************
// File Name : Gridmap.cs
// Author : Lucas Fehlberg
// Creation Date : 12/14/2025
// Last Modified : 12/28/2025
//
// Brief Description : 3D tile based system for creating 3D envirobnments from multiple mesh tiles.
*****************************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Gridmap
{
    public class Gridmap : MonoBehaviour, IGridmapEditable
    {
        #region CONSTS
        /// <summary>
        /// Size of the chunks
        /// </summary>
        private static readonly Vector3Int chunkSize = new(16, 16, 16);
        #endregion

        [SerializeField, HideInInspector] private ChunkDictionary chunks;

        #region Component References
        [SerializeReference, ShowIfNull] private Tilemap tilemap;
        #endregion

        public void OnCreate(Tilemap tmap)
        {
#if UNITY_EDITOR
            this.tilemap = tmap;
#endif
        }

        #region Tile Management
        /// <summary>
        /// Places a tile at this point, generating a new chunk if there isn't one already
        /// </summary>
        /// <param name="tile">The tile going into the chunk</param>
        /// <param name="point">The point in grid space where the tile is going</param>
        public void PlaceTileAtPoint(GridTileBase tile, Vector3Int point)
        {
            Vector3Int chunkPos = GridmapUtilities.GridToChunkPos(point, chunkSize);

            if (tile == null)
            {
                // If erasing in a null chunk, do nothing.
                if (chunks.ContainsKey(chunkPos))
                {
                    MeshChunk chunk = chunks[chunkPos];
                    chunk.SetTile(tile, GridmapUtilities.GridToChunkRelativePos(point, chunkSize));

                    if (chunk.IsEmpty())
                    {
                        chunks.Remove(chunkPos);
                        DestroyImmediate(chunk.gameObject);
                    }
                }
            }
            else
            {
                //If the chunk is null, we'll simply make a new chunk and add it to the list
                if (!chunks.ContainsKey(chunkPos))
                {
                    CreateNewChunk(this, chunkPos);
                }

                MeshChunk chunk = chunks[chunkPos];
                chunk.SetTile(tile, GridmapUtilities.GridToChunkRelativePos(point, chunkSize));
            }
        }

        /// <summary>
        /// Gets the tile at a given position.
        /// </summary>
        /// <param name="pos">The position to get the tile at.</param>
        /// <returns>The tile at that position.  Null if no tile.</returns>
        public GridTileBase GetTileAtPoint(Vector3Int pos)
        {
            MeshChunk chunk = chunks[GridmapUtilities.GridToChunkPos(pos, chunkSize)];
            if (chunk == null) { return null; }
            return chunk.GetTile(GridmapUtilities.GridToChunkRelativePos(pos, chunkSize));
        }
        #endregion

        #region Chunks
        /// <summary>
        /// Creates a new chunk with a given parent Gridmap
        /// </summary>
        /// <param name="gridmap">The transform of thet GridMap to child this chunk to.</param>
        /// <param name="chunkPosition">The position to spawn the chunk at.</param>
        /// <returns>The created MeshChunk</returns>
        private MeshChunk CreateNewChunk(Gridmap gridmap, Vector3Int chunkPosition)
        {
            // Create the chunk itself.
            GameObject chunkGo = new GameObject($"Chunk ({chunkPosition.x}, {chunkPosition.y}, {chunkPosition.z}) ");
            chunkGo.transform.SetParent(gridmap.transform);
            MeshChunk chunk = chunkGo.AddComponent<MeshChunk>();
            MeshFilter mFilter = chunkGo.AddComponent<MeshFilter>();
            MeshRenderer mRend = chunkGo.AddComponent<MeshRenderer>();
            chunk.Initialize(gridmap, chunkPosition, chunkSize, mFilter, mRend);

            chunks.Add(chunkPosition, chunk);

            return chunk;
        }

        /// <summary>
        /// Rebakes all chunks in this Gridmap.
        /// </summary>
        public void BakeAllChunks()
        {
            foreach (var chunk in chunks.Values)
            {
                if (chunk != null)
                {
                    chunk.BakeChunk();
                }
            }
        }
        #endregion

        #region Position Conversions
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
        /// Gets the position at the center of the grid cell in local space.
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
        #endregion

        /// <summary>
        /// Bakes the tile mesh information 
        /// </summary>
        /// <param name="editedBounds">Not sure if this is necessary, but this will have info on the edited tiles so 
        /// it could be used to know what chunks to bake.</param>
        public void BakeMesh(BoundsInt editedBounds)
        {
            List<Vector3Int> rebakedChunks = new List<Vector3Int>();
            foreach (Vector3Int pos in editedBounds.allPositionsWithin)
            {
                // The position is always in XYZ notation relative to the orientation of the grid.  Convert to the world
                // ortientation.
                Vector3Int swizzPos = GridmapUtilities.ConvertSwizzleSpace(pos, tilemap.layoutGrid.cellSwizzle);

                Vector3Int chunkPosition = GridmapUtilities.GridToChunkPos(swizzPos, chunkSize);
                if (!rebakedChunks.Contains(chunkPosition))
                {
                    rebakedChunks.Add(chunkPosition);
                }

                // Also check for rebaking adjacent chunks later when we implement rule tiles.
            }

            foreach(Vector3Int pos in rebakedChunks)
            {
                if (chunks.ContainsKey(pos))
                {
                    chunks[pos].BakeChunk();
                }
            }
        }

        public GridLayout.CellSwizzle GetSwizzle()
        {
            return tilemap.cellSwizzle;
        }
    }
}
