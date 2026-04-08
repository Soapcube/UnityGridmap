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

namespace Gridmap.Editor
{
    [InitializeOnLoad]
    public class GridmapPaletteUpdater
    {
        /// <summary>
        /// Subscribe to tilemap changed event.
        /// </summary>
        static GridmapPaletteUpdater()
        {
            Tilemap.tilemapTileChanged += UpdateGridPalette;
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
                foreach(Tilemap.SyncTile tileSync in tileChanges)
                {
                    if (tileSync.tile is GridTileBase gridTile)
                    {
                        gridmapPalette.PlaceTileAtPoint(gridTile, tileSync.position);
                    }
                }
                BoundsInt editedBounds = 
                    GridmapUtilities.GetBoundsFromPositions(tileChanges.Select(x => x.position).ToArray());
                gridmapPalette.BakeMesh(editedBounds);
            }
        }
    }
}
