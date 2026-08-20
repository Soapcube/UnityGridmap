/*****************************************************************************
// File Name : GridmapNameUpdater.cs
// Author : Arcadia Koederitz
// Creation Date : 4/20/2025
// Last Modified : 4/20/2025
//
// Brief Description : Automatically updates the name of the painting layer when the gridmap changes.
*****************************************************************************/
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Gridmap.Editor
{
    [InitializeOnLoad]
    public static class GridmapNameUpdater
    {
        static GridmapNameUpdater()
        {
            EditorApplication.hierarchyChanged += UpdateName;
        }

        private static void UpdateName()
        {
            if(Selection.activeObject is GameObject go && go.TryGetComponent(out Gridmap gmap))
            {
                Tilemap painter = gmap.GetComponentInChildren<Tilemap>();
                if (painter != null && painter.name != gmap.name)
                {
                    painter.name = gmap.name;
                }
            }
        }
    }
}
