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

            Vector3Int swizzPos = GridmapUtilities.ConvertSwizzleSpace(position, gridLayout.cellSwizzle);

            Debug.Log($"Grid Position: {swizzPos}.  World Space Position: {gridmap.GridToWorldPosition(swizzPos)}.  " +
                $"Center Position: {gridmap.GridToCenteredPosition(swizzPos)}");
        }
    }
}
