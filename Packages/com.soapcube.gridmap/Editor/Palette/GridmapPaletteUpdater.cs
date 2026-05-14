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
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Gridmap.Editor
{
    [InitializeOnLoad]
    public class GridmapPaletteUpdater
    {
        // Saves links between palette assets and palette instances created by the clipboard.
        private readonly static Dictionary<GameObject, GridmapPalette> instancePrefabDict = 
            new Dictionary<GameObject, GridmapPalette>();

        /// <summary>
        /// Subscribe to tilemap changed event.
        /// </summary>
        static GridmapPaletteUpdater()
        {
            Tilemap.tilemapTileChanged += UpdatePaletteInstance;
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
                palette.Bake(editedBounds);

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

                        paletteInstance.Mesh.CopyTo(palette.Mesh);
                        paletteInstance.Mesh = palette.Mesh;
                    }
                }
                return paths;
            }


        }
    }
}
