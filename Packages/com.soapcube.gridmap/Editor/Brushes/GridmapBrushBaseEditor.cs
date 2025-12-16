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
    [CustomEditor(typeof(GridmapBrush), true)]
    public class GridmapBrushBaseEditor : GridBrushEditorBase
    {
        public GridmapBrush targetBrush { get { return target as GridmapBrush; } }

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
        private SerializedProperty brushElevation;

        /// <summary>
        /// Initialize SerializedProperties here.
        /// </summary>
        protected virtual void OnEnable()
        {
            brushElevation = serializedObject.FindProperty(nameof(brushElevation));
        }

        /// <summary>
        /// Draw the editor.
        /// </summary>
        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            serializedObject.Update();

            EditorGUILayout.PropertyField(brushElevation);

            // Update the position of the tilemap based on the brushElevation.
            
            // Add a space to separate custom data from the default brush properties.
            EditorGUILayout.Space();
            serializedObject.ApplyModifiedProperties();
        }

        #region Elevation Adjusting
        /// <summary>
        /// Adjusts the elevation of the tilemap based on the brush's elevation and the swizzle mode of the grid.
        /// </summary>
        /// <param name="tilemapTarget">The target tilemap gameOjbect to adjust the position of.</param>
        /// <param name="brushElevation">The current elevation set by the brush.</param>
        /// <param name="swizzleMode">The swizzleMode of the grid.  Used to determine which axis to offset the grid by.</param>
        private static void SetTilemapElevation(GameObject tilemapTarget, int brushElevation, Grid.CellSwizzle swizzleMode)
        {
            // Adjust the elevation of the brushTarget tilemap.
            Vector3 pos;
            switch (swizzleMode)
            {
                case GridLayout.CellSwizzle.XYZ:
                case GridLayout.CellSwizzle.YXZ:
                default:
                    pos = Vector3.forward;
                    break;
                case GridLayout.CellSwizzle.XZY:
                case GridLayout.CellSwizzle.YZX:
                    pos = Vector3.up;
                    break;
                case GridLayout.CellSwizzle.ZXY:
                case GridLayout.CellSwizzle.ZYX:
                    pos = Vector3.right;
                    break;
            }
            pos *= brushElevation;
            tilemapTarget.transform.position = pos;
        }

        /// <summary>
        /// On painting the scene GUI, adjust the elevation of the tilemap brushTarget based on the brushElevation.
        /// </summary>
        /// <param name="gridLayout"></param>
        /// <param name="brushTarget"></param>
        /// <param name="position"></param>
        /// <param name="tool"></param>
        /// <param name="executing"></param>
        public override void OnPaintSceneGUI(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, 
            GridBrushBase.Tool tool, bool executing)
        {
            SetTilemapElevation(brushTarget, targetBrush.BrushElevation, gridLayout.cellSwizzle);

            base.OnPaintSceneGUI(gridLayout, brushTarget, position, tool, executing);
        }
        #endregion
    }
}
