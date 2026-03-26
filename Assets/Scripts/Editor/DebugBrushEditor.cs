/*****************************************************************************
// File Name : DebugBrushEditor.cs
// Author : Brandon Koederitz
// Creation Date : 12/28/2025
// Last Modified : 12/28/2025
//
// Brief Description : Editor for the debug brush to make tilemaps selectable.
*****************************************************************************/
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Gridmap.Brushes.Editor
{
    [CustomEditor(typeof(DebugBrush), true)]
    public class DebugBrushEditor : GridBrushEditorBase
    {
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
                           && !gameObject.hideFlags.HasFlag(HideFlags.NotEditable)
                           && gameObject.TryGetComponent(out Gridmap gmap);
                }).Select(x => x.gameObject).ToArray();
            }
        }
    }
}
