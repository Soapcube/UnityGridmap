/*****************************************************************************
// File Name : GridPaletteUtility.cs
// Author : Arcadia Koederitz
// Creation Date : 4/5/2025
// Last Modified : 4/5/2025
//
// Brief Description : Editor script for managing GridPalettes
*****************************************************************************/
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Gridmap.Editor
{
    public static class GridPaletteUtility
    {
        #region CONSTS
        private const string GRIDPALETTE_PATH = "GridPalette.prefab";
        private const string TEMPLATE_PATH = "";

        // Defaults
        private const string DEFAULT_LAYER_NAME = "Layer1";
        private const string DEFAULT_PALETTESO_NAME = "Palette Settings";
        #endregion

        private static Texture2D prefabIcon = (EditorGUIUtility.IconContent("Prefab Icon").image as Texture2D);

        #region Nested
        public class DoCreateRectangularPaletteFile : UnityEditor.ProjectWindowCallback.EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                Object o = CreatePalettePrefab(pathName, resourceFile, GridLayout.CellLayout.Rectangle, Vector3.one);
                ProjectWindowUtil.ShowCreatedAsset(o);
            }
        }
        #endregion

        [MenuItem("Assets/Create/Gridmap/Grid Palette")]
        public static void CreateGridPalette()
        {
            //ProjectWindowUtil.CreateScriptAssetFromTemplateFile(TEMPLATE_PATH, "GridPalette.prefab");
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, 
                ScriptableObject.CreateInstance<DoCreateRectangularPaletteFile>(), GRIDPALETTE_PATH, prefabIcon, TEMPLATE_PATH);
        }

        /// <summary>
        /// Creates the actual GridPalette prefab object.
        /// </summary>
        /// <param name="pathName">The path where the palette will be saved. (Automatically made unique)</param>
        /// <returns>The palette prefab created.</returns>
        internal static UnityEngine.Object CreatePalettePrefab(string pathName, string templatePath, 
            GridLayout.CellLayout layout, Vector3 cellSize)
        {
            string name = Path.GetFileNameWithoutExtension(pathName);
            // Create the GridPalette template GameObject.
            GameObject tempGo = CreatePaletteGameObject(name, layout, cellSize);

            GameObject prefab = PrefabUtility.SaveAsPrefabAssetAndConnect(tempGo, pathName, 
                InteractionMode.AutomatedAction);
            // Creat the SO that configures palette settings.
            GridPalette palette = CreatePaletteSettings();
            AssetDatabase.AddObjectToAsset(palette, prefab);
            PrefabUtility.ApplyPrefabInstance(tempGo, InteractionMode.AutomatedAction);
            AssetDatabase.Refresh();

            // Destroy the template.
            Object.DestroyImmediate(tempGo);
            return AssetDatabase.LoadAssetAtPath<GameObject>(pathName);
        }

        /// <summary>
        /// Creates the GameObject that holds the GridPalette.
        /// </summary>
        /// <param name="name">The name of the palette GameObject.</param>
        /// <param name="layout">The layout of the cells.</param>
        /// <param name="cellSize">The size of celles in the palette.</param>
        /// <returns></returns>
        internal static GameObject CreatePaletteGameObject(string name, GridLayout.CellLayout layout, Vector3 cellSize)
        {
            GameObject tempGo = new(name);

            // Configure tilemap defaults.
            Grid grid = tempGo.AddComponent<Grid>();
            grid.cellSize = cellSize;
            grid.cellLayout = layout;
            // Always use XYZ swizzle.
            grid.cellSwizzle = GridLayout.CellSwizzle.XYZ;
            Tilemap layer = CreatePaletteLayer(tempGo, DEFAULT_LAYER_NAME, layout);

            // Configure GridMap specific components.

            return tempGo;
        }

        /// <summary>
        /// Adds a new layer to a GridPalette.
        /// </summary>
        /// <param name="palette"></param>
        /// <param name="layerName"></param>
        /// <param name="layout"></param>
        /// <returns></returns>
        private static Tilemap CreatePaletteLayer(GameObject palette, string layerName, GridLayout.CellLayout layout)
        {
            GameObject layerGo = new GameObject(layerName);
            Tilemap tilemap = layerGo.AddComponent<Tilemap>();
            layerGo.transform.parent = palette.transform;
            layerGo.layer = palette.layer;

            // Set defaults for certain layouts.
            switch (layout)
            {
                case GridLayout.CellLayout.Hexagon:
                    tilemap.tileAnchor = Vector3.zero;
                    break;
            }
            
            return tilemap;
        }

        /// <summary>
        /// Creates the GridPalette ScriptableObject that holds the grid settings
        /// </summary>
        /// <returns></returns>
        internal static GridPalette CreatePaletteSettings()
        {
            GridPalette paletteSo = ScriptableObject.CreateInstance<GridPalette>();

            paletteSo.name = DEFAULT_PALETTESO_NAME;
            // Set some default settings as these shouldn't need to change.
            paletteSo.cellSizing = GridPalette.CellSizing.Automatic;
            paletteSo.transparencySortAxis = Vector3.forward;
            paletteSo.transparencySortMode = TransparencySortMode.Default;

            return paletteSo;
        }
    }
}
