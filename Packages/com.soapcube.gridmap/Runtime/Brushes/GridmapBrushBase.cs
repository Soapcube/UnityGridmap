/*****************************************************************************
// File Name : GridmapBrush.cs
// Author : Brandon Koederitz
// Creation Date : 12/14/2025
// Last Modified : 12/14/2025
//
// Brief Description : Paints meshes on a gridmap.
*****************************************************************************/
using Unity.Plastic.Newtonsoft.Json.Serialization;
using UnityEngine;

namespace Gridmap.Brushes
{
    [CreateAssetMenu(menuName = "Gridmap/Brushes/GridmapBrush")]
    [CustomGridBrush(false, true, false, "Gridmap Brush")]
    public class GridmapBrushBase : GridBrushBase
    {
        //[Header("Gridmap Brush Settings")]

        /// <summary>
        /// Called when the brush paints on a tilemap layer.
        /// </summary>
        /// <param name="gridLayout">The GridLayout component of the tilemap that was painted on.  
        /// This is the partent component of the tilemap.</param>
        /// <param name="brushTarget"> The GameObject that holds the Tilemap that was painted on.</param>
        /// <param name="position">The position that was painted on in XYZ notation regardless of grid orientation. </param>
        public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            base.Paint(gridLayout, brushTarget, position);
            // The position is always in XYZ notation relative to the orientation of the grid.  Convert to world
            // XZY orientation.
            position = GridmapHelpers.ConvertPosition(position, gridLayout.cellSwizzle);
            Debug.Log(position);
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
            base.Erase(gridLayout, brushTarget, position);
            Debug.Log(position);
        }
    }
}
