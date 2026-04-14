/*****************************************************************************
// File Name : ChunkDictionary.cs
// Author : Arcadia Koederitz
// Creation Date : 12/14/2025
// Last Modified : 12/28/2025
//
// Brief Description : 3D tile based system for creating 3D envirobnments from multiple mesh tiles.
*****************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gridmap
{
    [Serializable]
    public class ChunkDictionary : Dictionary<Vector3Int, MeshChunk>, ISerializationCallbackReceiver
    {
        [SerializeField] private Vector3Int[] keys;
        [SerializeField] private MeshChunk[] values;

        private bool deserialized;

        /// <summary>
        /// Converts the dictionary to serialized arrays to save the data.
        /// </summary>
        public void OnBeforeSerialize()
        {
            if (!deserialized) { return; }
            keys = new Vector3Int[this.Count];
            values = new MeshChunk[this.Count];
            for(int i = 0; i < this.Count; i++)
            {
                keys[i] = this.Keys.ElementAt(i);
                values[i] = this.Values.ElementAt(i);
            }
        } 

        /// <summary>
        /// Re-converts the serialized arrays to dictionary values.
        /// </summary>
        public void OnAfterDeserialize()
        {
            this.Clear();

            if (keys ==null || values ==null)
            {
                Debug.LogWarning("A serialized ChunkDictionary had a null keys or values array.");
                return;
            }
            // Error checking here.
            if (keys.Length != values.Length)
            {
                throw new Exception($"There are {keys.Length} key and {values.Length} chunks after deserialization.");
            }

            for(int i = 0; i < keys.Length; i++)
            {
                this.Add(keys[i], values[i]);
            }

            deserialized = true;
        }
    }
}
