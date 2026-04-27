/*****************************************************************************
// File Name : GridmapEditorUtilities.cs
// Author : Brandon Koederitz
// Creation Date : 4/20/2025
// Last Modified : 4/20/2025
//
// Brief Description : Set of static utility functions for the Gridmap Editor scripts.
*****************************************************************************/
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.Experimental.Playables;

namespace Gridmap.Editor
{
    // Based on TilemapEditor.GameObjectCreation.
    public static class GridmapGameObjectCreator
    {
        #region Consts
        private const string GRIDMAP_UNDO_MESSAGE = "Create Gridmap";
        #endregion

        #region Gridmap Creation
        [MenuItem("GameObject/Gridmap/Gridmap", false, 15)]
        private static void CreateGridmap()
        {
            CreateGridmapInternal();
        }

        private static GameObject CreateGridmapInternal()
        {
            if (!FindOrCreateRootGrid(Grid.CellLayout.Rectangle, false, GridLayout.CellSwizzle.XYZ, false, 
                Vector3.one, out GameObject root))
            {
                return null;
            }

            //Create the Gridmap GameObject
            string uniqueName = GameObjectUtility.GetUniqueNameForSibling(root.transform, "Gridmap");
            GameObject gridmapGo = ObjectFactory.CreateGameObject(uniqueName, typeof(Gridmap)); // TODO: Add components by types.
            Undo.SetTransformParent(gridmapGo.transform, root.transform, ""); // Register with Undo.
            gridmapGo.transform.position = Vector3.zero;

            // Change the grid to rectangle layout
            Grid grid = root.GetComponent<Grid>();
            if (grid.cellLayout != GridLayout.CellLayout.Rectangle)
            {
                Undo.RecordObject(grid, GRIDMAP_UNDO_MESSAGE);
                grid.cellLayout = GridLayout.CellLayout.Rectangle;
            }

            Gridmap gmap = gridmapGo.GetComponent<Gridmap>();

            // Create painting layer.
            Tilemap painting = CreatePaintingLayer(gmap, gridmapGo.transform, new Vector3(0.5f, 0.5f, 0.5f));

            // Setup Components
            gmap.OnCreate(painting);

            Selection.activeObject = gridmapGo;
            Undo.SetCurrentGroupName(GRIDMAP_UNDO_MESSAGE);
            return gridmapGo;
        }

        /// <summary>
        /// Creates the GameObject for the gridmap's painting layer.
        /// </summary>
        /// <param name="gmap"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        private static Tilemap CreatePaintingLayer(Gridmap gmap, Transform parent, Vector3 anchor)
        {
            GameObject paintGo = ObjectFactory.CreateGameObject(gmap.name, typeof(Tilemap), typeof(GridmapPainter));
            Undo.SetTransformParent(paintGo.transform, parent.transform, "");

            paintGo.hideFlags = GridmapUtilities.GRIDMAP_SUB_HIDEFLAGS;

            // Setup components.
            paintGo.transform.position = Vector3.zero;
            Tilemap tmap = paintGo.GetComponent<Tilemap>();
            tmap.tileAnchor = anchor;
            GridmapPainter painter = paintGo.GetComponent<GridmapPainter>();
            painter.OnCreate(gmap);

            return tmap;
        }

        /// <summary>
        /// Checks if the current editor selection is a child of a valid grid component.
        /// </summary>
        /// <param name="layout">The grid layout to check for.</param>
        /// <param name="changeSwizzle">If true, then the grid will be updated if the swizzles dont match.</param>
        /// <param name="swizzle">The cell swizzle to check for.</param>
        /// <param name="changeSize">If true, then the grid will be updated if the size doesn't match.</param>
        /// <param name="size">The size to check for.</param>
        /// <param name="root">The root grid gameobject.</param>
        /// <returns>True if a valid grid was found.</returns>
        private static bool FindOrCreateRootGrid(Grid.CellLayout layout, bool changeSwizzle, Grid.CellSwizzle swizzle, 
            bool changeSize, Vector3 size, out GameObject root)
        {
            root = null;
            if (Selection.activeObject is GameObject)
            {
                GameObject go = (GameObject)Selection.activeObject;
                Grid parentGrid = go.GetComponentInParent<Grid>();
                if ( parentGrid != null)
                {
                    // Add editor dialogue for modifying an existing grid here.
                    root = parentGrid.gameObject;
                }
            }

            if (root == null)
            {
                // Create the grid if it doesn't exist.
                root = ObjectFactory.CreateGameObject("Grid", typeof(Grid));
                root.transform.position = Vector3.zero;

                Grid grid = root.GetComponent<Grid>();
                grid.cellSize = Vector3.one;
                // Register undo.
                Undo.SetCurrentGroupName("Create Grid");
            }
            return true;
        }
        #endregion
    }
}
