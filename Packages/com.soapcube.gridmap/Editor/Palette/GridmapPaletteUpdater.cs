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

                // Save a link to this palette instance from the asset.
                string path = GetPalettePath(palette);
                GameObject paletteAsset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (!instancePrefabDict.ContainsKey(paletteAsset))
                {
                    instancePrefabDict.Add(paletteAsset, palette);
                }
                
                // Save changes to the prefab.
                EditorUtility.SetDirty(palette.Mesh);
                //Debug.Log(Undo.GetCurrentGroup() + " " + Undo.GetCurrentGroupName());
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


        public class GridmapPaletteAssetModificationProcessor : AssetModificationProcessor
        {
            static string[] OnWillSaveAssets(string[] paths)
            {
                // Check for palette modifications.
                foreach(string path in paths)
                {
                    if(AssetDatabase.LoadAssetAtPath<GridmapPaletteData>(path) != null)
                    {
                        GameObject paletteGo = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                        GridmapPalette palette = paletteGo.GetComponentInChildren<GridmapPalette>();

                        GridmapPalette paletteInstance = instancePrefabDict[paletteGo];

                        CopyTo(paletteInstance.Mesh, palette.Mesh);
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

        private static void CopyTo(Mesh original, Mesh target)
        {
            target.name = original.name;
            target.vertices = original.vertices;
            target.normals = original.normals;
            target.tangents = original.tangents;
            target.triangles = original.triangles;
            target.bounds = original.bounds;
            target.uv = original.uv;
            target.uv2 = original.uv2;
            target.uv3 = original.uv3;
            target.uv4 = original.uv4;
            target.uv5 = original.uv5;
            target.uv6 = original.uv6;
            target.uv7 = original.uv7;
            target.uv8 = original.uv8;
            target.colors = original.colors;
            target.bindposes = original.bindposes;
            target.boneWeights = original.boneWeights;
            target.indexFormat = original.indexFormat;
            target.indexBufferTarget = original.indexBufferTarget;
            target.vertexBufferTarget = original.vertexBufferTarget;
            target.subMeshCount = original.subMeshCount;
            for(int i = 0; i < original.subMeshCount; i++)
            {
                target.SetSubMesh(i, original.GetSubMesh(i));
            }
        }

    }
}
