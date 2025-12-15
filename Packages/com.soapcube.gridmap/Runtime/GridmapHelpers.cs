/*****************************************************************************
// File Name : GridmapHelpers.cs
// Author : Brandon Koederitz
// Creation Date : 12/15/2025
// Last Modified : 12/15/2025
//
// Brief Description : Set of static helper functions and core definitions for the Gridmap system.
*****************************************************************************/
using UnityEngine;

namespace Gridmap
{
    public static class GridmapHelpers
    {
        #region CONSTS
        public const Grid.CellSwizzle DEFAULT_GRID_NOTATION = GridLayout.CellSwizzle.XZY;
        #endregion

        /// <summary>
        /// Converts a Vector3 position to the correct axes specified by a grid swizzle.
        /// </summary>
        /// <param name="position">The position in XYZ coordinate space.</param>
        /// <param name="targetSwizzleMode">The swizzle mode to convert the position into.</param>
        /// <param name="baseSwizzleMode">The current swizzle mode of the position.</param>
        /// <returns>The position in the specified swizzle mode.</returns>
        public static Vector3Int ConvertPosition(Vector3Int position, 
            Grid.CellSwizzle baseSwizzleMode = Grid.CellSwizzle.XYZ, 
            Grid.CellSwizzle targetSwizzleMode = DEFAULT_GRID_NOTATION)
        {
            (int, int, int)
            switch (targetSwizzleMode)
            {
                case Grid.CellSwizzle.XYZ:
                    break;
                case Grid.CellSwizzle.XZY:
                    (position.x, position.y, position.z) = (position.x, position.z, position.y);
                    break;
                case Grid.CellSwizzle.YXZ:
                    break;
                case Grid.CellSwizzle.YZX:
                    break;
                case Grid.CellSwizzle.ZXY:
                    break;
                case Grid.CellSwizzle.ZYX:
                    break;
            }
            return position;
        }

        public static 
    }
}
