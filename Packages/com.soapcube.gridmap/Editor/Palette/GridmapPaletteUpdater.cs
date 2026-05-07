/*****************************************************************************
// File Name : GridPaletteUpdater.cs
// Author : Arcadia Koederitz
// Creation Date : 4/8/2025
// Last Modified : 4/8/2025
//
// Brief Description : Automatically updates and GridPalettes when their associated tilemap changes.
*****************************************************************************/
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Gridmap.Editor
{
    [InitializeOnLoad]
    public class GridmapPaletteUpdater
    {
        #region CONSTS
        private const string PALETTE_UNDO_NAME = "Edit Palette";
        #endregion

        private static Dictionary<int, GridmapPalette> undoPaletteDict = new Dictionary<int, GridmapPalette>();

        /// <summary>
        /// Subscribe to tilemap changed event.
        /// </summary>
        static GridmapPaletteUpdater()
        {
            Tilemap.tilemapTileChanged += UpdateGridPalette;
            Undo.undoRedoEvent += UpdateGridPaletteUndo;

        }

        /// <summary>
        /// Rebakes the mesh when the palette is updated from an undo.
        /// </summary>
        /// <param name="info"></param>
        private static void UpdateGridPaletteUndo(in UndoRedoInfo info)
        {
            Debug.Log(info.undoGroup);
            if (!undoPaletteDict.ContainsKey(info.undoGroup)) { return; }
            GridmapPalette paletteInstance = undoPaletteDict[info.undoGroup];
            if (info.undoName == PALETTE_UNDO_NAME)
            {
                // Only bake the mesh itself, no changes to the palette.
                paletteInstance.BakeMeshAsset();
                EditorUtility.SetDirty(paletteInstance.Mesh);

                //string[] assetGUIDs = AssetDatabase.FindAssets("t:GridmapPaletteData");
                //foreach (string assetGUID in assetGUIDs)
                //{
                //    // So this whole bit is wrong.  The issue is that the palette is saved on an instance before 
                //    // changes are applied to the prefab.  So I need to rebake the instance's mesh.
                //    string path = AssetDatabase.GUIDToAssetPath(assetGUID);
                //    GameObject paletteGo = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                //    GridmapPalette palette = paletteGo.GetComponentInChildren<GridmapPalette>();
                //    // Compare to the
                //    if (palette != null && paletteInstance.Mesh == palette.Mesh)
                //    {
                        
                //    }
                //}
            }
            
        }

        /// <summary>
        /// When a tilemap changes, check if it's a GridPalette and update the associated palette.
        /// </summary>
        /// <param name="tilemap">The tilemap that was modified.</param>
        /// <param name="tileChanges"> information on the tiles that were changed.</param>
        private static void UpdateGridPalette(Tilemap tilemap, Tilemap.SyncTile[] tileChanges)
        {

            if (tilemap.TryGetComponent(out GridmapPalette palette))
            {
                //foreach(Tilemap.SyncTile tileSync in tileChanges)
                //{
                //    if (tileSync.tile is GridTileBase gridTile)
                //    {
                //        gridmapPalette.PlaceTileAtPoint(gridTile, tileSync.position);
                //    }
                //}
                BoundsInt editedBounds = 
                    GridmapUtilities.GetBoundsFromPositions(tileChanges.Select(x => x.position).ToArray());
                palette.BakeMesh(editedBounds);

                // Log the change to the palette in the undo dict using the current undo group so it can be tracked.
                undoPaletteDict.Add(Undo.GetCurrentGroup(), palette);

                // Save changes to the prefab.
                EditorUtility.SetDirty(palette.Mesh);
                Debug.Log(Undo.GetCurrentGroup() + " " + Undo.GetCurrentGroupName());
            }
        }

        private static GameObject GetPaletteRoot(GridmapPalette palette)
        {
            if (palette == null) { return null; }
            return palette.GetComponentInParent<Grid>().gameObject;
        }
    }
}
