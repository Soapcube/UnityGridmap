/*****************************************************************************
// File Name : GridmapUtilities.cs
// Author : Brandon Koederitz
// Creation Date : 12/15/2025
// Last Modified : 12/15/2025
//
// Brief Description : Set of static helper functions and core definitions for the Gridmap system.
*****************************************************************************/
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace Gridmap
{
    public static class GridmapUtilities
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
        public static Vector3Int ConvertSwizzleSpace(Vector3Int position, 
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
        public static int Mod(int x, int m)
        {
            return ((x % m) + m) % m;
        }

        #region Chunk Positions
        /// <summary>
        /// Gets the position of the chunk that contains a given grid position in chunk space.
        /// </summary>
        /// <param name="gridPos">The grid position to convert to get the chunk position of.</param>
        public static Vector3Int GetChunkPos(Vector3Int gridPos, Vector3Int chunkSize)
        {
            //Vector3Int chunkPosition = gridPos;
            ////Suprisingly, you can get the index of a Vector3
            ////This function floors this to the position of Tile0 in a chunk
            //for (int index = 0; index < 3; index++)
            //{
            //    float thingToFloor = chunkPosition[index] / (float)chunkSize[index];
            //    chunkPosition[index] = Mathf.FloorToInt(thingToFloor) * chunkSize[index];
            //}
            //return chunkPosition;

            // Switching to a chunk position in 1 by 1 instead of 16 by 16 for array storage.
            Vector3Int chunkPos = Vector3Int.zero;
            for (int i = 0; i < 3; i++)
            {
                float thingToFloor = gridPos[i] / (float)chunkSize[i];
                chunkPos[i] = Mathf.FloorToInt(thingToFloor);
            }
            return chunkPos;
        }

        /// <summary>
        /// Gets the actual local position of a chunk based on it's position in chunk space.
        /// </summary>
        /// <param name="chunkPos">The position of the chunk in chunk space.</param>
        /// <param name="chunkSize">The size of chunks chunk.</param>
        /// <returns>The local position of the chunk.</returns>
        public static Vector3 GetChunkLocalPosition(Vector3Int chunkPos, Vector3Int chunkSize)
        {
            for(int i = 0; i < 3; i++)
            {
                chunkPos[i] *= chunkSize[i];
            }
            return chunkPos;
        }

        /// <summary>
        /// Gets the relative position of a certain grid position witin a given chunk.
        /// </summary>
        /// <param name="gridPos"></param>
        /// <returns></returns>
        public static Vector3Int GetChunkRelativePos(Vector3Int gridPos, Vector3Int chunkSize)
        {
            //Wrap around to our tilemap, so we don't get out of range exceptions
            //This might be a bad idea but we'll see
            for (int i = 0; i < 3; i++)
            {
                gridPos[i] = GridmapUtilities.Mod(gridPos[i], chunkSize[i]);
            }
            return gridPos;
        }

        /// <summary>
        /// Gets the absolute grid position of a cell based on it's relative position within a chunk and that chunk's 
        /// position in chunk space..
        /// </summary>
        /// <param name="relativePos">The relative position of the cell in the chunk.</param>
        /// <param name="chunkPos"></param>
        /// <param name="chunkSize"></param>
        /// <returns></returns>
        public static Vector3Int GetGridPositionFromChunk(Vector3Int relativePos, Vector3Int chunkPos, 
            Vector3Int chunkSize)
        {
            Vector3Int chunkGridPos = Vector3Int.zero;
            for (int i = 0; i < 3; i++)
            {
                chunkGridPos[i] = chunkPos[i] * chunkSize[i];
            }
            return chunkGridPos + relativePos;
        }
        #endregion


        #region Vector - Index
        /// <summary>
        /// Gets the index of a certain position in a chunk given the chunk's size.
        /// </summary>
        /// <param name="pos">The position within the chunk.</param>
        /// <param name="size">The size of teh chunk.</param>
        /// <returns>The index of that cell in the chunk.</returns>
        public static int GetIndexFromPosition(Vector3Int pos, Vector3Int size)
        {
            //Wrap around to our tilemap, so we don't get out of range exceptions
            //This might be a bad idea but we'll see
            pos.x %= size.x;
            pos.y %= size.y;
            pos.z %= size.z;

            int index = pos.x + (pos.y * size.x) + (pos.z * size.x * size.y);

            return index;
        }

        /// <summary>
        /// Converts an index of an array into a 3D position based on the size of the cube the array represents.
        /// </summary>
        /// <param name="index">The array index of the position to find.</param>
        /// <param name="size">The size of the cube area that the array represents.</param>
        /// <returns>The position of the cell represented by the index.</returns>
        public static Vector3Int GetPositionFromIndex(int index, Vector3Int size)
        {
            Vector3Int offset = Vector3Int.zero;

            offset.x = index % size.x;

            index -= offset.x;
            index /= size.x;

            offset.y = index % size.y;

            index -= offset.y;
            index /= size.y;
            offset.z = index;

            return offset;
        }
        #endregion

        #region Mesh Management
        /// <summary>
        /// Creates a mesh asset in the project's assets folder to save the baked mesh data.
        /// </summary>
        /// <param name="gridmapName"> The name to use to identify the meshes associated with a given gridmap.</param>
        /// <param name="targetChunk">The chunk that this mesh will belong to.</param>
        /// <param name="createdMesh">The created mesh.</param>
        /// <param name="meshPath">The path in the assets folder that the mesh was saved to.</param>
        /// <param name="subdirectory">An optional subdirectory specifier for organization.</param>
        //        internal static void CreateMeshAsset(string gridmapName, MeshChunk targetChunk,
        //            out Mesh createdMesh, out string meshPath, string subdirectory = "Scenes/GridmapMeshes")
        //        {
        //            Mesh mesh = new Mesh();
        //            mesh.MarkDynamic();

        //            // Store the mesh files in a subfolder with the gridmap's name (just the scene name probably).
        //            subdirectory = System.IO.Path.Join(subdirectory, gridmapName);
        //            string filePath = System.IO.Path.Join(ASSET_FOLDER, subdirectory, gridmapName +
        //                targetChunk.Position.ToString() + MESH_FILE_EXTENSION);

        //            // Assign out variables.
        //            meshPath = filePath;
        //            createdMesh = mesh;

        //#if UNITY_EDITOR
        //            UnityEditor.AssetDatabase.CreateAsset(mesh, filePath);
        //#endif
        //        }

        #endregion
    }
}
