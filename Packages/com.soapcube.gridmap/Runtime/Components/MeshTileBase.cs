/*****************************************************************************
// File Name : MeshTileBase.cs
// Author :Lucas Fehlberg
// Creation Date : 12/22/2025
// Last Modified : 12/22/2025
//
// Brief Description : Base class for mesh tiles that contain data about a mesh to paint on the gridmap.
*****************************************************************************/
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Gridmap
{
    public abstract class MeshTileBase : ScriptableObject
    {
        /// <summary>
        /// Gets the mesh to paint on the gridmap, taking into account any vertex/face removal or rules.
        /// </summary>
        /// <returns></returns>
        public abstract Mesh GetMesh();
    }
}
