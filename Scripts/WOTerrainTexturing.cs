// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Hazelnut, Daniel87
//
// Notes:
//

using UnityEngine;
using System;
using DaggerfallConnect.Arena2;
using Unity.Jobs;
using Unity.Collections;
using DaggerfallWorkshop;

namespace WildernessOverhaul
{
    public class WOTerrainTexturing : ITerrainTexturing
    {
        // Use same seed to ensure continuous tiles
        const int seed = 417028;
        const byte water = 0;
        const byte dirt = 1;
        const byte grass = 2;
        const byte stone = 3;

        static bool interestingTerrainEnabled;

        // Order: Deser1, Desert2, Mountains, Rainforest, Swamp,
        // Subtropics, Mountain Woods, Woodland, Haunted Woods, Ocean
        static float[] frequency = {0.02f, 0.02f, 0.025f, 0.035f, 0.02f, 0.02f, 0.035f, 0.035f, 0.035f, 0.1f};
        static float[] amplitude = {0.3f, 0.3f, 0.3f, 0.4f, 0.3f, 0.3f, 0.4f, 0.4f, 0.4f, 0.95f};
        static float[] persistance = {0.5f, 0.55f, 0.95f, 0.8f, 0.5f, 0.5f, 0.8f, 0.8f, 0.8f, 0.3f};
        static int octaves = 5;
        static float[] upperWaterSpread = {-1.0f, -1.0f, 0.0f, 0.0f, -1.0f, -1.0f, 0.0f, 0.0f, 0.0f, 0.0f};
        static float[] lowerGrassSpread = {0.4f, 0.35f, 0.35f, 0.35f, 0.4f, 0.4f, 0.35f, 0.35f, 0.35f, 0.35f};
        static float[] upperGrassSpread = {0.5f, 0.5f, 0.95f, 0.95f, 0.5f, 0.5f, 0.95f, 0.95f, 0.95f, 0.95f};

        public static float treeLine = UnityEngine.Random.Range(0.815f, 0.835f);

        protected static readonly int tileDataDim = MapsFile.WorldMapTileDim + 1;
        protected static readonly int assignTilesDim = MapsFile.WorldMapTileDim;

        protected byte[] lookupTable;
        static TileObject[] tileTable;
        static MapPixelData currentMapData;

        public WOTerrainTexturing(
            bool ITEnabled)
        {
            interestingTerrainEnabled = ITEnabled;
            CreateLookupTable();
        }

        public virtual JobHandle ScheduleAssignTilesJob(ITerrainSampler terrainSampler, ref MapPixelData mapData, JobHandle dependencies, bool march = true)
        {
            // Cache tile data to minimise noise sampling during march.
            NativeArray<byte> tileData = new NativeArray<byte>(tileDataDim * tileDataDim, Allocator.TempJob);
            currentMapData = mapData;

            GenerateTileDataJob tileDataJob = new GenerateTileDataJob
            {
                heightmapData = mapData.heightmapData,
                tileData = tileData,
                tdDim = tileDataDim,
                hDim = terrainSampler.HeightmapDimension,
                maxTerrainHeight = terrainSampler.MaxTerrainHeight,
                oceanElevation = terrainSampler.OceanElevation,
                beachElevation = terrainSampler.BeachElevation,
                mapPixelX = mapData.mapPixelX,
                mapPixelY = mapData.mapPixelY,
                worldClimate = mapData.worldClimate,
            };
            JobHandle tileDataHandle = tileDataJob.Schedule(tileDataDim * tileDataDim, 64, dependencies);

            // Assign tile data to terrain
            NativeArray<byte> lookupData = new NativeArray<byte>(lookupTable, Allocator.TempJob);
            AssignTilesJob assignTilesJob = new AssignTilesJob
            {
                lookupTable = lookupData,
                tileData = tileData,
                tilemapData = mapData.tilemapData,
                tdDim = tileDataDim,
                tDim = assignTilesDim,
                march = march,
                locationRect = mapData.locationRect,
            };
            JobHandle assignTilesHandle = assignTilesJob.Schedule(assignTilesDim * assignTilesDim, 64, tileDataHandle);

            // Add both working native arrays to disposal list.
            mapData.nativeArrayList.Add(tileData);
            mapData.nativeArrayList.Add(lookupData);

            return assignTilesHandle;

        }

        protected struct AssignTilesJob : IJobParallelFor
        {
            [ReadOnly]
            public NativeArray<byte> tileData;
            [ReadOnly]
            public NativeArray<byte> lookupTable;

            public NativeArray<byte> tilemapData;

            public int tdDim;
            public int tDim;
            public bool march;
            public Rect locationRect;



            public void Execute(int index)
            {
                int x = JobA.Row(index, tDim);
                int y = JobA.Col(index, tDim);

                // Do nothing if in location rect as texture already set, to 0xFF if zero
                if (tilemapData[index] != 0)
                    return;

                // Assign tile texture
                if (march)
                {
                    if(!currentMapData.hasLocation) {

                        int bl = tileData[JobA.Idx(x, y, tdDim)];
                        int br = tileData[JobA.Idx(x + 1, y, tdDim)];
                        int tr = tileData[JobA.Idx(x + 1, y + 1, tdDim)];
                        int tl = tileData[JobA.Idx(x, y + 1, tdDim)];

                        tilemapData[index] = tileTable[FindTileIndex(bl, br, tr, tl)].Tile;

                    } else {

                        int tdIdx = JobA.Idx(x, y, tdDim);
                        int b0 = tileData[tdIdx];               // tileData[x, y]
                        int b1 = tileData[tdIdx + 1];           // tileData[x + 1, y]
                        int b2 = tileData[tdIdx + tdDim];       // tileData[x, y + 1]
                        int b3 = tileData[tdIdx + tdDim + 1];   // tileData[x + 1, y + 1]
                        int shape = (b0 & 1) | (b1 & 1) << 1 | (b2 & 1) << 2 | (b3 & 1) << 3;
                        int ring = (b0 + b1 + b2 + b3) >> 2;
                        int tileID = shape | ring << 4;

                        tilemapData[index] = lookupTable[tileID];
                    }
                }
                else
                {
                    tilemapData[index] = tileData[JobA.Idx(x, y, tdDim)];
                }
            }
        }

        protected struct GenerateTileDataJob : IJobParallelFor
        {
            [ReadOnly]
            public NativeArray<float> heightmapData;

            public NativeArray<byte> tileData;

            public int hDim;
            public int tdDim;
            public int tDim;
            public float maxTerrainHeight;
            public float oceanElevation;
            public float beachElevation;
            public int mapPixelX;
            public int mapPixelY;
            public int worldClimate;

            // Gets noise value
            private float NoiseWeight(float worldX, float worldY, float height)
            {
                float persistanceRnd = 0.95f;
                int climateNum = 9;
                switch (worldClimate) {
                    case (int)MapsFile.Climates.Desert:
                        climateNum = 0;
                        persistanceRnd = persistance[climateNum] + ((height / maxTerrainHeight) * 2) - 0.25f;
                        break;
                    case (int)MapsFile.Climates.Desert2:
                        climateNum = 1;
                        persistanceRnd = persistance[climateNum] + ((height / maxTerrainHeight) * 2) - 0.25f;
                        break;
                    case (int)MapsFile.Climates.Mountain:
                        climateNum = 2;
                        if ((height / maxTerrainHeight) + JobRand.Next(-5, 5) / 1000f > treeLine)
                            persistanceRnd = persistance[climateNum] - treeLine + ((height / maxTerrainHeight) * 1.2f);
                        else
                            persistanceRnd = persistance[climateNum] - (1 - (height / maxTerrainHeight)) * 0.4f;
                        break;
                    case (int)MapsFile.Climates.Rainforest:
                        climateNum = 3;
                        persistanceRnd = persistance[climateNum] + ((height / maxTerrainHeight) * 1.5f) - 0.35f;
                        break;
                    case (int)MapsFile.Climates.Swamp:
                        climateNum = 4;
                        persistanceRnd = persistance[climateNum] + ((height / maxTerrainHeight) * 3) - 0.25f;
                        break;
                    case (int)MapsFile.Climates.Subtropical:
                        climateNum = 5;
                        persistanceRnd = persistance[climateNum] + ((height / maxTerrainHeight) * 2) - 0.25f;
                        break;
                    case (int)MapsFile.Climates.MountainWoods:
                        climateNum = 6;
                        persistanceRnd = persistance[climateNum] + ((height / maxTerrainHeight) * 1.5f) - 0.35f;
                        break;
                    case (int)MapsFile.Climates.Woodlands:
                        climateNum = 7;
                        if (interestingTerrainEnabled)
                            persistanceRnd = 0.6f + ((height / maxTerrainHeight) * 2f); //persistance[climateNum] + ((height / maxTerrainHeight) * 2f) - 0.20f;
                        else
                            persistanceRnd = persistance[climateNum] + ((height / maxTerrainHeight) * 1.4f) - 0.30f;
                        break;
                    case (int)MapsFile.Climates.HauntedWoodlands:
                        climateNum = 8;
                        persistanceRnd = persistance[climateNum] + ((height / maxTerrainHeight) * 1.5f) - 0.35f;
                        break;
                    case (int)MapsFile.Climates.Ocean:
                        climateNum = 9;
                        persistanceRnd = persistance[climateNum] + ((height / maxTerrainHeight) * 1.5f) - 0.35f;
                        break;
                }
                return GetNoise(worldX, worldY, frequency[climateNum], amplitude[climateNum], persistanceRnd, octaves, seed);
            }

            // Sets texture by range
            private byte GetWeightedRecord(float weight, float upperWaterSpread = 0.0f, float lowerGrassSpread = 0.35f, float upperGrassSpread = 0.95f)
            {
                if (weight < upperWaterSpread)
                    return water;
                else if (weight >= upperWaterSpread && weight < lowerGrassSpread)
                    return dirt;
                else if (weight >= lowerGrassSpread && weight < upperGrassSpread)
                    return grass;
                else
                    return stone;
            }

            // Noise function
            private float GetNoise(
              float x,
              float y,
              float frequency,
              float amplitude,
              float persistance,
              int octaves,
              int seed = 0)
            {
                float finalValue = 0f;
                for (int i = 0; i < octaves; ++i)
                {
                    finalValue += Mathf.PerlinNoise(seed + (x * frequency), seed + (y * frequency)) * amplitude;
                    frequency *= 2.0f;
                    amplitude *= persistance;
                }
                return Mathf.Clamp(finalValue, -1, 1);
            }

            public void Execute(int index)
            {
                int x = JobA.Row(index, tdDim);
                int y = JobA.Col(index, tdDim);
                int uB = heightmapData.Length;

                if (interestingTerrainEnabled)
                    maxTerrainHeight = 4890;

                // Height sample for ocean and beach tiles
                int hx = (int)Mathf.Clamp(hDim * ((float)x / (float)tdDim), 0, hDim - 1);
                int hy = (int)Mathf.Clamp(hDim * ((float)y / (float)tdDim), 0, hDim - 1);
                float height = heightmapData[JobA.Idx(hy, hx, hDim)] * maxTerrainHeight;

                // Ocean and Beach texture
                if (height <= oceanElevation) {
                    tileData[index] = water;
                    return;
                }
                // Adds a little +/- randomness to threshold so beach line isn't too regular
                if (height <= beachElevation + (JobRand.Next(-100000000, -55000000) / 10000000f)) {
                    tileData[index] = dirt;
                    return;
                }

                // Get latitude and longitude of this tile
                int latitude = (int)(mapPixelX * MapsFile.WorldMapTileDim + x);
                int longitude = (int)(MapsFile.MaxWorldTileCoordZ - mapPixelY * MapsFile.WorldMapTileDim + y);

                // Set texture tile using weighted noise
                float weight = 0;
                if (interestingTerrainEnabled)
                    weight += NoiseWeight(latitude, longitude, height/1.05f);
                else
                    weight += NoiseWeight(latitude, longitude, height);


                int climateNum = 9;
                switch (worldClimate)
                {
                    case (int)MapsFile.Climates.Desert:
                        climateNum = 0;
                        break;
                    case (int)MapsFile.Climates.Desert2:
                        climateNum = 1;
                        break;
                    case (int)MapsFile.Climates.Mountain:
                        climateNum = 2;
                        break;
                    case (int)MapsFile.Climates.Rainforest:
                        climateNum = 3;
                        break;
                    case (int)MapsFile.Climates.Swamp:
                        climateNum = 4;
                        break;
                    case (int)MapsFile.Climates.Subtropical:
                        climateNum = 5;
                        break;
                    case (int)MapsFile.Climates.MountainWoods:
                        climateNum = 6;
                        break;
                    case (int)MapsFile.Climates.Woodlands:
                        climateNum = 7;
                        break;
                    case (int)MapsFile.Climates.HauntedWoodlands:
                        climateNum = 8;
                        break;
                    case (int)MapsFile.Climates.Ocean:
                        climateNum = 9;
                        break;
                }
                tileData[index] = GetWeightedRecord(weight, upperWaterSpread[climateNum], lowerGrassSpread[climateNum], upperGrassSpread[climateNum]);

                // Check for lowest local point in desert to place oasis
                if (worldClimate == (int)MapsFile.Climates.Desert2 && LowestPointFound(30, heightmapData, maxTerrainHeight, hx, hy, hDim, uB, index, tdDim, tileData))
                    tileData[index] = water;
                if (worldClimate == (int)MapsFile.Climates.Desert && LowestPointFound(80, heightmapData, maxTerrainHeight, hx, hy, hDim, uB, index, tdDim, tileData))
                    tileData[index] = water;

                // Rock Mountain Face
                if (SteepnessWithinLimits(true, Mathf.Clamp(90f - ((height / maxTerrainHeight)/0.85f * 100f),40f,90f), heightmapData, maxTerrainHeight, hx, hy, hDim, uB, index, tdDim, tileData))
                    tileData[index] = stone;

                int rnd = JobRand.Next(25,35);
                if (tileData[index] == stone && SteepnessWithinLimits(false, Mathf.Clamp(90f - ((height / maxTerrainHeight)/0.85f * 100f),40f,90f), heightmapData, maxTerrainHeight, hx, hy, hDim, uB, index, tdDim, tileData))
                    tileData[index] = grass;

                // Max angle for dirt patches
                rnd = JobRand.Next(20,25);
                if (tileData[index] == dirt && SteepnessWithinLimits(false, rnd, heightmapData, maxTerrainHeight, hx, hy, hDim, uB, index, tdDim, tileData))
                    tileData[index] = dirt;
                else if (tileData[index] == dirt && !SteepnessWithinLimits(false, rnd, heightmapData, maxTerrainHeight, hx, hy, hDim, uB, index, tdDim, tileData))
                    tileData[index] = grass;
            }
        }

        // Creates lookup table
        void CreateLookupTable()
        {
            lookupTable = new byte[64];
            AddLookupRange(0, 1, 5, 48, false, 0);
            AddLookupRange(2, 1, 10, 51, true, 16);
            AddLookupRange(2, 3, 15, 53, false, 32);
            AddLookupRange(3, 3, 15, 53, true, 48);

            tileTable = new TileObject[114];
            // Water - Dirt
            tileTable[0]   = new TileObject(MakeLookup(0, false, false),  0,0,0,0);

            tileTable[1]   = new TileObject(MakeLookup(4, true, false),   1,0,0,0);
            tileTable[2]   = new TileObject(MakeLookup(4, false, true),   0,1,0,0);
            tileTable[3]   = new TileObject(MakeLookup(4, true, true),    0,0,1,0);
            tileTable[4]   = new TileObject(MakeLookup(4, false, false),  0,0,0,1);

            tileTable[5]   = new TileObject(MakeLookup(5, true, false),   1,1,0,0);
            tileTable[6]   = new TileObject(MakeLookup(5, false, true),   0,1,1,0);
            tileTable[7]   = new TileObject(MakeLookup(5, true, true),    0,0,1,1);
            tileTable[8]   = new TileObject(MakeLookup(5, false, false),  1,0,0,1);

            tileTable[9]   = new TileObject(MakeLookup(6, false, true),   1,1,1,0);
            tileTable[10]  = new TileObject(MakeLookup(6, true, true),    0,1,1,1);
            tileTable[11]  = new TileObject(MakeLookup(6, false, false),  1,0,1,1);
            tileTable[12]  = new TileObject(MakeLookup(6, true, false),   1,1,0,1);

            tileTable[13]  = new TileObject(MakeLookup(21, false, false), 0,1,0,1);
            tileTable[14]  = new TileObject(MakeLookup(21, true, false),  1,0,1,0);

            tileTable[15]  = new TileObject(MakeLookup(1, false, false),  1,1,1,1);

            // Dirt - Grass
            tileTable[16]  = new TileObject(MakeLookup(7, true, false),   2,1,1,1);
            tileTable[17]  = new TileObject(MakeLookup(7, false, true),   1,2,1,1);
            tileTable[18]  = new TileObject(MakeLookup(7, true, true),    1,1,2,1);
            tileTable[19]  = new TileObject(MakeLookup(7, false, false),  1,1,1,2);

            tileTable[20]  = new TileObject(MakeLookup(8, true, false),   2,2,1,1);
            tileTable[21]  = new TileObject(MakeLookup(8, false, true),   1,2,2,1);
            tileTable[22]  = new TileObject(MakeLookup(8, true, true),    1,1,2,2);
            tileTable[23]  = new TileObject(MakeLookup(8, false, false),  2,1,1,2);

            tileTable[24]  = new TileObject(MakeLookup(9, false, true),   2,2,2,1);
            tileTable[25]  = new TileObject(MakeLookup(9, true, true),    1,2,2,2);
            tileTable[26]  = new TileObject(MakeLookup(9, false, false),  2,1,2,2);
            tileTable[27]  = new TileObject(MakeLookup(9, true, false),   2,2,1,2);

            tileTable[28]  = new TileObject(MakeLookup(22, true, false),  2,1,2,1);
            tileTable[29]  = new TileObject(MakeLookup(22, false, false), 1,2,1,2);

            tileTable[30]  = new TileObject(MakeLookup(2, false, false),  2,2,2,2);

            // Grass - Stone
            tileTable[31]  = new TileObject(MakeLookup(10, true, false),  3,2,2,2);
            tileTable[32]  = new TileObject(MakeLookup(10, false, true),  2,3,2,2);
            tileTable[33]  = new TileObject(MakeLookup(10, true, true),   2,2,3,2);
            tileTable[34]  = new TileObject(MakeLookup(10, false, false), 2,2,2,3);

            tileTable[35]  = new TileObject(MakeLookup(11, true, false),  3,3,2,2);
            tileTable[36]  = new TileObject(MakeLookup(11, false, true),  2,3,3,2);
            tileTable[37]  = new TileObject(MakeLookup(11, true, true),   2,2,3,3);
            tileTable[38]  = new TileObject(MakeLookup(11, false, false), 3,2,2,3);

            tileTable[39]  = new TileObject(MakeLookup(12, false, true),  3,3,3,2);
            tileTable[40]  = new TileObject(MakeLookup(12, true, true),   2,3,3,3);
            tileTable[41]  = new TileObject(MakeLookup(12, false, false), 3,2,3,3);
            tileTable[42]  = new TileObject(MakeLookup(12, true, false),  3,3,2,3);

            tileTable[43]  = new TileObject(MakeLookup(24, true, false),  3,2,3,2);
            tileTable[44]  = new TileObject(MakeLookup(24, false, false), 2,3,2,3);

            tileTable[45]  = new TileObject(MakeLookup(3, false, false),  3,3,3,3);

            // Dirt - Stone
            tileTable[46]  = new TileObject(MakeLookup(13, true, false),  3,1,1,1);
            tileTable[47]  = new TileObject(MakeLookup(13, false, true),  1,3,1,1);
            tileTable[48]  = new TileObject(MakeLookup(13, true, true),   1,1,3,1);
            tileTable[49]  = new TileObject(MakeLookup(13, false, false), 1,1,1,3);

            tileTable[50]  = new TileObject(MakeLookup(14, true, false),  3,3,1,1);
            tileTable[51]  = new TileObject(MakeLookup(14, false, true),  1,3,3,1);
            tileTable[52]  = new TileObject(MakeLookup(14, true, true),   1,1,3,3);
            tileTable[53]  = new TileObject(MakeLookup(14, false, false), 3,1,1,3);

            tileTable[54]  = new TileObject(MakeLookup(15, false, true),  3,3,3,1);
            tileTable[55]  = new TileObject(MakeLookup(15, true, true),   1,3,3,3);
            tileTable[56]  = new TileObject(MakeLookup(15, false, false), 3,1,3,3);
            tileTable[57]  = new TileObject(MakeLookup(15, true, false),  3,3,1,3);

            tileTable[58]  = new TileObject(MakeLookup(23, true, false),  3,1,3,1);
            tileTable[59]  = new TileObject(MakeLookup(23, false, false), 1,3,1,3);

            //Dirt - Grass - Stone
            tileTable[60]  = new TileObject(MakeLookup(16, true, false),  1,1,2,3);
            tileTable[61]  = new TileObject(MakeLookup(16, false, true),  3,1,1,2);
            tileTable[62]  = new TileObject(MakeLookup(16, true, true),   2,3,1,1);
            tileTable[63]  = new TileObject(MakeLookup(16, false, false), 1,2,3,1);

            tileTable[64]  = new TileObject(MakeLookup(27, true, false),  1,1,3,2);
            tileTable[65]  = new TileObject(MakeLookup(27, false, true),  2,1,1,3);
            tileTable[66]  = new TileObject(MakeLookup(27, true, true),   3,2,1,1);
            tileTable[67]  = new TileObject(MakeLookup(27, false, false), 1,3,2,1);

            tileTable[68]  = new TileObject(MakeLookup(17, true, false),  2,2,1,3);
            tileTable[69]  = new TileObject(MakeLookup(17, false, true),  3,2,2,1);
            tileTable[70]  = new TileObject(MakeLookup(17, true, true),   1,3,2,2);
            tileTable[71]  = new TileObject(MakeLookup(17, false, false), 2,1,3,2);

            tileTable[72]  = new TileObject(MakeLookup(28, true, false),  2,2,3,1);
            tileTable[73]  = new TileObject(MakeLookup(28, false, true),  1,2,2,3);
            tileTable[74]  = new TileObject(MakeLookup(28, true, true),   3,1,2,2);
            tileTable[75]  = new TileObject(MakeLookup(28, false, false), 2,3,1,2);

            tileTable[76]  = new TileObject(MakeLookup(18, true, false),  3,3,1,2);
            tileTable[77]  = new TileObject(MakeLookup(18, false, true),  2,3,3,1);
            tileTable[78]  = new TileObject(MakeLookup(18, true, true),   1,2,3,3);
            tileTable[79]  = new TileObject(MakeLookup(18, false, false), 3,1,2,3);

            tileTable[80]  = new TileObject(MakeLookup(29, true, false),  3,3,2,1);
            tileTable[81]  = new TileObject(MakeLookup(29, false, true),  1,3,3,2);
            tileTable[82]  = new TileObject(MakeLookup(29, true, true),   2,1,3,3);
            tileTable[83]  = new TileObject(MakeLookup(29, false, false), 3,2,1,3);

            //Dirt - Stone & Grass
            tileTable[84]  = new TileObject(MakeLookup(30, true, false),  3,1,2,1);
            tileTable[85]  = new TileObject(MakeLookup(30, false, true),  1,3,1,2);
            tileTable[86]  = new TileObject(MakeLookup(30, true, true),   2,1,3,1);
            tileTable[87]  = new TileObject(MakeLookup(30, false, false), 1,2,1,3);

            //Dirt - Water & Grass
            tileTable[88]  = new TileObject(MakeLookup(31, true, false),  2,1,0,1);
            tileTable[89]  = new TileObject(MakeLookup(31, false, true),  1,2,1,0);
            tileTable[90]  = new TileObject(MakeLookup(31, true, true),   0,1,2,1);
            tileTable[91]  = new TileObject(MakeLookup(31, false, false), 1,0,1,2);

            //Dirt - Water & Stone
            tileTable[92]  = new TileObject(MakeLookup(32, true, false),  3,1,0,1);
            tileTable[93]  = new TileObject(MakeLookup(32, false, true),  1,3,1,0);
            tileTable[94]  = new TileObject(MakeLookup(32, true, true),   0,1,3,1);
            tileTable[95]  = new TileObject(MakeLookup(32, false, false), 1,0,1,3);

            //Grass - Dirt & Stone
            tileTable[96]  = new TileObject(MakeLookup(33, true, false),  3,2,1,2);
            tileTable[97]  = new TileObject(MakeLookup(33, false, true),  2,3,2,1);
            tileTable[98]  = new TileObject(MakeLookup(33, true, true),   1,2,3,2);
            tileTable[99]  = new TileObject(MakeLookup(33, false, false), 2,1,2,3);

            //Stone - Dirt & Grass
            tileTable[100] = new TileObject(MakeLookup(34, true, false),  2,3,1,3);
            tileTable[101] = new TileObject(MakeLookup(34, false, true),  3,2,3,1);
            tileTable[102] = new TileObject(MakeLookup(34, true, true),   1,3,2,3);
            tileTable[103] = new TileObject(MakeLookup(34, false, false), 3,1,3,2);

            //Road - Dirt & Grass
            tileTable[105] = new TileObject(MakeLookup(19, false, false), 19,19,19,19);

            tileTable[106] = new TileObject(MakeLookup(20, true, false),  1,20,19,20);
            tileTable[107] = new TileObject(MakeLookup(20, false, true),  20,1,20,19);
            tileTable[108] = new TileObject(MakeLookup(20, true, true),   19,20,1,20);
            tileTable[109] = new TileObject(MakeLookup(20, false, false), 20,19,20,1);

            tileTable[110] = new TileObject(MakeLookup(26, true, false),  2,26,19,26);
            tileTable[111] = new TileObject(MakeLookup(26, false, true),  26,2,26,19);
            tileTable[112] = new TileObject(MakeLookup(26, true, true),   19,26,2,26);
            tileTable[113] = new TileObject(MakeLookup(26, false, false), 26,19,26,2);
        }

        // Adds range of 16 values to lookup table
        void AddLookupRange(int baseStart, int baseEnd, int shapeStart, int saddleIndex, bool reverse, int offset)
        {
            if (reverse)
            {
                // high > low
                lookupTable[offset] = MakeLookup2(baseStart, false, false);
                lookupTable[offset + 1] = MakeLookup2(shapeStart + 2, true, true);
                lookupTable[offset + 2] = MakeLookup2(shapeStart + 2, false, false);
                lookupTable[offset + 3] = MakeLookup2(shapeStart + 1, true, true);
                lookupTable[offset + 4] = MakeLookup2(shapeStart + 2, false, true);
                lookupTable[offset + 5] = MakeLookup2(shapeStart + 1, false, true);
                lookupTable[offset + 6] = MakeLookup2(saddleIndex, true, false); //d
                lookupTable[offset + 7] = MakeLookup2(shapeStart, true, true);
                lookupTable[offset + 8] = MakeLookup2(shapeStart + 2, true, false);
                lookupTable[offset + 9] = MakeLookup2(saddleIndex, false, false); //d
                lookupTable[offset + 10] = MakeLookup2(shapeStart + 1, false, false);
                lookupTable[offset + 11] = MakeLookup2(shapeStart, false, false);
                lookupTable[offset + 12] = MakeLookup2(shapeStart + 1, true, false);
                lookupTable[offset + 13] = MakeLookup2(shapeStart, false, true);
                lookupTable[offset + 14] = MakeLookup2(shapeStart, true, false);
                lookupTable[offset + 15] = MakeLookup2(baseEnd, false, false);
            }
            else
            {
                // low > high
                lookupTable[offset] = MakeLookup2(baseStart, false, false);
                lookupTable[offset + 1] = MakeLookup2(shapeStart, true, false);
                lookupTable[offset + 2] = MakeLookup2(shapeStart, false, true);
                lookupTable[offset + 3] = MakeLookup2(shapeStart + 1, true, false);
                lookupTable[offset + 4] = MakeLookup2(shapeStart, false, false);
                lookupTable[offset + 5] = MakeLookup2(shapeStart + 1, false, false);
                lookupTable[offset + 6] = MakeLookup2(saddleIndex, false, false); //d
                lookupTable[offset + 7] = MakeLookup2(shapeStart + 2, true, false);
                lookupTable[offset + 8] = MakeLookup2(shapeStart, true, true);
                lookupTable[offset + 9] = MakeLookup2(saddleIndex, true, false); //d
                lookupTable[offset + 10] = MakeLookup2(shapeStart + 1, false, true);
                lookupTable[offset + 11] = MakeLookup2(shapeStart + 2, false, true);
                lookupTable[offset + 12] = MakeLookup2(shapeStart + 1, true, true);
                lookupTable[offset + 13] = MakeLookup2(shapeStart + 2, false, false);
                lookupTable[offset + 14] = MakeLookup2(shapeStart + 2, true, true);
                lookupTable[offset + 15] = MakeLookup2(baseEnd, false, false);
            }
        }

        // Encodes a byte with Daggerfall tile neighbours
        static int FindTileIndex(int bl, int br, int tr, int tl)
        {
            if (Array.FindIndex(tileTable, tile => tile.Bl == bl && tile.Br == br && tile.Tr == tr && tile.Tl == tl) != -1)
                return Array.FindIndex(tileTable, tile => tile.Bl == bl && tile.Br == br && tile.Tr == tr && tile.Tl == tl);
            else
                return 0;
        }

        // Encodes a byte with Daggerfall tile lookup
        byte MakeLookup(int index, bool rotate, bool flip)
        {
            if (index > 255)
                throw new IndexOutOfRangeException("Index out of range. Valid range 0-255");
            if (rotate) index += 64;
            if (flip) index += 128;

            return (byte)index;
        }

        // Encodes a byte with Daggerfall tile lookup
        byte MakeLookup2(int index, bool rotate, bool flip)
        {
            if (index > 55)
                throw new IndexOutOfRangeException("Index out of range. Valid range 0-55");
            if (rotate) index += 64;
            if (flip) index += 128;

            return (byte)index;
        }

        static bool LowestPointFound(int chance, NativeArray<float> heightmapData, float maxTerrainHeight, int hx, int hy, int hDim, int upperBound, int index, int tdDim, NativeArray<byte> tileData)
        {
            int newChance = (int)(chance - (chance * heightmapData[JobA.Idx(hy, hx, hDim)] * 2));

            if (tileData[index] != dirt ||
              JobA.Row(index, tdDim) - 5 <= 0 ||
              JobA.Col(index, tdDim) + 5 >= tdDim ||
              JobA.Row(index, tdDim) + 5 >= tdDim ||
              JobA.Col(index, tdDim) - 5 <= 0)
            {
                return false;
            }
            else
            {
                float thisHeight = heightmapData[JobA.Idx(hy, hx, hDim)] * maxTerrainHeight;
                if (JobA.Idx(hy - 5, hx - 5, hDim) < 0 ||
                    JobA.Idx(hy - 5, hx - 5, hDim) > upperBound ||
                    JobA.Idx(hy - 5, hx + 5, hDim) < 0 ||
                    JobA.Idx(hy - 5, hx + 5, hDim) > upperBound ||
                    JobA.Idx(hy + 5, hx - 5, hDim) < 0 ||
                    JobA.Idx(hy + 5, hx - 5, hDim) > upperBound ||
                    JobA.Idx(hy + 5, hx + 5, hDim) < 0 ||
                    JobA.Idx(hy + 5, hx + 5, hDim) > upperBound
                    )
                {
                    return false;
                }
                else
                {
                    for (int a = -4; a < 5; a++)
                    {
                        for (int b = -4; b < 5; b++)
                        {
                            if ((a != 0 && b != 0) && heightmapData[JobA.Idx(hy + a, hx + b, hDim)] * maxTerrainHeight < thisHeight + (0.0000035f * Mathf.Abs(a)) + (0.0000035f * Mathf.Abs(b)))
                            {
                                return false;
                            }
                        }
                    }
                    if (JobRand.Next(0, 100) <= newChance)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        static bool SteepnessWithinLimits(bool bigger, float steepness, NativeArray<float> heightmapData, float maxTerrainHeight, int hx, int hy, int hDim, int upperBound, int index, int tdDim, NativeArray<byte> tileData)
        {
            int offsetX = 0;
            int offsetY = 0;
            if (JobA.Col(index, tdDim) + 1 >= tdDim)
            {
                offsetY = -1;
            }
            if (JobA.Row(index, tdDim) + 1 >= tdDim)
            {
                offsetX = -1;
            }

            float minSmpl = 0;
            float maxSmpl = 0;
            float smpl = minSmpl = maxSmpl = heightmapData[JobA.Idx(hy, hx, hDim)] * maxTerrainHeight;
            for (int a = 0; a <= 1; a++)
            {
                for (int b = 0; b <= 1; b++)
                {
                    smpl = heightmapData[JobA.Idx(hy + a + offsetY, hx + b + offsetX, hDim)] * maxTerrainHeight;

                    if (smpl < minSmpl)
                    {
                        minSmpl = smpl;
                    }
                    if (smpl > maxSmpl)
                    {
                        maxSmpl = smpl;
                    }

                }
            }

            float diff = (maxSmpl - minSmpl) * 10f;

            if (bigger) {
                if (diff >= steepness)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            } else {
                if (diff <= steepness)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
