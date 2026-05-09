/*****************************************************************************
// File Name : GridmapBrushEditor.cs
// Author : Brandon Koederitz
// Creation Date : 12/15/2025
// Last Modified : 12/15/2025
//
// Brief Description : Base editor for the Gridmap brush.
*****************************************************************************/
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
                if(brushTarget.TryGetComponent(out IGridmapEditable editable))
                {
                    switch(editable)
                    {
                        case GridmapPainter painter:
                            RegisterGridmapUndo(painter.Gridmap, tool.ToString());
                            break;
                        case Gridmap gridmap:
                            RegisterGridmapUndo(gridmap, tool.ToString());
                            break;
                        default:
                            break;
                    }
                }
            }
        }


        private void RegisterGridmapUndo(Gridmap gmap, string undoMessage)
        {
            Undo.RegisterFullObjectHierarchyUndo(gmap.gameObject, undoMessage);
        }
        #region Undo Handling

        #endregion
    }
}
