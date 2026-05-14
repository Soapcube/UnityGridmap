/*****************************************************************************
// File Name : DebugBrush.cs
// Author : Brandon Koederitz
// Creation Date : 12/28/2025
// Last Modified : 12/28/2025
//
// Brief Description : Test brush we can use to interface with the gridmap for debugging.
*****************************************************************************/
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.XR;

namespace Gridmap.Brushes
{
    [CreateAssetMenu(menuName = "Testing/Brushes/Debug Brush")]
    [CustomGridBrush(false, true, false, "Debug Brush")]
    public class DebugBrush : GridBrushBase
    {
        public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            //base.Paint(gridLayout, brushTarget, position);

            if (brushTarget == null) { return; }
            if (!brushTarget.TryGetComponent(out Tilemap tilemap)) { return; }

            Vector3Int swizzPos = GridmapUtilities.ConvertSwizzleSpace(position, gridLayout.cellSwizzle);

            Vector3Int chunkSize = new(16, 16, 16);
            Debug.Log($"Grid Position: {swizzPos}.  World Space Position: {tilemap.CellToWorld(GridmapUtilities.ConvertSwizzleSpace(swizzPos, tilemap.cellSwizzle))}.  " +
                $"Chunk Position: {GridmapUtilities.GridToChunkPos(swizzPos, chunkSize)}.  Relative Position: " +
                $"{GridmapUtilities.GridToChunkRelativePos(swizzPos, chunkSize)}. Converted Grid Position: " +
                $"{GridmapUtilities.ChunkToGridPos(GridmapUtilities.GridToChunkRelativePos(swizzPos, chunkSize), GridmapUtilities.GridToChunkPos(swizzPos, chunkSize), chunkSize)}");
            Debug.DrawLine(tilemap.CellToWorld(position), tilemap.CellToWorld(position) + Vector3.up, Color.red, 5);
        }
    }
}
