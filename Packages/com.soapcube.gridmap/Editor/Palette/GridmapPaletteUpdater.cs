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
        // Saves links between palette assets and palette instances created by the clipboard.
        private static Dictionary<GameObject, GridmapPalette> instancePrefabDict = new Dictionary<GameObject, GridmapPalette>();

        /// <summary>
        /// Subscribe to tilemap changed event.
        /// </summary>
        static GridmapPaletteUpdater()
        {
            Tilemap.tilemapTileChanged += UpdatePaletteInstance;
            Undo.undoRedoEvent += GridPaletteUndo;

        }

        /// <summary>
        /// Rebakes the mesh of a palette instance when the palette is updated from an undo.
        /// </summary>
        /// <param name="info"></param>
        private static void GridPaletteUndo(in UndoRedoInfo info)
        {
            if (!undoPaletteDict.ContainsKey(info.undoGroup)) { return; }
            GridmapPalette paletteInstance = undoPaletteDict[info.undoGroup];
            if (info.undoName == PALETTE_UNDO_NAME)
            {
                // Only bake the mesh itself, no changes to the palette.
                //paletteInstance.BakeMeshRaw();
                //EditorUtility.SetDirty(paletteInstance.Mesh);
            }
            
        }

        /// <summary>
        /// When a tilemap changes, check if it's a GridPalette and update the associated palette.
        /// </summary>
        /// <param name="tilemap">The tilemap that was modified.</param>
        /// <param name="tileChanges"> information on the tiles that were changed.</param>
        private static void UpdatePaletteInstance(Tilemap tilemap, Tilemap.SyncTile[] tileChanges)
        {

            if (tilemap.TryGetComponent(out GridmapPalette palette))
            {
                BoundsInt editedBounds = 
                    GridmapUtilities.GetBoundsFromPositions(tileChanges.Select(x => x.position).ToArray());
                palette.BakeMesh(editedBounds);

                // Log the change to the palette in the undo dict using the current undo group so it can be tracked.
                undoPaletteDict.Add(Undo.GetCurrentGroup(), palette);

                // Save a link to this palette instance from the asset.
                string path = GetPalettePath(palette);
                GameObject paletteAsset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (!instancePrefabDict.ContainsKey(paletteAsset))
                {
                    instancePrefabDict.Add(paletteAsset, palette);
                }
                else if(instancePrefabDict[paletteAsset] == null)
                {
                    instancePrefabDict[paletteAsset] = palette;
                }
            }
        }



        /// <summary>
        /// Gets the palette asset GameObject from a palette.
        /// </summary>
        /// <param name="palette"></param>
        /// <returns></returns>
        private static string GetPalettePath(GridmapPalette palette)
        {
            return AssetDatabase.GetAssetPath(palette.PaletteData);
        }

        // Handles applying mesh changes when the palette instance is saved.
        public class GridmapPaletteAssetModificationProcessor : AssetModificationProcessor
        {
            static string[] OnWillSaveAssets(string[] paths)
            {
                // Check for palette modifications.
                foreach(string path in paths)
                {
                    // Check if a GridmapPalette is being saved.
                    if(AssetDatabase.LoadAssetAtPath<GridmapPaletteData>(path) != null)
                    {
                        GameObject paletteGo = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                        GridmapPalette palette = paletteGo.GetComponentInChildren<GridmapPalette>();

                        // Get the palette instance that was modified from a stored dictionary of changed instances.
                        GridmapPalette paletteInstance = instancePrefabDict[paletteGo];

                        Debug.Log("Instance is: " + paletteInstance);

                        paletteInstance.Mesh.CopyTo(palette.Mesh);
                        paletteInstance.Mesh = palette.Mesh;
                    }
                }

                string debug = "Saving Assets: ";
                foreach (string path in paths)
                {
                    debug += path + ", ";
                }
                Debug.Log(debug);
                return paths;
            }


        }
    }
}
