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
    [CreateAssetMenu(fileName = "MeshTileTest", menuName = "Gridmap/Mesh Tile Test")]
    public class MeshTileTest : MeshTileBase
    {
        [SerializeField] private Mesh testMesh;

        /// <summary>
        /// return specific mesh for testing.
        /// </summary>
        /// <returns></returns>
        public override Mesh GetMesh()
        {
            return testMesh;
        }
    }
}
