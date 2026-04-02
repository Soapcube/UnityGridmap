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
        [SerializeField, ReadOnly] private Vector3Int position;
        [SerializeField, ReadOnly] private int tileNum;

        [SerializeField, HideInInspector] private GridTileBase[] tilesInChunk;

        /// <summary>
        /// Size of the chunk
        /// </summary>
        [SerializeField, ReadOnly] private Vector3Int chunkSize;

        [SerializeField, ShowIfNull] private MeshFilter meshFilter;
        [SerializeField, ShowIfNull] private MeshRenderer meshRenderer;


        [SerializeField, ShowIfNull] private Gridmap gridmap;
        private Mesh mesh;

        /// <summary>
        /// Position of the Mesh Chunk
        /// </summary>
        public Vector3Int Position { get => position; set => position = value; }
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

        internal void Initialize(Gridmap parentMap, Vector3Int position, Vector3Int chunkSize, MeshFilter mFilter, MeshRenderer mRend)
        {
            gameObject.hideFlags = HideFlags.NotEditable;
            this.gridmap = parentMap;
            this.meshFilter = mFilter;
            this.meshRenderer = mRend;
            this.position = position;
            transform.localPosition = GridmapUtilities.GetChunkLocalPosition(position, chunkSize);
            //This doesn't matter but we always refer to X/Z/Y
            tilesInChunk = new GridTileBase[chunkSize.x * chunkSize.y * chunkSize.z];

            this.chunkSize = chunkSize;
            mesh = new Mesh();
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
            Mesh myMesh = BakeMesh();
            meshFilter.sharedMesh = BakeMesh();
        }

        /// <summary>
        /// Bakes the mesh in a simple way
        /// </summary>
        /// <returns>The baked mesh also saves the baked mesh to the MeshChunk's Mesh property</returns>
        public Mesh BakeMesh()
        {
            Mesh masterMesh = new();
            Dictionary<Material, List<CombineInstance>> instances = new();
            for (int i = 0; i < tilesInChunk.Length; i++)
            {
                //Get the mesh and add it to our mesh
                if (tilesInChunk[i] == null) { continue; }
                Mesh tileMesh = tilesInChunk[i].GetMesh();
                if (tileMesh == null || tileMesh.vertices.Length == 0)
                {
                    continue;
                }

                Vector3 offset = gridmap.GridToCenteredPosition(GridmapUtilities.IndexToPos(i, chunkSize)) 
                    + tilesInChunk[i].Offset;
                Material[] materials = tilesInChunk[i].GetMaterials();
                if (!instances.ContainsKey(materials[0]))
                {
                    instances.Add(materials[0], new());
                }

                //So much math...This feels inefficient. I'll have to find a better way
                //I found a better way
                //Vector3[] offsetVertices = new Vector3[tileMesh.vertexCount];
                //for(int j = 0; j < tileMesh.vertexCount; j++)
                //{
                //    offsetVertices[j] = tileMesh.vertices[j] + offset;
                //}
                //tileMesh.vertices = offsetVertices;
                CombineInstance newInstance = new()
                {
                    mesh = tileMesh,
                    transform = Matrix4x4.Translate(offset),
                };

                instances[materials[0]].Add(newInstance);
            }
            if(instances.Count == 0)
            {
                return null;
            }
            List<CombineInstance> finalInstance = new();
            foreach (List<CombineInstance> instance in instances.Values)
            {
                Mesh newInstance = new();
                newInstance.CombineMeshes(instance.ToArray(), true);

                CombineInstance nextInstance = new()
                {
                    mesh = newInstance,
                    transform = Matrix4x4.identity,
                };
                finalInstance.Add(nextInstance);
            }
            //masterMesh = finalInstance[0].mesh;
            //masterMesh.CombineMeshes(instances[instances.Keys.First()].ToArray(), true, true);
            masterMesh.CombineMeshes(finalInstance.ToArray(), false);
            mesh = masterMesh;
            meshRenderer.SetMaterials(instances.Keys.ToList());

            return masterMesh;
        }
    }
}
