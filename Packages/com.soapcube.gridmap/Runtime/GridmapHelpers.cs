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
        public static Vector3Int ConvertSwizzleSpace(Vector3Int position, 
            Grid.CellSwizzle targetSwizzleMode = DEFAULT_GRID_NOTATION, 
            Grid.CellSwizzle baseSwizzleMode = Grid.CellSwizzle.XYZ)
        {
            /// Gets the tuple that converts the position from base swizzle spacea into XYZ swizzle space.
            (int, int, int) GetSwizzleTupleBase()
            {
                switch (baseSwizzleMode)
                {
                    case Grid.CellSwizzle.XZY:
                        return (position.x, position.z, position.y);
                    case Grid.CellSwizzle.YXZ:
                        return (position.y, position.x, position.z);
                    // Swaps the condition for the YZX and ZXY swizzles because they need to be inverted to convert from a
                    // given swizzle space to XYZ
                    case Grid.CellSwizzle.YZX:
                        return (position.z, position.x, position.y);
                    case Grid.CellSwizzle.ZXY:
                        return (position.y, position.z, position.x);
                    case Grid.CellSwizzle.ZYX:
                        return (position.z, position.y, position.x);
                    default:
                        return (position.x, position.y, position.z);
                }
            }

            /// Gets the tuple that converts the position from XYZ swizzle space to the target space.
            (int, int, int) GetSwizzleTupleTarget()
            {
                switch (targetSwizzleMode)
                {
                    case Grid.CellSwizzle.XZY:
                        return (position.x, position.z, position.y);
                    case Grid.CellSwizzle.YXZ:
                        return (position.y, position.x, position.z);
                    // This condition gets inverted for GetSwizzleTupleBase
                    case Grid.CellSwizzle.YZX:
                        return (position.y, position.z, position.x);
                    // This condition gets inverted for GetSwizzleTupleBase
                    case Grid.CellSwizzle.ZXY:
                        return (position.z, position.x, position.y);
                    case Grid.CellSwizzle.ZYX:
                        return (position.z, position.y, position.x);
                    default:
                        return (position.x, position.y, position.z);
                }
            }

            (position.x, position.y, position.z) = GetSwizzleTupleBase();
            (position.x, position.y, position.z) = GetSwizzleTupleTarget();

            return position;
        }

        /// <summary>
        /// Returns the canonical modulus of a number to the mod of another number
        /// </summary>
        /// <remarks>
        /// Differs from % in that % gives the remainder, which can be negative.  In this case, negative number loop 
        /// around.
        /// </remarks>
        /// <param name="x">The first neumber</param>
        /// <param name="m">The number to take the mod of.</param>
        /// <returns>The canonical modulus number of X mod M.</returns>
        public static int Mod(int x, int m)
        {
            return ((x % m) + m) % m;
        }

        /// <summary>
        /// Gets the sign of a given number
        /// </summary>
        /// <param name="x">The number to get the sign of.</param>
        /// <returns>-1, 0, or 1, depending ont the sign of the number.</returns>
        public static int GetSign(int x)
        {
            if (x == 0) { return 0; }
            return Mathf.Abs(x) / x;
        }

        /// <summary>
        /// Gets the sign of a given number
        /// </summary>
        /// <param name="x">The number to get the sign of.</param>
        /// <returns>-1, 0, or 1, depending ont the sign of the number.</returns>
        public static int GetSign(float x)
        {
            if (x == 0) { return 0; }
            return (int)(Mathf.Abs(x) / x);
        }
    }
}
