/*****************************************************************************
// File Name : GridmapEditorUtilities.cs
// Author : Arcadia Koederitz
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
        internal static bool showDialog = true;
        #endregion

        private static class Styles
        {
            internal const string RECT_UNDO_MESSAGE = "Create Rectangular Gridmap";
            internal const string HEX_UNDO_MESSAGE = "Create Hexagonal Gridmap";

            internal const string MODIFY_EXISTING_GRID_TITLE = "Modify existing Grid";
            internal const string MODIFY_EXISTING_GRID_MESSAGE = "Creating the Gridmap will modify the existing grid.  Continue?";
            internal const string MODIFY_EXISTING_CONTINUE = "Continue";
            internal const string MODIFY_EXISTING_CANCEL = "Cancel";
        }


        #region Gridmap Creation
        [MenuItem("GameObject/Gridmap/Rectangular", false, (int)GridmapEditorUtility.GridmapCreatePriority.Rectangular)]
        private static void CreateRectGridmap()
        {
            CreateRectGridmapInternal(Tilemap.CellSwizzle.XZY, Styles.RECT_UNDO_MESSAGE);
        }

        [MenuItem("GameObject/Gridmap/Hexagonal", false, (int)GridmapEditorUtility.GridmapCreatePriority.Hexagonal)]
        private static void CreateHexGridmap()
        {
            CreateHexGridmapInternal(Tilemap.CellSwizzle.XZY, Styles.HEX_UNDO_MESSAGE, GridmapEditorUtility.HEX_GRID_SIZE);
        }

        private static GameObject CreateRectGridmapInternal(Tilemap.CellSwizzle swizzle, string undoMessage)
        {
            if (!FindOrCreateRootGrid(Grid.CellLayout.Rectangle, true, swizzle, false,
                Vector3.one, out GameObject root))
            {
                return null;
            }

            GameObject gridmapGo = CreateGridmapObject(root);

            // Change the grid to rectangle layout
            Grid grid = root.GetComponent<Grid>();
            Undo.RecordObject(grid, undoMessage);
            grid.cellLayout = GridLayout.CellLayout.Rectangle;
            grid.cellSwizzle = swizzle;

            Gridmap gmap = gridmapGo.GetComponent<Gridmap>();
            gmap.tileAnchor = GridmapEditorUtility.RECT_ANCHOR;

            // Create painting layer.
            Tilemap painting = CreatePaintingLayer(gmap, gridmapGo.transform, GridmapEditorUtility.RECT_ANCHOR);

            Selection.activeObject = gridmapGo;
            Undo.SetCurrentGroupName(undoMessage);
            return gridmapGo;
        }

        private static GameObject CreateHexGridmapInternal(Tilemap.CellSwizzle swizzle, string undoMessage, Vector3 cellSize)
        {
            if (!FindOrCreateRootGrid(Grid.CellLayout.Rectangle, false, swizzle, false,
                Vector3.one, out GameObject root))
            {
                return null;
            }

            //Create the Gridmap GameObject
            GameObject gridmapGo = CreateGridmapObject(root);

            // Change the grid to hex layout.
            Grid grid = root.GetComponent<Grid>();
            Undo.RecordObject(grid, undoMessage);
            grid.cellLayout = GridLayout.CellLayout.Hexagon;
            grid.cellSwizzle = swizzle;
            grid.cellSize = cellSize;

            Gridmap gmap = gridmapGo.GetComponent<Gridmap>();
            gmap.tileAnchor = Vector3.zero;

            // Create painting layer.
            Tilemap painting = CreatePaintingLayer(gmap, gridmapGo.transform, Vector3.zero); 

            Selection.activeObject = gridmapGo;
            Undo.SetCurrentGroupName(undoMessage);
            return gridmapGo;
        }

        private static GameObject CreateGridmapObject(GameObject root)
        {
            //Create the Gridmap GameObject
            string uniqueName = GameObjectUtility.GetUniqueNameForSibling(root.transform, "Gridmap");
            GameObject gridmapGo = ObjectFactory.CreateGameObject(uniqueName, typeof(Gridmap));
            Undo.SetTransformParent(gridmapGo.transform, root.transform, ""); // Register with Undo.
            gridmapGo.transform.position = Vector3.zero;
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
            GameObject paintGo = ObjectFactory.CreateGameObject(gmap.name, typeof(Tilemap));
            Undo.SetTransformParent(paintGo.transform, parent.transform, "");

            // Unity is bugged and won't record hide flags in the undo stack.
            paintGo.hideFlags = GridmapUtilities.GRIDMAP_SUB_HIDEFLAGS;

            // Setup components.
            paintGo.transform.position = Vector3.zero;
            Tilemap tmap = paintGo.GetComponent<Tilemap>();
            tmap.tileAnchor = anchor;
            Undo.RegisterCompleteObjectUndo(paintGo, "Create Painter");

            // Setup Components
            Undo.RecordObject(gmap, "Assign Gridmap Components");
            gmap.OnCreate(tmap);

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
                    // Displays a dialogue popup that notifies the user the grid will be auto-updated.
                    if (showDialog
                        && !Application.isBatchMode
                        && (parentGrid.cellLayout != layout // Checks for incongruence.
                            || (changeSwizzle && parentGrid.cellSwizzle != swizzle)
                            || (changeSize && Vector3.Distance(parentGrid.cellSize, size) > 0.001f)))
                    {
                        var option = EditorUtility.DisplayDialog(Styles.MODIFY_EXISTING_GRID_TITLE
                            , Styles.MODIFY_EXISTING_GRID_MESSAGE
                            , Styles.MODIFY_EXISTING_CONTINUE
                            , Styles.MODIFY_EXISTING_CANCEL);
                        if (!option)
                            return false;
                    }

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
