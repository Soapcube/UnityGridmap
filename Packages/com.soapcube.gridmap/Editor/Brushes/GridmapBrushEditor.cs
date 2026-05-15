/*****************************************************************************
// File Name : GridmapBrushEditor.cs
// Author : Arcadia Koederitz
// Creation Date : 12/15/2025
// Last Modified : 12/15/2025
//
// Brief Description : Base editor for the Gridmap brush.
*****************************************************************************/
using Gridmap.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Gridmap.Brushes.Editor
{
    [CustomEditor(typeof(GridmapBrush), true)]
    public class GridmapBrushEditor : GridmapBrushBaseEditor
    {
        /// <summary>
        /// Registers an undo for modifying a gridmap.
        /// </summary>
        /// <param name="brushTarget"></param>
        /// <param name="tool"></param>
        public override void RegisterUndo(GameObject brushTarget, GridBrushBase.Tool tool)
        {
            if (brushTarget != null)
            {
                IGridmapEditable editable = brushTarget.GetComponentInParent<IGridmapEditable>();
                if (editable == null) { return; }
                {
                    switch (editable)
                    {
                        case Gridmap gridmap:
                            GridmapUndoUtility.RegisterGridmapUndo(gridmap, tool.ToString());
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public override void OnPaintSceneGUI(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, GridBrushBase.Tool tool, bool executing)
        {
            Tilemap tmap = brushTarget.GetComponentInParent<Tilemap>();
        }
    }
}
