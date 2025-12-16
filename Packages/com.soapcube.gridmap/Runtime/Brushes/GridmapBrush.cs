/*****************************************************************************
// File Name : GridmapBrush.cs
// Author : Brandon Koederitz
// Creation Date : 12/14/2025
// Last Modified : 12/14/2025
//
// Brief Description : Paints meshes on a gridmap.
*****************************************************************************/
using UnityEngine;

namespace Gridmap.Brushes
{
    [CreateAssetMenu(menuName = "Gridmap/Brushes/GridmapBrush")]
    [CustomGridBrush(false, true, false, "Gridmap Brush")]
    public class GridmapBrush : GridBrushBase
    {
        #region CONSTS
        private static readonly Vector3Int DEFAULT_CELL_SIZE = Vector3Int.one;
        #endregion

        // These are parameters for a size and pivot point int the default tilemap GridBrush.
        // Unsure what they do, but will reference later if necessary.

        [Header("Gridmap Brush Settings")]
        [SerializeField, Tooltip("Whether this brush should automatically update the position of the tilemap to " +
            "paint on different elevation layers.")] 
        private bool autoUpdateElevation;
        [SerializeField, Tooltip("The elevation of the brush relative to (0, 0, 0).")] 
        private int brushElevation;

        #region Properties
        public bool AutoUpdateElevation => autoUpdateElevation;
        public int BrushElevation => brushElevation;
        #endregion

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
            // Base BoxFill just runs paint on each position in the bounds.  Causes lag.
            //base.BoxFill(gridLayout, brushTarget, position);

            // Get tilemap reference.

            // Loop through all filled positions
            foreach(Vector3Int pos in position.allPositionsWithin)
            {
                // The position is always in XYZ notation relative to the orientation of the grid.  Convert to the world
                // ortientation.
                Vector3Int swizzPos = GridmapHelpers.ConvertSwizzleSpace(pos, gridLayout.cellSwizzle);

                // Actual painting implementation goes here.
                Debug.Log(swizzPos);
            }

            // Bake the mesh after all fills.
        }

        public override void BoxErase(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            // Base BoxErase just runs erase on each position in the bounds.  Causes lag.
            //base.BoxErase(gridLayout, brushTarget, position);

            // Loop through all filled positions
            foreach (Vector3Int pos in position.allPositionsWithin)
            {
                // The position is always in XYZ notation relative to the orientation of the grid.  Convert to the world
                // ortientation.
                Vector3Int swizzPos = GridmapHelpers.ConvertSwizzleSpace(pos, gridLayout.cellSwizzle);

                // Actual painting implementation goes here.
                Debug.Log(swizzPos);
            }

            // Bake the mesh after all changes have been made.
        }

        #endregion
    }
}
