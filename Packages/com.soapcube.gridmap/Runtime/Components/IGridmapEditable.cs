/*****************************************************************************
// File Name : IGridmapEditable.cs
// Author : Arcadia Koederitz
// Creation Date : 4/8/2025
// Last Modified : 4/8/2025
//
// Brief Description : Interface for objects that can be edited like a gridmap.
*****************************************************************************/
using UnityEngine;

namespace Gridmap
{
    public interface IGridmapEditable
    {
        void PlaceTileAtPoint(GridTileBase tile, Vector3Int point);
        GridTileBase GetTileAtPoint(Vector3Int pos);
        void BakeMesh(BoundsInt editedBounds);
    }
}
