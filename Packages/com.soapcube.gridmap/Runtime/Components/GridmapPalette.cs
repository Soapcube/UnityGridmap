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

        /// <summary>
        /// Gets a tile that occupies a certain cell on the palette.
        /// </summary>
        /// <param name="pos">The cell position to get the tile of.</param>
        /// <returns>The tile at that position.</returns>
        public GridTileBase GetTileAtPoint(Vector3Int pos)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Places a tile at a given position on the GridPalette.
        /// </summary>
        /// <param name="tile">The tile to place.</param>
        /// <param name="cellPos"> the position of the cell to place at.</param>
        public void PlaceTileAtPoint(GridTileBase tile, Vector3Int cellPos)
        {
            Debug.Log($"Grid tile {tile} was added to GridmapPalette {name} at " +
                $"position {cellPos}");
        }

        /// <summary>
        /// Bakes the mesh that renders this TilePalette.
        /// </summary>
        /// <param name="editedBounds">The bounds of the changed tiles.</param>
        public void BakeMesh(BoundsInt editedBounds)
        {
            // GridmapPalette will only have 1 mesh, so it just rebakes the mesh.
            throw new System.NotImplementedException();
        }
    }
}
