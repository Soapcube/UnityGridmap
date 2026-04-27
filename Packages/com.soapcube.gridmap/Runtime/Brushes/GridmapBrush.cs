/*****************************************************************************
// File Name : GridmapBrush.cs
// Author : Arcadia Koederitz
// Creation Date : 12/23/2025
// Last Modified : 12/23/2025
//
// Brief Description : Default mesh brush that paints the selected mseh on the tile palette.
*****************************************************************************/
using UnityEngine;

namespace Gridmap.Brushes
{
    [CreateAssetMenu(menuName = "Gridmap/Brushes/GridmapBrush")]
    [CustomGridBrush(false, true, false, "Gridmap Brush")]
    public class GridmapBrush : GridmapBrushBase
    {
        [SerializeField] private GridTileBase currentTile;

        #region Tools
        /// <summary>
        /// Called when the brush paints on a tilemap layer.
        /// </summary>
        /// <param name="gridLayout">The GridLayout component of the tilemap that was painted on.  
        /// This is the partent component of the tilemap.</param>
        /// <param name="brushTarget"> The GameObject that holds the Tilemap that was painted on.</param>
        /// <param name="position">The position that was painted on in XYZ notation regardless of grid orientation. </param>
        public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            // Offset position based on the brushElevation since it isn't applied automatically.
            position.z += gridZ;
            // Reuse the BoxFill function to paint a singular tile.
            BoundsInt bounds = new BoundsInt(position, DEFAULT_CELL_SIZE);
            BoxFill(gridLayout, brushTarget, bounds);
        }

        /// <summary>
        /// Called when the erase brush paints on a tilemap layer.
        /// </summary>
        /// <param name="gridLayout">The GridLayout component of the tilemap that was painted on.  
        /// This is the partent component of the tilemap.</param>
        /// <param name="brushTarget"> The GameObject that holds the Tilemap that was painted on.</param>
        /// <param name="position">The position that was painted on. </param>
        public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            // Offset position based on the brushElevation since it isn't applied automatically.
            position.z += gridZ;
            // Reuse the BoxErase function to erase a singular tile.
            BoundsInt bounds = new BoundsInt(position, DEFAULT_CELL_SIZE);
            BoxErase(gridLayout, brushTarget, bounds);
        }

        /// <summary>
        /// Called when the BoxFill tool is used, and is also used by the default paint tool to paint one cell.
        /// </summary>
        /// <param name="gridLayout">The GridLayout component of the tilemap that was painted on.  
        /// This is the partent component of the tilemap.</param>
        /// <param name="brushTarget"> The GameObject that holds the Tilemap that was painted on.</param>
        /// <param name="position">The positions painted on by the grid brush.</param>
        public override void BoxFill(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            // Get tilemap reference.  Currently, I'm assuming it goes with the tilemap component.
            if (brushTarget == null) { return; }
            if (!brushTarget.TryGetComponent(out IGridmapEditable gridmap)) { return; }

            // Loop through all filled positions
            foreach (Vector3Int pos in position.allPositionsWithin)
            {
                // The position is always in XYZ notation relative to the orientation of the grid.  Convert to the world
                // ortientation.
                Vector3Int swizzPos = GridmapUtilities.ConvertSwizzleSpace(pos, gridLayout.cellSwizzle);

                // Actual painting implementation goes here.
                gridmap.PlaceTileAtPoint(GetMeshTile(), swizzPos);
            }

            // Bake the mesh after all fills.
            gridmap.BakeMesh(position);
        }

        public override void BoxErase(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            // Get tilemap reference.
            if (brushTarget == null) { return; }
            if (!brushTarget.TryGetComponent(out IGridmapEditable gridmap)) { return; }

            // Loop through all filled positions
            foreach (Vector3Int pos in position.allPositionsWithin)
            {
                // The position is always in XYZ notation relative to the orientation of the grid.  Convert to the world
                // ortientation.
                Vector3Int swizzPos = GridmapUtilities.ConvertSwizzleSpace(pos, gridLayout.cellSwizzle);

                gridmap.PlaceTileAtPoint(null, swizzPos);
            }

            // Bake the mesh after all changes have been made.
            gridmap.BakeMesh(position);
        }

        /// <summary>
        /// Picks a tile from a selected gridmap, given tile coordiantes.
        /// </summary>
        /// <param name="gridLayout">Grid to pick data from.</param>
        /// <param name="brushTarget">The gameobject that was picked from.</param>
        /// <param name="position">The position of the picked cell.</param>
        /// <param name="pivot">The pivot of the picking brush,</param>
        public override void Pick(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, Vector3Int pivot)
        {
            // Offset position based on the brushElevation since it isn't applied automatically.
            position.z += gridZ;
            if (brushTarget == null) { return; }
            if (!brushTarget.TryGetComponent(out IGridmapEditable gridmap)) { return; }

            foreach(Vector3Int pos in position.allPositionsWithin)
            {
                Vector3Int swizzPos = GridmapUtilities.ConvertSwizzleSpace(pos, gridLayout.cellSwizzle);
                // Add support for multiple cell picking in the future.
                PickCell(gridmap, swizzPos);
            }
        }

        /// <summary>
        /// Picks a cell on the GridMap and sets this brush's selected tile.
        /// </summary>
        /// <param name="gridmap">The gridmap to pick from.</param>
        /// <param name="pos">The position on the gridmap to pick.</param>
        private void PickCell(IGridmapEditable gridmap, Vector3Int pos)
        {
            currentTile = gridmap.GetTileAtPoint(pos);
        }
        #endregion

        /// <summary>
        /// Gets the currently selected MeshTile from the TilePalette.
        /// </summary>
        /// <returns>The selected MeshTile.</returns>
        private GridTileBase GetMeshTile()
        {
            return currentTile;
        }
    }
}
