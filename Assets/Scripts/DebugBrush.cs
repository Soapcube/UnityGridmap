/*****************************************************************************
// File Name : DebugBrush.cs
// Author : Brandon Koederitz
// Creation Date : 12/28/2025
// Last Modified : 12/28/2025
//
// Brief Description : Test brush we can use to interface with the gridmap for debugging.
*****************************************************************************/
using UnityEngine;
using UnityEngine.XR;

namespace Gridmap.Brushes
{
    [CreateAssetMenu(menuName = "Gridmap/Brushes/Debug Brush")]
    [CustomGridBrush(false, true, false, "Debug Brush")]
    public class DebugBrush : GridBrushBase
    {
        public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            //base.Paint(gridLayout, brushTarget, position);

            if (brushTarget == null) { return; }
            if (!brushTarget.TryGetComponent(out Gridmap gridmap)) { return; }

            Vector3Int swizzPos = ConvertSwizzleSpace(position, gridLayout.cellSwizzle);

            Debug.Log($"Grid Position: {swizzPos}.  World Space Position: {gridmap.GridToWorldPosition(swizzPos)}.  " +
                $"Center Position: {gridmap.GridToCenteredPosition(swizzPos)}");
        }

        /// <summary>
        /// I Got lazy
        /// </summary>
        /// <param name="position"></param>
        /// <param name="targetSwizzleMode"></param>
        /// <param name="baseSwizzleMode"></param>
        /// <returns></returns>
        private static Vector3Int ConvertSwizzleSpace(Vector3Int position,
            Grid.CellSwizzle targetSwizzleMode,
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
    }
}
