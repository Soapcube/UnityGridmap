/*****************************************************************************
// File Name : GridmapUtilities.cs
// Author : Arcadia Koederitz
// Creation Date : 4/8/2025
// Last Modified : 4/8/2025
//
// Brief Description : Manages GridPalette prefabs and exposes functions for setting and getting specific tiles.
*****************************************************************************/
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Gridmap
{
    public class GridmapPalette : MonoBehaviour, IGridmapEditable
    {
        [SerializeField, ReadOnly] private MeshFilter meshFilter;
        [SerializeField, ReadOnly] private Tilemap tilemap;

        /// <summary>
        /// Initialzies the GridPalette on creation.
        /// </summary>
        /// <param name="meshFilter"></param>
        public void Initialize(MeshFilter meshFilter, Tilemap tilemap)
        {
            this.meshFilter = meshFilter;
            this.tilemap = tilemap;
        }

        public GridTileBase GetTileAtPoint(Vector3Int pos)
        {
            throw new System.NotImplementedException();
        }

        public void PlaceTileAtPoint(GridTileBase tile, Vector3Int point)
        {
            throw new System.NotImplementedException();
        }

        public void BakeMesh(BoundsInt editedBounds)
        {
            throw new System.NotImplementedException();
        }
    }
}
