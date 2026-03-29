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
        [SerializeField] private MeshTileBase testTile;

        /// <summary>
        /// Gets the currently selected MeshTile from the TilePalette.
        /// </summary>
        /// <returns>The selected MeshTile.</returns>
        protected override MeshTileBase GetMeshTile()
        {
            return testTile;
        }
    }
}
