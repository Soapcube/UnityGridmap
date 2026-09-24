/*****************************************************************************
// File Name : GridmapUndoUtility.cs
// Author : Arcadia Koederitz
// Creation Date : 5/14/2025
// Last Modified : 5/14/2026
//
// Brief Description : Stores data about the tiles in a certain chunk in the gridmap.
*****************************************************************************/
using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Gridmap.Editor
{
    [InitializeOnLoad]
    public static class GridmapUndoUtility
    {
        private const string GMAP_UNDO_IDENTIFIER = "Gridmap ID";

        static GridmapUndoUtility()
        {
            Undo.undoRedoEvent += HandleGridmapUndo;
        }

        /// <summary>
        /// Handles rebaking a gridmap after an undo.
        /// </summary>
        /// <param name="info"></param>
        private static void HandleGridmapUndo(in UndoRedoInfo info)
        {
            if (info.undoName.Contains(GMAP_UNDO_IDENTIFIER))
            {
                EntityId entityId = GetEntityIDFromUndoName(info.undoName);
                GameObject gmapObbj = EditorUtility.EntityIdToObject(entityId) as GameObject;
                if (gmapObbj != null && gmapObbj.TryGetComponent(out Gridmap gmap))
                {
                    // Bake all for now, we can make it more efficient by storing edited chunks.
                    gmap.BakeAllChunks();
                }
            }
        }

        private static EntityId GetEntityIDFromUndoName(string name)
        {
            // Get the edited gridmap from the instance id.
            int startIndex = name.IndexOf(GMAP_UNDO_IDENTIFIER) + GMAP_UNDO_IDENTIFIER.Length + 1;
            int endIndex = name.Length - 1;
            string instanceIdString = name.Substring(startIndex, endIndex - startIndex);
            // Updated from InstanceID int to EntityId ulong.  Should update the undo utility to be more flexible
            // with new implementations of EntityId since it's stated to be changed.
            ulong instanceId = UInt64.Parse(instanceIdString);
            return EntityId.FromULong(instanceId);
        }

        /// <summary>
        /// Records changes to a gridmap in the undo stack, with handling to rebake the mesh.
        /// </summary>
        /// <param name="gmap">The gridmap to register for undo.</param>
        /// <param name="undoMessage">The undo message.</param>
        public static void RegisterGridmapUndo(Gridmap gmap, string undoMessage)
        {
            //Uses the undo message to encode the Instance ID of the modified gridmap so it can be found for
            //rebaking later.
            Undo.RegisterFullObjectHierarchyUndo(gmap.gameObject, undoMessage + 
                $" ({GMAP_UNDO_IDENTIFIER}:{EntityId.ToULong(gmap.gameObject.GetEntityId())})");
        }
    }
}
