/*****************************************************************************
// File Name : GridPaletteUpdater.cs
// Author : Arcadia Koederitz
// Creation Date : 4/8/2025
// Last Modified : 4/8/2025
//
// Brief Description : Automatically updates and GridPalettes when their associated tilemap changes.
*****************************************************************************/
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor.EditorTools;

namespace Gridmap.Editor
{
    [InitializeOnLoad]
    public class GridmapPaletteUpdater
    {
        #region CONSTS
        private const string PALETTE_UNDO_NAME = "Edit Palette";
        #endregion

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
            if (info.undoName == "Edit Palette")
            {
                string[] assetGUIDs = AssetDatabase.FindAssets("t:GridmapPaletteData");
                foreach (string assetGUID in assetGUIDs)
                {
                    string path = AssetDatabase.GUIDToAssetPath(assetGUID);
                    GameObject paletteGo = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    GridmapPalette palette = paletteGo.GetComponentInChildren<GridmapPalette>();
                    if (palette != null)
                    {
                        // Pass in an empty BoundsInt as the bounds passed to a Palette doesn't matter.
                        Debug.Log("Baking " + palette);
                        palette.BakeMesh(new BoundsInt());
                    }
                }
            }
            
        }

        /// <summary>
        /// When a tilemap changes, check if it's a GridPalette and update the associated palette.
        /// </summary>
        /// <param name="tilemap">The tilemap that was modified.</param>
        /// <param name="tileChanges"> information on the tiles that were changed.</param>
        private static void UpdateGridPalette(Tilemap tilemap, Tilemap.SyncTile[] tileChanges)
        {
            if (tilemap.TryGetComponent(out GridmapPalette gridmapPalette))
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
                gridmapPalette.BakeMesh(editedBounds);
            }
        }
    }
}
