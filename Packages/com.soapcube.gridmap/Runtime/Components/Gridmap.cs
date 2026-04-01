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
    [RequireComponent(typeof(Tilemap))]
    public class Gridmap : MonoBehaviour
    {
        #region CONSTS
        /// <summary>
        /// Size of the chunks
        /// </summary>
        private static readonly Vector3Int chunkSize = new(16, 16, 16);
        #endregion

        [SerializeField] private GridTileBase tile;
        /// <summary>
        /// The chunks in this map
        /// </summary>
        [SerializeField] private List<MeshChunk> chunks = new();
        [SerializeField] private MeshChunk[] chunkArray;
        [SerializeField] private BoundsInt chunkArrayBounds = new BoundsInt();

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


        #region Tile Management
        /// <summary>
        /// Places a tile at this point, generating a new chunk if there isn't one already
        /// </summary>
        /// <param name="tile">The tile going into the chunk</param>
        /// <param name="point">The point in grid space where the tile is going</param>
        public void PlaceTileAtPoint(GridTileBase tile, Vector3Int point)
        {
            Vector3Int chunkPosition = GridmapUtilities.GridToChunkPos(point, chunkSize);

            MeshChunk chunk = GetChunkByPosition(chunkPosition);
            // Logic for erasing.
            if (tile == null)
            {
                // If erasing in a null chunk, do nothing.
                if (chunk != null)
                {
                    //Remove the tile from the chunk.
                    chunk.SetTile(tile, GridmapUtilities.GridToChunkRelativePos(point, chunkSize));
                    if (chunk.IsEmpty())
                    {
                        // Delete the chunk.
                        DestroyImmediate(chunk.gameObject);
                        RemoveChunk(chunk);
                    }
                }
            }
            else
            {
                //If the chunk is null, we'll simply make a new chunk and add it to the list
                if (chunk == null)
                {
                    //chunk = new(chunkPosition, chunkSize);
                    chunk = CreateNewChunk(this, chunkPosition);
                }

                //Add the tile to the chunk
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
            MeshChunk chunk = GetChunkByPosition(GridmapUtilities.GridToChunkPos(pos, chunkSize));
            if (chunk == null) { return null; }
            return chunk.GetTile(pos);
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
            chunk.Initialize(gridmap, chunkPosition, chunkSize, mFilter);

            // Add the created chunk to the chunks array, performing resizing if needed.
            //if (!chunkArrayBounds.Contains(chunkPosition))
            //{
            //    // Expand the chunk array's bounds to contain 1 extra layer.
            //    chunkArrayBounds.SetMinMax(chunkArrayBounds.min - Vector3Int.one, chunkArrayBounds.max + Vector3Int.one);
            //    MeshChunk[] newArray = new MeshChunk[chunkArrayBounds.x * chunkArrayBounds.y * chunkArrayBounds.z];
            //    chunkArray.CopyTo(newArray, 0);
            //}
            //// Get the index of the chunk position relative to the min chunk position.
            //int index = GetChunkIndex(chunkPosition, chunkArrayBounds);
            //chunkArray[index] = chunk;
            chunks.Add(chunk);

            return chunk;
        }

        /// <summary>
        /// Removes a chunk from the chunks array, resizing if needed.
        /// </summary>
        /// <param name="chunk"></param>
        internal void RemoveChunk(MeshChunk chunk)
        {
            chunks.Remove(chunk);
        }

        /// <summary>
        /// Gets the chunk at this position
        /// </summary>
        /// <param name="position">The position of the chunk</param>
        /// <returns>The requested MeshChunk</returns>
        private MeshChunk GetChunkByPosition(Vector3Int position)
        {
            //return chunkArray[GetChunkIndex(position, chunkArrayBounds)];
            return chunks.Find(c => c.Position == position);
        }

        /// <summary>
        /// Rebakes all chunks in this Gridmap.
        /// </summary>
        public void BakeAllChunks()
        {
            foreach (var chunk in chunks)
            {
                if (chunk != null)
                {
                    chunk.BakeChunk();
                }
            }
        }

        #region Chunk Array Management
        private void ResizeChunkArray()
        {

        }

        /// <summary>
        /// Gets the layer of the chunkArray that this chunk exists in.
        /// </summary>
        /// <param name="chunkPos"></param>
        /// <returns></returns>
        private static int GetChunkLayer(Vector3Int chunkPos)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Gets the index of a chunk at a given chunk position.
        /// </summary>
        /// <param name="chunkPos">The chunk position to get the index of.</param>
        /// <param name="bounds">The BoundsInt used to define the area where chunks are placed.</param>
        /// <returns>The index of the chunk in the chunkArray.</returns>
        private static int GetChunkIndex(Vector3Int chunkPos, BoundsInt bounds)
        {
            return GridmapUtilities.PosToIndex(chunkPos - bounds.min, bounds.size);
        }
        #endregion

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

                Vector3Int chunkPosition = GridmapUtilities.GridToChunkPos(swizzPos, chunkSize);
                if (!rebakedChunks.Contains(chunkPosition))
                {
                    rebakedChunks.Add(chunkPosition);
                }

                // Also check for rebaking adjacent chunks later when we implement rule tiles.
            }

            foreach(Vector3Int pos in rebakedChunks)
            {
                MeshChunk toBake = GetChunkByPosition(pos);
                if (toBake != null)
                {
                    toBake.BakeChunk();
                }
            }
        }
    }
}
