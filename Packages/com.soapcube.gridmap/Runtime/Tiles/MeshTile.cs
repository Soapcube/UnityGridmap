/*****************************************************************************
// File Name : MeshTileTest.cs
// Author : Brandon Koederitz
// Creation Date : 12/23/2025
// Last Modified : 12/23/2025
//
// Brief Description : Concrete MeshTile class for me to use in testing.
*****************************************************************************/
using UnityEngine;

namespace Gridmap
{
    [CreateAssetMenu(fileName = "MeshTile", menuName = "Gridmap/Mesh Tile")]
    public class MeshTile : GridTileBase
    {
        [SerializeField] private Mesh testMesh;
        [SerializeField] private Vector3 offset;
        [SerializeField] private Material[] materials;

        internal override Vector3 Offset => offset;

        /// <summary>
        /// return specific mesh for testing.
        /// </summary>
        /// <returns></returns>
        public override Mesh GetMesh()
        {
            return testMesh;
        }

        public override Material[] GetMaterials()
        {
            return materials;
        }
    }
}
