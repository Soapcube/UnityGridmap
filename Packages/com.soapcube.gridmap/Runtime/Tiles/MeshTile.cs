/*****************************************************************************
// File Name : MeshTileTest.cs
// Author : Arcadia Koederitz
// Creation Date : 12/23/2025
// Last Modified : 12/23/2025
//
// Brief Description : Concrete MeshTile class for me to use in testing.
*****************************************************************************/
using UnityEngine;

namespace Gridmap
{
    [Icon(ICON_PATH)]
    [CreateAssetMenu(fileName = "MeshTile", menuName = "Gridmap/Tiles/Mesh Tile")]
    public class MeshTile : GridTileBase
    {
        private const string ICON_PATH = "Packages/com.soapcube.gridmap/Editor/EditorDefaultResources/Icons/d_MeshTile@64.png";

        [SerializeField] private Mesh mesh;
        [SerializeField] private Vector3 offset;
        [SerializeField] private Material[] materials;


        #region Properties
        public Mesh Mesh { get =>  mesh; set => mesh = value; }
        public Material[] Materials { get => materials; set => materials = value; }
        #endregion

        internal override Vector3 Offset => offset;

        /// <summary>
        /// return specific mesh for testing.
        /// </summary>
        /// <returns></returns>
        public override Mesh GetMesh()
        {
            return mesh;
        }

        public override Material[] GetMaterials()
        {
            return materials;
        }
    }
}
