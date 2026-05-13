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
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Gridmap
{
    [AddComponentMenu("GameObject/")] // Hides the palette in the AddComponent menu so it cant be used.
    public class GridmapPalette : MonoBehaviour, IGridmapEditable
    {
        [SerializeField, ReadOnly] private MeshFilter meshFilter;
        [SerializeField, ReadOnly] private MeshRenderer meshRenderer;
        [SerializeField, ReadOnly] private Tilemap tilemap;
        [SerializeField, ReadOnly] private Mesh mesh;
        [SerializeField, ReadOnly] private ScriptableObject paletteData;

        #region Properties
        public Mesh Mesh
        { 
            get {  return mesh; }
            set
            {
                mesh = value;
                meshFilter.sharedMesh = value;
            }
        }
        public ScriptableObject PaletteData => paletteData;
        #endregion

        /// <summary>
        /// Initialzies the GridPalette on creation.
        /// </summary>
        /// <param name="meshFilter"></param>
        public void Initialize(MeshFilter meshFilter, MeshRenderer meshRenderer, Tilemap tilemap, Mesh mesh, ScriptableObject paletteData)
        {
            this.meshFilter = meshFilter;
            this.meshRenderer = meshRenderer;
            this.tilemap = tilemap;
            this.mesh = mesh;
            this.paletteData = paletteData;
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
            //Debug.Log($"Grid tile {tile} was added to GridmapPalette {name} at " +
            //    $"position {cellPos}");
            tilemap.SetTile(cellPos, tile);
        }

        /// <summary>
        /// Bakes the mesh that renders this TilePalette.
        /// </summary>
        /// <param name="editedBounds">The bounds of the changed tiles.  Unused.</param>
        public void Bake(BoundsInt editedBounds)
        {
            // When a palette is baked, it creates a new mesh each time that's not saved as an asset.
            // Asset changes are stored on save.
            tilemap.CompressBounds();

            GridTileBase[] gridTiles = tilemap.GetTilesBlock(tilemap.cellBounds).Select(x => x as GridTileBase).ToArray();

            Mesh = MeshHelper.BakeMesh(gridTiles, tilemap.cellBounds, this, out List<Material> materials);
            if (materials == null)
            {
                materials = new List<Material>();
            }
            meshRenderer.SetMaterials(materials);
        }

        public Vector3 GridToCenteredPosition(Vector3Int gridPos)
        {
            switch (tilemap.cellLayout)
            {
                case GridLayout.CellLayout.Hexagon: // Hexagon has the center of the cell set as the position by default.
                    Vector3Int swizzPos = GridmapUtilities.ConvertSwizzleSpace(gridPos, GridLayout.CellSwizzle.XYZ, tilemap.cellSwizzle);
                    //Debug.Log(tilemap.CellToLocal(swizzPos));
                    return tilemap.CellToLocal(swizzPos);
                default:
                    // gridPos is always the bottom-left-back corner of the cell.
                    // Uses the tileAnchor parameter of the tilemap, but we can make a custom parameter if needed.
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
        }

        public GridLayout.CellSwizzle GetSwizzle()
        {
            return tilemap.cellSwizzle;
        }
    }
}
