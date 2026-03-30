/*****************************************************************************
// File Name : GridmapBrush.cs
// Author : Brandon Koederitz
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
        [SerializeField] private GridTileBase testTile;

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
            position.z += brushElevation;
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
            position.z += brushElevation;
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
            if (!brushTarget.TryGetComponent(out Gridmap gridmap)) { return; }

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
            if (!brushTarget.TryGetComponent(out Gridmap gridmap)) { return; }

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

        #endregion

        /// <summary>
        /// Gets the currently selected MeshTile from the TilePalette.
        /// </summary>
        /// <returns>The selected MeshTile.</returns>
        private GridTileBase GetMeshTile()
        {
            return testTile;
        }
    }
}
