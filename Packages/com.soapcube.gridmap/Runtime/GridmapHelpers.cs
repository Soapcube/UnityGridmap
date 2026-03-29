/*****************************************************************************
// File Name : GridmapHelpers.cs
// Author : Brandon Koederitz
// Creation Date : 12/15/2025
// Last Modified : 12/15/2025
//
// Brief Description : Set of static helper functions and core definitions for the Gridmap system.
*****************************************************************************/
using UnityEngine;

namespace Gridmap
{
    internal static class GridmapHelpers
    {
        #region CONSTS
        private const string ASSET_FOLDER = "Assets";
        private const string MESH_FILE_EXTENSION = ".mesh";
        #endregion

        /// <summary>
        /// Converts a Vector3 position to the correct axes specified by a grid swizzle.
        /// </summary>
        /// <param name="position">The position in XYZ coordinate space.</param>
        /// <param name="targetSwizzleMode">The swizzle mode to convert the position into.</param>
        /// <param name="baseSwizzleMode">The current swizzle mode of the position.</param>
        /// <returns>The position in the specified swizzle mode.</returns>
        internal static Vector3Int ConvertSwizzleSpace(Vector3Int position, 
            Grid.CellSwizzle targetSwizzleMode, 
            Grid.CellSwizzle baseSwizzleMode = Grid.CellSwizzle.XYZ)
        {
            /// Gets the tuple that converts the position from base swizzle spacea into XYZ swizzle space.
            (int, int, int) GetSwizzleTupleBase()
            {
                switch (baseSwizzleMode)
                {
                    case Grid.CellSwizzle.XZY:
                        return (position.x, position.z, position.y);
                    case Grid.CellSwizzle.YXZ:
                        return (position.y, position.x, position.z);
                    // Swaps the condition for the YZX and ZXY swizzles because they need to be inverted to convert from a
                    // given swizzle space to XYZ
                    case Grid.CellSwizzle.YZX:
                        return (position.z, position.x, position.y);
                    case Grid.CellSwizzle.ZXY:
                        return (position.y, position.z, position.x);
                    case Grid.CellSwizzle.ZYX:
                        return (position.z, position.y, position.x);
                    default:
                        return (position.x, position.y, position.z);
                }
            }

            /// Gets the tuple that converts the position from XYZ swizzle space to the target space.
            (int, int, int) GetSwizzleTupleTarget()
            {
                switch (targetSwizzleMode)
                {
                    case Grid.CellSwizzle.XZY:
                        return (position.x, position.z, position.y);
                    case Grid.CellSwizzle.YXZ:
                        return (position.y, position.x, position.z);
                    // This condition gets inverted for GetSwizzleTupleBase
                    case Grid.CellSwizzle.YZX:
                        return (position.y, position.z, position.x);
                    // This condition gets inverted for GetSwizzleTupleBase
                    case Grid.CellSwizzle.ZXY:
                        return (position.z, position.x, position.y);
                    case Grid.CellSwizzle.ZYX:
                        return (position.z, position.y, position.x);
                    default:
                        return (position.x, position.y, position.z);
                }
            }

            (position.x, position.y, position.z) = GetSwizzleTupleBase();
            (position.x, position.y, position.z) = GetSwizzleTupleTarget();

            return position;
        }

        /// <summary>
        /// Returns the canonical modulus of a number to the mod of another number
        /// </summary>
        /// <remarks>
        /// Differs from % in that % gives the remainder, which can be negative.  In this case, negative number loop 
        /// around.
        /// </remarks>
        /// <param name="x">The first neumber</param>
        /// <param name="m">The number to take the mod of.</param>
        /// <returns>The canonical modulus number of X mod M.</returns>
        internal static int Mod(int x, int m)
        {
            return ((x % m) + m) % m;
        }

        /// <summary>
        /// Gets the sign of a given number
        /// </summary>
        /// <param name="x">The number to get the sign of.</param>
        /// <returns>-1, 0, or 1, depending ont the sign of the number.</returns>
        internal static int GetSign(int x)
        {
            if (x == 0) { return 0; }
            return Mathf.Abs(x) / x;
        }

        /// <summary>
        /// Gets the sign of a given number
        /// </summary>
        /// <param name="x">The number to get the sign of.</param>
        /// <returns>-1, 0, or 1, depending ont the sign of the number.</returns>
        internal static int GetSign(float x)
        {
            if (x == 0) { return 0; }
            return (int)(Mathf.Abs(x) / x);
        }

        #region Mesh Management
        /// <summary>
        /// Creates a mesh asset in the project's assets folder to save the baked mesh data.
        /// </summary>
        /// <param name="gridmapName"> The name to use to identify the meshes associated with a given gridmap.</param>
        /// <param name="targetChunk">The chunk that this mesh will belong to.</param>
        /// <param name="createdMesh">The created mesh.</param>
        /// <param name="meshPath">The path in the assets folder that the mesh was saved to.</param>
        /// <param name="subdirectory">An optional subdirectory specifier for organization.</param>
        internal static void CreateMeshAsset(string gridmapName, MeshChunk targetChunk,
            out Mesh createdMesh, out string meshPath, string subdirectory = "Scenes/GridmapMeshes")
        {
            Mesh mesh = new Mesh();
            mesh.MarkDynamic();

            // Store the mesh files in a subfolder with the gridmap's name (just the scene name probably).
            subdirectory = System.IO.Path.Join(subdirectory, gridmapName);
            string filePath = System.IO.Path.Join(ASSET_FOLDER, subdirectory, gridmapName +
                targetChunk.Position.ToString() + MESH_FILE_EXTENSION);

            // Assign out variables.
            meshPath = filePath;
            createdMesh = mesh;

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(mesh, filePath);
#endif
        }

        #endregion
    }
}
