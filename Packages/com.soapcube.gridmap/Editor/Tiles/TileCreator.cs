/*****************************************************************************
// File Name : TileCreator.cs
// Author : Arcadia Koederitz
// Creation Date : 4/26/2025
// Last Modified : 4/26/2025
//
// Brief Description : Handles auto-generating MeshTiles from user meshes.
*****************************************************************************/
using UnityEngine;
using UnityEditor;

namespace Gridmap.Editor
{
    public static class TileCreator
    {
        [MenuItem("Assets/Create/Gridmap/Debug Selection")]
        private static void DebugSelection()
        {
            Debug.Log(Selection.activeGameObject.transform.GetChild(0));
        }

        [MenuItem("Assets/Create/Gridmap/Tile From Mesh")]
        private static void CreateTileFromMesh()
        {
            GameObject modelObj = Selection.activeGameObject;

            // Get the real root mesh components of the model, getting from children if there.

            // Create the Tile
            MeshTile tile = ScriptableObject.CreateInstance<MeshTile>();

            // Configure the tile's settings.

            // Save the tile as an asset.
        }

        [MenuItem("Assets/Create/Gridmap/Tile From Mesh", true)]
        private static bool ValidateMeshSelection()
        {
            return Selection.activeObject is GameObject go && 
                PrefabUtility.GetPrefabAssetType(go) == PrefabAssetType.Model;
        }
    }
}
