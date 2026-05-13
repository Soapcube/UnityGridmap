/*****************************************************************************
// File Name : CellSwizzleDrawer.cs
// Author : Arcadia Koederitz
// Creation Date : 5/13/2025
// Last Modified : 5/13/2025
//
// Brief Description : Custom PropertyDrawer for the CellSwizzle enum so that child gridmaps can be rebaked if layout
// changes.
*****************************************************************************/
using UnityEditor;
using UnityEngine;

namespace Gridmap.Editor
{
    [CustomPropertyDrawer(typeof(Grid.CellSwizzle))]
    public class CellSwizzleDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Debug.Log(property.serializedObject);
            EditorGUI.BeginChangeCheck();
            base.OnGUI(position, property, label);
            if (EditorGUI.EndChangeCheck())
            {

            }
        }
    }
}
