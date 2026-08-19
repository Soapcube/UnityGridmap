/*****************************************************************************
// File Name : GridmapEditorUtility.cs
// Author : Arcadia Koederitz
// Creation Date : 5/13/2025
// Last Modified : 5/13/2025
//
// Brief Description : Centralized utility class for gridmap editor functions used in multiple places.
*****************************************************************************/
using UnityEngine;

namespace Gridmap.Editor
{
    public static class GridmapEditorUtility
    {
        internal static readonly Vector3 HEX_GRID_SIZE = new Vector3(0.8659766f, 1, 1);
        internal static readonly Vector3 RECT_ANCHOR = new Vector3(0.5f, 0.5f, 0.5f);
        internal static readonly Vector3 TOP_DOWN_ROTATION = new Vector3(-90, 0, 0);

        internal enum GridmapCreatePriority
        {
            Rectangular = 3,
            RectangularTopDown,
            Hexagonal,
            HexagonalTopDown
        }
    }
}
