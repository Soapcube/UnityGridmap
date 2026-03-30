/*****************************************************************************
// File Name : ShowIfNullDrawer.cs
// Author : Brandon Koederitz
// Creation Date : 3/29/2025
// Last Modified : 3/29/2025
//
// Brief Description : Hides a property field if it is a non-null object reference.
*****************************************************************************/
using UnityEditor;
using UnityEngine;

namespace Gridmap.Editor
{
    [CustomPropertyDrawer(typeof(ShowIfNullAttribute))]
    public class ShowIfNullDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Only show the property if it's not an object reference or it's value is null.
            if (property.propertyType != SerializedPropertyType.ObjectReference || 
                property.objectReferenceValue == null)
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }

        /// <summary>
        /// Give a property height of 0 if the property is hidden due to not being null.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.ObjectReference &&
                property.objectReferenceValue != null)
            {
                return 0;
            }
            return base.GetPropertyHeight(property, label);
        }
    }
}
