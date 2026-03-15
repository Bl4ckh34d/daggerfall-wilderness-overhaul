using UnityEngine;
using System.Collections.Generic;
using DaggerfallWorkshop;

namespace WildernessOverhaul {

    public class ContainerObject {
        public DaggerfallTerrain dfTerrain;
        public DaggerfallBillboardBatch dfBillboardBatch;
        public Terrain terrain;
        public float scale;
        public float steepness;
        public int x;
        public int y;
        public int terrainDist;
        public float maxTerrainHeight;

        public ContainerObject(
          DaggerfallTerrain DFTerrain,
          DaggerfallBillboardBatch DFBillboardBatch,
          Terrain Terrain,
          float Scale,
          float Steepness,
          int X,
          int Y,
          int TerrainDist,
          float MaxTerrainHeight)
        {
            dfTerrain = DFTerrain;
            dfBillboardBatch = DFBillboardBatch;
            terrain = Terrain;
            scale = Scale;
            steepness = Steepness;
            x = X;
            y = Y;
            terrainDist = TerrainDist;
            maxTerrainHeight = MaxTerrainHeight;
        }
    }
}
