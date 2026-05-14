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
        #region Undo Handling

        #endregion
    }
}
