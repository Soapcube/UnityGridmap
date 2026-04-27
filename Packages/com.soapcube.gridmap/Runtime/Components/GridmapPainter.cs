/*****************************************************************************
// File Name : GridmapPainter.cs
// Author : Arcadia Koederitz
// Creation Date : 4/12/2025
// Last Modified : 4/12/2025
//
// Brief Description : Relay script that reroutes all IGridmapEditable functions to a reference Gridmap.
*****************************************************************************/
using UnityEngine;

namespace Gridmap
{
    public class GridmapPainter : MonoBehaviour, IGridmapEditable
    {
        [SerializeField, ShowIfNull] private Gridmap gmap;

        public void OnCreate(Gridmap gmap)
        {
#if UNITY_EDITOR
            this.gmap = gmap;
#endif
        }

        public void BakeMesh(BoundsInt editedBounds)
        {
            gmap.BakeMesh(editedBounds);
        }

        public GridTileBase GetTileAtPoint(Vector3Int cellPos)
        {
            return gmap.GetTileAtPoint(cellPos);
        }

        public void PlaceTileAtPoint(GridTileBase tile, Vector3Int cellPos)
        {
            gmap.PlaceTileAtPoint(tile, cellPos);
        }

        public Vector3 GridToCenteredPosition(Vector3Int gridPos)
        {
            return gmap.GridToCenteredPosition(gridPos);
        }
    }
}
