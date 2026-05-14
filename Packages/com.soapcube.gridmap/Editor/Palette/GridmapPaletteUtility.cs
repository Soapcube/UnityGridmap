/*****************************************************************************
// File Name : GridPaletteUtility.cs
// Author : Arcadia Koederitz
// Creation Date : 4/5/2025
// Last Modified : 4/8/2025
//
// Brief Description : Editor script for managing GridPalettes
*****************************************************************************/
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Gridmap.Editor
{
    public static class GridmapPaletteUtility
    {
        #region CONSTS
        private const string GRIDPALETTE_PATH = "GridPalette.prefab";
        private const string TEMPLATE_PATH = "";

        // Defaults
        private const string DEFAULT_LAYER_NAME = "Layer1";
        private const string DEFAULT_PALETTESO_NAME = "Palette Settings";

        private const string ASSET_FOLDER = "Assets";
        private const string MESH_FILE_EXTENSION = ".mesh";
        #endregion

        private static Texture2D prefabIcon = (EditorGUIUtility.IconContent("Prefab Icon").image as Texture2D);

        #region Nested
        public class DoCreateRectangularPaletteFile : UnityEditor.ProjectWindowCallback.EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                Object o = CreatePalettePrefab(pathName, resourceFile, GridLayout.CellLayout.Rectangle, Vector3.one, GridmapEditorUtility.RECT_ANCHOR);
                ProjectWindowUtil.ShowCreatedAsset(o);
            }
        }

        public class DoCreateHexagonalPaletteFile : UnityEditor.ProjectWindowCallback.EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                Object o = CreatePalettePrefab(pathName, resourceFile, GridLayout.CellLayout.Hexagon, GridmapEditorUtility.HEX_GRID_SIZE, Vector3.zero);
                ProjectWindowUtil.ShowCreatedAsset(o);
            }
        }
        #endregion

        [MenuItem("Assets/Create/Gridmap/Gridmap Palette/Rectangular", false, (int)GridmapEditorUtility.GridmapCreatePriority.Rectangular)]
        public static void CreateGridmapPaletteRect()
        {
            //Utilized built-in project window utilities to create the GridPalette object.
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, 
                ScriptableObject.CreateInstance<DoCreateRectangularPaletteFile>(), GRIDPALETTE_PATH, prefabIcon, TEMPLATE_PATH);
        }
        [MenuItem("Assets/Create/Gridmap/Gridmap Palette/Hexagonal", false, (int)GridmapEditorUtility.GridmapCreatePriority.Hexagonal)]
        public static void CreateGridmapPaletteHex()
        {
            //Utilized built-in project window utilities to create the GridPalette object.
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                ScriptableObject.CreateInstance<DoCreateHexagonalPaletteFile>(), GRIDPALETTE_PATH, prefabIcon, TEMPLATE_PATH);
        }

        /// <summary>
        /// Creates the actual GridPalette prefab object.
        /// </summary>
        /// <param name="pathName">The path where the palette will be saved. (Automatically made unique)</param>
        /// <returns>The palette prefab created.</returns>
        internal static Object CreatePalettePrefab(string pathName, string templatePath, 
            GridLayout.CellLayout layout, Vector3 cellSize, Vector3 anchor)
        {
            string name = Path.GetFileNameWithoutExtension(pathName);

            // Create the mesh that renders the grid palette.
            Mesh paletteMesh = MeshHelper.NewGridMesh(name + " Mesh");

            // Create the SO that configures palette settings.
            GridmapPaletteData palette = CreatePaletteSettings();

            // Create the GridPalette prefab.
            GameObject tempGo = CreatePaletteGameObject(name, layout, cellSize, paletteMesh, anchor, palette);
            GameObject prefab = PrefabUtility.SaveAsPrefabAssetAndConnect(tempGo, pathName, 
                InteractionMode.AutomatedAction);

            // Add Sub-Assets.
            AssetDatabase.AddObjectToAsset(palette, prefab);
            // I want to make sub-assets work, but they conflict with how palettes are saved.  Going to do some research.
            AssetDatabase.AddObjectToAsset(paletteMesh, prefab); 

            //string directory = Path.GetDirectoryName(pathName);
            //AssetDatabase.CreateAsset(paletteMesh, Path.Combine(directory, paletteMesh.name + ".asset"));

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
        /// <param name="paletteMesh">The mesh that stores rendering information about the grid palette.</param>
        /// <returns>The created grid palette GameObject.</returns>
        internal static GameObject CreatePaletteGameObject(string name, GridLayout.CellLayout layout, Vector3 cellSize, 
            Mesh paletteMesh, Vector3 tileAnchor, GridPalette palette)
        {
            GameObject tempGo = new(name);

            // Configure tilemap defaults.
            Grid grid = tempGo.AddComponent<Grid>();
            grid.cellSize = cellSize;
            grid.cellLayout = layout;
            grid.cellSwizzle = GridLayout.CellSwizzle.XYZ; // Always use XYZ swizzle.
            Tilemap layer = CreatePaletteLayer(tempGo, DEFAULT_LAYER_NAME, layout, paletteMesh, tileAnchor, palette);

            return tempGo;
        }

        /// <summary>
        /// Adds a new layer to a GridPalette.
        /// </summary>
        /// <param name="palette">The palette to add a layer to.</param>
        /// <param name="layerName">The name of the layer.</param>
        /// <param name="layout">The cell layout of the layer.</param>
        /// <param name="paletteMesh">The mesh that the layer uses to render the GridPalette.</param>
        /// <returns>The created tilemap component on the layer.</returns>
        private static Tilemap CreatePaletteLayer(GameObject palette, string layerName, GridLayout.CellLayout layout, 
            Mesh paletteMesh, Vector3 tileAnchor, GridPalette paletteData)
        {
            GameObject layerGo = new GameObject(layerName);
            Tilemap tilemap = layerGo.AddComponent<Tilemap>();
            layerGo.transform.parent = palette.transform;
            layerGo.layer = palette.layer;
            tilemap.tileAnchor = tileAnchor;

            // Configure Gridmap Specific Components.
            MeshFilter meshFilter = layerGo.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = layerGo.AddComponent<MeshRenderer>();
            meshFilter.sharedMesh = paletteMesh;

            GridmapPalette gmp = layerGo.AddComponent<GridmapPalette>();
            gmp.Initialize(meshFilter, meshRenderer, tilemap, paletteMesh, paletteData);
            
            return tilemap;
        }

        /// <summary>
        /// Creates the GridPalette ScriptableObject that holds the grid settings
        /// </summary>
        /// <returns>The GridPalette ScriptableObject that holds the palette's editor settings.</returns>
        internal static GridmapPaletteData CreatePaletteSettings()
        {
            GridmapPaletteData paletteSo = ScriptableObject.CreateInstance<GridmapPaletteData>();

            paletteSo.name = DEFAULT_PALETTESO_NAME;
            // Set some default settings as these shouldn't need to change.
            paletteSo.cellSizing = GridPalette.CellSizing.Automatic;
            paletteSo.transparencySortAxis = Vector3.forward;
            paletteSo.transparencySortMode = TransparencySortMode.Default;

            return paletteSo;
        }
    }
}
