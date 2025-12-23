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
    
    }
}
