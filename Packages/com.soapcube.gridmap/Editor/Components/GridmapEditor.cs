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
using UnityEngine.Tilemaps;

namespace Gridmap.Editor
{
    [CustomEditor(typeof(Gridmap))]
    public class GridmapEditor : UnityEditor.Editor
    {
        private SerializedProperty _tileAnchor;
        private SerializedProperty chunks;
        private SerializedProperty tilemap;

        /// <summary>
        /// Get SerializedProperty references.
        /// </summary>
        private void OnEnable()
        {
            _tileAnchor = serializedObject.FindProperty(nameof(_tileAnchor));
            chunks = serializedObject.FindProperty(nameof(chunks));
            tilemap = serializedObject.FindProperty(nameof(tilemap));
        }

        /// <summary>
        /// Draw inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_tileAnchor);
            if (EditorGUI.EndChangeCheck())
            {
                Tilemap tilemapObj = tilemap.objectReferenceValue as Tilemap;
                if (tilemapObj != null)
                {
                    tilemapObj.tileAnchor = _tileAnchor.vector3Value;
                }
            }
            
            EditorGUILayout.PropertyField(tilemap);
            EditorGUILayout.PropertyField(chunks);

            Gridmap gridmap = (Gridmap)target;

            if (GUILayout.Button("Bake All Chunks"))
            {
                gridmap.BakeAllChunks();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
