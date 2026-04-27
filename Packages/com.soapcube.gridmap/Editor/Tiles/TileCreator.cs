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
using System.Linq;

namespace Gridmap.Editor
{
    public static class TileCreator
    {
        #region CONSTS
        private const string SO_PATH_ENDING = ".asset";
        #endregion
        //[MenuItem("Assets/Create/Gridmap/Debug Selection")]
        //private static void DebugSelection()
        //{
        //    Debug.Log(Selection.activeGameObject.transform.GetChild(0));
        //}

        [MenuItem("Assets/Create/Gridmap/Tile From Mesh")]
        private static void CreateTileFromMesh()
        {
            GameObject[] modelObjs = Selection.gameObjects;

            foreach (GameObject obj in modelObjs)
            {
                if (PrefabUtility.GetPrefabAssetType(obj) == PrefabAssetType.Model)
                {
                    // Get the real root mesh components of the model, getting from children if there.
                    MeshFilter meshF = obj.GetComponentInChildren<MeshFilter>();
                    MeshRenderer meshR = obj.GetComponentInChildren<MeshRenderer>();

                    // Create the Tile
                    MeshTile tile = ScriptableObject.CreateInstance<MeshTile>();

                    // Configure the tile's settings.
                    tile.Mesh = meshF.sharedMesh;
                    tile.Materials = meshR.sharedMaterials;

                    // Save the tile as an asset.
                    string directory = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(obj));
                    string path = System.IO.Path.Combine(directory, obj.name + SO_PATH_ENDING);
                    path = AssetDatabase.GenerateUniqueAssetPath(path);
                    AssetDatabase.CreateAsset(tile, path);
                }
            }
        }

        [MenuItem("Assets/Create/Gridmap/Tile From Mesh", true)]
        private static bool ValidateMeshSelection()
        {
            return Selection.objects.Any(x => x is GameObject go && PrefabUtility.GetPrefabAssetType(go) == PrefabAssetType.Model);
        }
    }
}
