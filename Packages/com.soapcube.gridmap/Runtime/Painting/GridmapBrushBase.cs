/*****************************************************************************
// File Name : GridmapBrush.cs
// Author : Arcadia Koederitz
// Creation Date : 12/14/2025
// Last Modified : 12/14/2025
//
// Brief Description : Base class for brushes tha paint meshes on a gridmap.
*****************************************************************************/
using UnityEngine;

namespace Gridmap.Brushes
{
    
    public abstract class GridmapBrushBase : GridBrushBase
    {
        #region CONSTS
        protected static readonly Vector3Int DEFAULT_CELL_SIZE = Vector3Int.one;
        #endregion

        // These are parameters for a size and pivot point int the default tilemap GridBrush.
        // Unsure what they do, but will reference later if necessary.

        [Header("Gridmap Brush Settings")]
        [SerializeField, Tooltip("Whether this brush should automatically update the position of the tilemap to " +
            "paint on different elevation layers.")] 
        protected bool autoUpdateElevation;
        [SerializeField, Tooltip("The elevation of the brush relative to (0, 0, 0).")] 
        protected int brushElevation;

        #region Properties
        public bool AutoUpdateElevation => autoUpdateElevation;
        public int BrushElevation => brushElevation;
        #endregion
    }
}
