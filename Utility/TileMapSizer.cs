using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RetroGlad
{
    public class TileMapSizer : MonoBehaviour
    {
        public Tilemap Tilemap;
        public Vector3Int Size;

        private void OnValidate()
        {
            Tilemap = GetComponent<Tilemap>();
            Size = Tilemap.size;
        }
    }
}
