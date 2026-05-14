/*****************************************************************************
// File Name : MeshChunk.cs
// Author : Lucas Fehlberg
// Creation Date : 12/22/2025
// Last Modified : 3/25/2026
//
// Brief Description : Stores data about the tiles in a certain chunk in the gridmap.
*****************************************************************************/
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gridmap
{
    /// <summary>
    /// The chunks used in the Gridmap Class
    /// </summary>
    public class MeshChunk : MonoBehaviour
    {
        [SerializeField, ShowIfNull] private MeshFilter meshFilter;
        [SerializeField, ShowIfNull] private Gridmap gridmap;
        [SerializeField, ReadOnly] private Vector3Int chunkPosition;
        [SerializeField, ReadOnly] private int tileNum;

        [SerializeField, HideInInspector] private GridTileBase[] tilesInChunk;

        /// <summary>
        /// Size of the chunk
        /// </summary>
        [SerializeField, ReadOnly] private Vector3Int chunkSize;

        [SerializeField, ShowIfNull] private MeshRenderer meshRenderer;


        [SerializeField] private Mesh mesh;

        /// <summary>
        /// Position of the Mesh Chunk
        /// </summary>
        public Vector3Int Position { get => chunkPosition; set => chunkPosition = value; }
        public Mesh Mesh { get => mesh; }

        public GridTileBase[] TilesInChunk { get => tilesInChunk; set => tilesInChunk = value; }

        /// <summary>
        /// Create a new MeshChunk
        /// </summary>
        /// <param name="chunkPos">Position of the Chunk, also the position of index 0</param>
        /// <param name="chunkSize">Size of the chunk, constant for all chunks. All values must be greater than 0</param>
        //public MeshChunk(Vector3Int position, Vector3Int chunkSize)
        //{
        //    this.position = position;
        //    //This doesn't matter but we always refer to X/Z/Y
        //    tilesInChunk = new GridTileBase[chunkSize.x * chunkSize.y * chunkSize.z];

        //    this.chunkSize = chunkSize;
        //}

        internal void Initialize(Gridmap parentMap, Vector3Int chunkPos, Vector3Int chunkSize, MeshFilter mFilter, MeshRenderer mRend)
        {
            gameObject.hideFlags = GridmapUtilities.GRIDMAP_SUB_HIDEFLAGS;
            this.gridmap = parentMap;
            this.meshFilter = mFilter;
            this.meshRenderer = mRend;
            this.chunkPosition = chunkPos;
            //transform.localPosition = GridmapUtilities.GetChunkLocalPosition(chunkPos, chunkSize);
            //This doesn't matter but we always refer to X/Z/Y
            tilesInChunk = new GridTileBase[chunkSize.x * chunkSize.y * chunkSize.z];
            this.chunkSize = chunkSize;
            mesh = MeshHelper.NewGridMesh(name + " Mesh");
            meshFilter.sharedMesh = mesh;
        }

        internal bool IsEmpty()
        {
            return tileNum == 0;
        }

        #region Tiles
        /// <summary>
        /// Gets the tile in this chunk position
        /// </summary>
        /// <param name="pos">The position of the mesh inside the chunk</param>
        /// <returns></returns>
        public GridTileBase GetTile(Vector3Int pos)
        {
            int index = GridmapUtilities.PosToIndex(pos, chunkSize);
            return TilesInChunk[index];
        }

        /// <summary>
        /// Adds a tile to the chunk
        /// </summary>
        /// <param name="tile">The tile to be set</param>
        /// <param name="pos">The position of the tile in gridmap space. (Not relative to the chunk)</param>
        public void SetTile(GridTileBase tile, Vector3Int pos)
        {
            int index = GridmapUtilities.PosToIndex(pos, chunkSize);

            GridTileBase oldTile = TilesInChunk[index];
            TilesInChunk[index] = tile;

            // If we're erasing a tile that exists, decrement tileNum
            if (oldTile != null && tile == null)
            {
                tileNum--;
            }
            // If we're adding a new non-null tile, increment tileNum.
            else if (oldTile == null && tile != null)
            {
                tileNum++;
            }
        }
        #endregion

        /// <summary>
        /// Makes baked updates to this chunk after it is modified.
        /// </summary>
        public void BakeChunk()
        {
            MeshHelper.BakeMesh(mesh, tilesInChunk, new BoundsInt(Vector3Int.zero, chunkSize), gridmap, 
                out List<Material> materials);
            meshRenderer.SetMaterials(materials);
        }
    }
}
