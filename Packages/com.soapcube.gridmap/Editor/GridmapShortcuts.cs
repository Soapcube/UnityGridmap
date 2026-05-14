/*****************************************************************************
// File Name : GridmapShortcuts.cs
// Author : Arcadia Koederitz
// Creation Date : 4/12/2025
// Last Modified : 4/12/2025
//
// Brief Description : Handles all gridmap editor shortcuts.
*****************************************************************************/
using Gridmap.Brushes;
using UnityEditor.ShortcutManagement;
using UnityEditor.Tilemaps;
using UnityEngine;

namespace Gridmap.Editor
{
    public static class GridmapShortcuts
    {
        [Shortcut("Gridmap/Gridmap Painter Up", typeof(TilemapEditorTool.ShortcutContext), null, KeyCode.Equals, 
            ShortcutModifiers.Shift)]
        public static void GridmapPainterUp()
        {
            GridmapBrushBase brush = GridPaintingState.gridBrush as GridmapBrushBase;
            if (brush != null)
            {
                brush.BrushElevation++;
            }
            
        }

        [Shortcut("Gridmap/Gridmap Painter Down", typeof(TilemapEditorTool.ShortcutContext), null, KeyCode.Minus, 
            ShortcutModifiers.Shift)]
        public static void GridmapPainterDown()
        {
            GridmapBrushBase brush = GridPaintingState.gridBrush as GridmapBrushBase;
            if (brush != null)
            {
                brush.BrushElevation--;
            }
        }
    }
}
