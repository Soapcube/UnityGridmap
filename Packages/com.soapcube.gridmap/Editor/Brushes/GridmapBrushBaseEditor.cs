/*****************************************************************************
// File Name : GridmapBrushBaseEditor.cs
// Author : Brandon Koederitz
// Creation Date : 12/15/2025
// Last Modified : 12/15/2025
//
// Brief Description : Base editor for the Gridmap brush.
*****************************************************************************/
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Gridmap.Brushes.Editor
{
    [CustomEditor(typeof(GridmapBrushBase), true)]
    public class GridmapBrushBaseEditor : GridBrushEditorBase
    {
        public GridmapBrushBase targetBrush { get { return target as GridmapBrushBase; } }

        /// <summary>
        /// The collection of GameObjects that hold tilemaps that this brush can paint on.  
        /// When we make a Gridmap system, this will include only tiles that are part of a gridmap.
        /// </summary>
        public override GameObject[] validTargets
        {
            get
            {
                // Stage handle is basically an editing context.  This method makes it easier to find components.
                StageHandle currentStageHandle = StageUtility.GetCurrentStageHandle();
                return currentStageHandle.FindComponentsOfType<Tilemap>().Where(x =>
                {
                    GameObject gameObject;
                    return (gameObject = x.gameObject).scene.isLoaded
                           && gameObject.activeInHierarchy
                           && !gameObject.hideFlags.HasFlag(HideFlags.NotEditable);
                           // Only get tilemaps that are childed to a tilemap 3D.
                           //&& gameObject.transform.parent.TryGetComponent(out VoxelTilemap3D tilemap3D);
                }).Select(x => x.gameObject).ToArray();
            }
        }

    
        // SerializedProperty variables.

        /// <summary>
        /// Initialize SerializedProperties here.
        /// </summary>
        protected virtual void OnEnable()
        {

        }

        /// <summary>
        /// Draw the editor.
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}
