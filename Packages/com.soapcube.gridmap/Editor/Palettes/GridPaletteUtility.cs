/*****************************************************************************
// File Name : GridPaletteUtility.cs
// Author : Arcadia Koederitz
// Creation Date : 4/5/2025
// Last Modified : 4/5/2025
//
// Brief Description : Editor script for managing GridPalettes
*****************************************************************************/
using UnityEditor;
using UnityEngine;

namespace Gridmap.Editor
{
    public static class GridPaletteUtility
    {
        #region CONSTS
        private const string GRIDPALETTE_PATH = "GridPalette.prefab";
        private const string TEMPLATE_PATH = "";
        #endregion

        private static Texture2D prefabIcon = (EditorGUIUtility.IconContent("Prefab Icon").image as Texture2D);

        #region Nested
        public class DoCreatePaletteFile : UnityEditor.ProjectWindowCallback.EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                Object o = CreatePalettePrefab(pathName, resourceFile);
                ProjectWindowUtil.ShowCreatedAsset(o);
            }
        }
        #endregion

        [MenuItem("Assets/Create/Gridmap/Grid Palette")]
        public static void CreateGridPalette()
        {
            //ProjectWindowUtil.CreateScriptAssetFromTemplateFile(TEMPLATE_PATH, "GridPalette.prefab");
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, 
                ScriptableObject.CreateInstance<DoCreatePaletteFile>(), GRIDPALETTE_PATH, prefabIcon, TEMPLATE_PATH);
        }

        /// <summary>
        /// Creates the actual GridPalette prefab object.
        /// </summary>
        /// <param name="pathName">The path where the palette will be saved.</param>
        /// <returns>The palette prefab created.</returns>
        internal static UnityEngine.Object CreatePalettePrefab(string pathName, string templatePath)
        {
            GameObject tempGo = CreatePaletteGameObject();


            // Configure a non-duplicate asset path.

            GameObject prefab = PrefabUtility.SaveAsPrefabAssetAndConnect(tempGo, pathName, 
                InteractionMode.AutomatedAction);
            GridPalette palette = CreatePaletteSettings();
            AssetDatabase.AddObjectToAsset(palette, prefab);
            PrefabUtility.ApplyPrefabInstance(tempGo, InteractionMode.AutomatedAction);
            AssetDatabase.Refresh();

            Object.DestroyImmediate(tempGo);
            return AssetDatabase.LoadAssetAtPath<GameObject>(pathName);
        }

        /// <summary>
        /// Creates the GameObject that holds the GridPalette.
        /// </summary>
        /// <returns></returns>
        internal static GameObject CreatePaletteGameObject()
        {
            GameObject tempGo = new GameObject();

            // Configure prefab.

            return tempGo;
        }

        /// <summary>
        /// Creates the GridPalette ScriptableObject that holds the grid settings
        /// </summary>
        /// <returns></returns>
        internal static GridPalette CreatePaletteSettings()
        {
            GridPalette gp = ScriptableObject.CreateInstance<GridPalette>();

            // Configure GP here.

            return gp;
        }
    }
}
