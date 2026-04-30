/*****************************************************************************
// File Name : GridmapUtilities.cs
// Author : Arcadia Koederitz
// Creation Date : 4/8/2025
// Last Modified : 4/8/2025
//
// Brief Description : Manages GridPalette prefabs and exposes functions for setting and getting specific tiles.
Utilizes the tilemap's built-in functionality,as tile palettes are only 2D.
*****************************************************************************/
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Gridmap
{
    public class GridmapPalette : MonoBehaviour, IGridmapEditable
    {
        [SerializeField, ReadOnly] private MeshFilter meshFilter;
        [SerializeField, ReadOnly] private MeshRenderer meshRenderer;
        [SerializeField, ReadOnly] private Tilemap tilemap;
        [SerializeField, ReadOnly] private Mesh mesh;


        #region Properties
        public Mesh Mesh => mesh;
        #endregion

        /// <summary>
        /// Initialzies the GridPalette on creation.
        /// </summary>
        /// <param name="meshFilter"></param>
        public void Initialize(MeshFilter meshFilter, MeshRenderer meshRenderer, Tilemap tilemap, Mesh mesh)
        {
            this.meshFilter = meshFilter;
            this.meshRenderer = meshRenderer;
            this.tilemap = tilemap;
            this.mesh = mesh;
            mesh.MarkDynamic();
        }

        /// <summary>
        /// Gets a tile that occupies a certain cell on the palette.
        /// </summary>
        /// <param name="pos">The cell position to get the tile of.</param>
        /// <returns>The tile at that position.</returns>
        public GridTileBase GetTileAtPoint(Vector3Int pos)
        {
            // Flatten cell pos to 2D
            pos.z = 0;
            return tilemap.GetTile(pos) as GridTileBase;
        }

        /// <summary>
        /// Places a tile at a given position on the GridPalette.
        /// </summary>
        /// <param name="tile">The tile to place.</param>
        /// <param name="cellPos"> the position of the cell to place at.</param>
        public void PlaceTileAtPoint(GridTileBase tile, Vector3Int cellPos)
        {
            // Flatten cell pos to 2D.
            cellPos.z = 0;
            Debug.Log($"Grid tile {tile} was added to GridmapPalette {name} at " +
                $"position {cellPos}");
            tilemap.SetTile(cellPos, tile);
        }

        /// <summary>
        /// Bakes the mesh that renders this TilePalette.
        /// </summary>
        /// <param name="editedBounds">The bounds of the changed tiles.  Unused.</param>
        public void BakeMesh(BoundsInt editedBounds)
        {
            Debug.Log("Baking");
            tilemap.CompressBounds();
            GridTileBase[] gridTiles = tilemap.GetTilesBlock(tilemap.cellBounds).Select(x => x as GridTileBase).ToArray();
            // GridmapPalette will only have 1 mesh, so it just rebakes the mesh.
            //for (int i = 0; i < gridTiles.Length; i++)
            //{
            //    if (gridTiles[i] == null) { continue; }
            //    Vector3Int cellPos = GridmapUtilities.IndexToPos(i, tilemap.cellBounds.size) + 
            //        tilemap.cellBounds.position;
            //    // Bake the mesh here.
            //    Debug.Log($"Tile: {gridTiles[i]}.  Index: {i}. Position: {cellPos}");
            //}

            MeshHelper.BakeMesh(mesh, gridTiles, tilemap.cellBounds, this, out List<Material> materials);
            if (materials == null)
            {
                materials = new List<Material>();
            }
            meshRenderer.SetMaterials(materials);
        }

        public Vector3 GridToCenteredPosition(Vector3Int gridPos)
        {
            Vector3 centeredPosition = gridPos;
            for (int i = 0; i < 3; i++)
            {
                GridLayout grid = tilemap.layoutGrid;
                if (grid == null)
                {
                    grid = transform.parent.GetComponent<GridLayout>();
                }
                float cellSize = grid.cellSize[i];
                float startPos = gridPos[i] * cellSize;
                centeredPosition[i] = Mathf.LerpUnclamped(startPos, startPos + cellSize,
                    tilemap.tileAnchor[i]) + (gridPos[i] * grid.cellGap[i]);
            }
            return centeredPosition;
        }

        public GridLayout.CellSwizzle GetSwizzle()
        {
            return tilemap.cellSwizzle;
        }
    }
}
