using UnityEngine;

namespace Gridmap
{
    [CreateAssetMenu(fileName = "MeshTile", menuName = "Gridmap/Tiles/MeshTile")]
    public class MeshTile : MeshTileBase
    {
        public override Mesh GetMesh()
        {
            return Mesh;
        }
    }
}
