/*****************************************************************************
// File Name : GridmapEditor.cs
// Author : Brandon Koederitz
// Creation Date : 3/30/2025
// Last Modified : 3/30/2025
//
// Brief Description : 3D tile based system for creating 3D envirobnments from multiple mesh tiles.
*****************************************************************************/
using UnityEditor;
using UnityEngine;

namespace Gridmap.Editor
{
    [CustomEditor(typeof(Gridmap))]
    public class GridmapEditor : UnityEditor.Editor
    {
        /// <summary>
        /// Get SerializedProperty references.
        /// </summary>
        private void OnEnable()
        {
            
        }

        /// <summary>
        /// Draw inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Gridmap gridmap = (Gridmap)target;

            if (GUILayout.Button("Bake All Chunks"))
            {
                gridmap.BakeAllChunks();
            }
        }
    }
}
