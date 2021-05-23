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
using DaggerfallWorkshop.Game.Utility.ModSupport;

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
        readonly bool basicRoadsEnabled;

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
        static protected TileObject[] tileTable = {
            // Water - Dirt
            new TileObject(MakeLookup(0, false, false),  0,0,0,0),

            new TileObject(MakeLookup(5, true, false),   1,0,0,0),
            new TileObject(MakeLookup(5, false, true),   0,1,0,0),
            new TileObject(MakeLookup(5, true, true),    0,0,1,0),
            new TileObject(MakeLookup(5, false, false),  0,0,0,1),

            new TileObject(MakeLookup(6, true, false),   1,1,0,0),
            new TileObject(MakeLookup(6, false, true),   0,1,1,0),
            new TileObject(MakeLookup(6, true, true),    0,0,1,1),
            new TileObject(MakeLookup(6, false, false),  1,0,0,1),

            new TileObject(MakeLookup(7, false, true),   1,1,1,0),
            new TileObject(MakeLookup(7, true, true),    0,1,1,1),
            new TileObject(MakeLookup(7, false, false),  1,0,1,1),
            new TileObject(MakeLookup(7, true, false),   1,1,0,1),

            new TileObject(MakeLookup(48, false, false), 0,1,0,1),
            new TileObject(MakeLookup(48, true, false),  1,0,1,0),

            new TileObject(MakeLookup(1, false, false),  1,1,1,1),

            // Dirt - Grass
            new TileObject(MakeLookup(10, true, false),   2,1,1,1),
            new TileObject(MakeLookup(10, false, true),   1,2,1,1),
            new TileObject(MakeLookup(10, true, true),    1,1,2,1),
            new TileObject(MakeLookup(10, false, false),  1,1,1,2),

            new TileObject(MakeLookup(11, true, false),   2,2,1,1),
            new TileObject(MakeLookup(11, false, true),   1,2,2,1),
            new TileObject(MakeLookup(11, true, true),    1,1,2,2),
            new TileObject(MakeLookup(11, false, false),  2,1,1,2),

            new TileObject(MakeLookup(12, false, true),   2,2,2,1),
            new TileObject(MakeLookup(12, true, true),    1,2,2,2),
            new TileObject(MakeLookup(12, false, false),  2,1,2,2),
            new TileObject(MakeLookup(12, true, false),   2,2,1,2),

            new TileObject(MakeLookup(51, true, false),  2,1,2,1),
            new TileObject(MakeLookup(51, false, false), 1,2,1,2),

            new TileObject(MakeLookup(2, false, false),  2,2,2,2),

            // Grass - Stone
            new TileObject(MakeLookup(15, true, false),  3,2,2,2),
            new TileObject(MakeLookup(15, false, true),  2,3,2,2),
            new TileObject(MakeLookup(15, true, true),   2,2,3,2),
            new TileObject(MakeLookup(15, false, false), 2,2,2,3),

            new TileObject(MakeLookup(16, true, false),  3,3,2,2),
            new TileObject(MakeLookup(16, false, true),  2,3,3,2),
            new TileObject(MakeLookup(16, true, true),   2,2,3,3),
            new TileObject(MakeLookup(16, false, false), 3,2,2,3),

            new TileObject(MakeLookup(17, false, true),  3,3,3,2),
            new TileObject(MakeLookup(17, true, true),   2,3,3,3),
            new TileObject(MakeLookup(17, false, false), 3,2,3,3),
            new TileObject(MakeLookup(17, true, false),  3,3,2,3),

            new TileObject(MakeLookup(53, true, false),  3,2,3,2),
            new TileObject(MakeLookup(53, false, false), 2,3,2,3),

            new TileObject(MakeLookup(3, false, false),  3,3,3,3),

            // Dirt - Stone
            new TileObject(MakeLookup(25, true, false),  3,1,1,1),
            new TileObject(MakeLookup(25, false, true),  1,3,1,1),
            new TileObject(MakeLookup(25, true, true),   1,1,3,1),
            new TileObject(MakeLookup(25, false, false), 1,1,1,3),

            new TileObject(MakeLookup(26, true, false),  3,3,1,1),
            new TileObject(MakeLookup(26, false, true),  1,3,3,1),
            new TileObject(MakeLookup(26, true, true),   1,1,3,3),
            new TileObject(MakeLookup(26, false, false), 3,1,1,3),

            new TileObject(MakeLookup(27, false, true),  3,3,3,1),
            new TileObject(MakeLookup(27, true, true),   1,3,3,3),
            new TileObject(MakeLookup(27, false, false), 3,1,3,3),
            new TileObject(MakeLookup(27, true, false),  3,3,1,3),

            new TileObject(MakeLookup(52, true, false),  3,1,3,1),
            new TileObject(MakeLookup(52, false, false), 1,3,1,3),

            //Dirt - Grass - Stone
            new TileObject(MakeLookup(39, true, false),  1,1,2,3),
            new TileObject(MakeLookup(39, false, true),  3,1,1,2),
            new TileObject(MakeLookup(39, true, true),   2,3,1,1),
            new TileObject(MakeLookup(39, false, false), 1,2,3,1),

            new TileObject(MakeLookup(56, true, false),  1,1,3,2),
            new TileObject(MakeLookup(56, false, true),  2,1,1,3),
            new TileObject(MakeLookup(56, true, true),   3,2,1,1),
            new TileObject(MakeLookup(56, false, false), 1,3,2,1),

            new TileObject(MakeLookup(42, true, false),  2,2,1,3),
            new TileObject(MakeLookup(42, false, true),  3,2,2,1),
            new TileObject(MakeLookup(42, true, true),   1,3,2,2),
            new TileObject(MakeLookup(42, false, false), 2,1,3,2),

            new TileObject(MakeLookup(57, true, false),  2,2,3,1),
            new TileObject(MakeLookup(57, false, true),  1,2,2,3),
            new TileObject(MakeLookup(57, true, true),   3,1,2,2),
            new TileObject(MakeLookup(57, false, false), 2,3,1,2),

            new TileObject(MakeLookup(45, true, false),  3,3,1,2),
            new TileObject(MakeLookup(45, false, true),  2,3,3,1),
            new TileObject(MakeLookup(45, true, true),   1,2,3,3),
            new TileObject(MakeLookup(45, false, false), 3,1,2,3),

            new TileObject(MakeLookup(58, true, false),  3,3,2,1),
            new TileObject(MakeLookup(58, false, true),  1,3,3,2),
            new TileObject(MakeLookup(58, true, true),   2,1,3,3),
            new TileObject(MakeLookup(58, false, false), 3,2,1,3),

            //Dirt - Stone & Grass
            new TileObject(MakeLookup(59, true, false),  3,1,2,1),
            new TileObject(MakeLookup(59, false, true),  1,3,1,2),
            new TileObject(MakeLookup(59, true, true),   2,1,3,1),
            new TileObject(MakeLookup(59, false, false), 1,2,1,3),

            //Dirt - Water & Grass
            new TileObject(MakeLookup(60, true, false),  2,1,0,1),
            new TileObject(MakeLookup(60, false, true),  1,2,1,0),
            new TileObject(MakeLookup(60, true, true),   0,1,2,1),
            new TileObject(MakeLookup(60, false, false), 1,0,1,2),

            //Dirt - Water & Stone
            new TileObject(MakeLookup(61, true, false),  3,1,0,1),
            new TileObject(MakeLookup(61, false, true),  1,3,1,0),
            new TileObject(MakeLookup(61, true, true),   0,1,3,1),
            new TileObject(MakeLookup(61, false, false), 1,0,1,3),

            //Grass - Dirt & Stone
            new TileObject(MakeLookup(62, true, false),  3,2,1,2),
            new TileObject(MakeLookup(62, false, true),  2,3,2,1),
            new TileObject(MakeLookup(62, true, true),   1,2,3,2),
            new TileObject(MakeLookup(62, false, false), 2,1,2,3),

            //Stone - Dirt & Grass
            new TileObject(MakeLookup(63, true, false),  2,3,1,3),
            new TileObject(MakeLookup(63, false, true),  3,2,3,1),
            new TileObject(MakeLookup(63, true, true),   1,3,2,3),
            new TileObject(MakeLookup(63, false, false), 3,1,3,2),

            //Road - Dirt & Grass
            new TileObject(MakeLookup(46, false, false), 19,19,19,19),

            new TileObject(MakeLookup(47, true, false),  1,20,19,20),
            new TileObject(MakeLookup(47, false, true),  20,1,20,19),
            new TileObject(MakeLookup(47, true, true),   19,20,1,20),
            new TileObject(MakeLookup(47, false, false), 20,19,20,1),

            new TileObject(MakeLookup(55, true, false),  2,26,19,26),
            new TileObject(MakeLookup(55, false, true),  26,2,26,19),
            new TileObject(MakeLookup(55, true, true),   19,26,2,26),
            new TileObject(MakeLookup(55, false, false), 26,19,26,2)
        };

        static MapPixelData currentMapData;

        public WOTerrainTexturing(bool ITEnabled, bool basicRoadsEnabled)
        {
            interestingTerrainEnabled = ITEnabled;
            this.basicRoadsEnabled = basicRoadsEnabled;
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

            // Schedule the paint roads jobs if basic roads mod is enabled
            JobHandle preAssignTilesHandle = tileDataHandle;
            if (basicRoadsEnabled)
            {
                ModManager.Instance.SendModMessage("BasicRoads", "scheduleRoadsJob", new object[] { mapData, tileData, tileDataHandle },
                    (string message, object data) =>
                    {
                        if (message == "error")
                            Debug.LogError(data as string);
                        else
                            preAssignTilesHandle = (JobHandle)data;
                    });
            }

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
            JobHandle assignTilesHandle = assignTilesJob.Schedule(assignTilesDim * assignTilesDim, 64, preAssignTilesHandle);

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
                    int bl = tileData[JobA.Idx(x, y, tdDim)];
                    int br = tileData[JobA.Idx(x + 1, y, tdDim)];
                    int tr = tileData[JobA.Idx(x + 1, y + 1, tdDim)];
                    int tl = tileData[JobA.Idx(x, y + 1, tdDim)];

                    tilemapData[index] = tileTable[FindTileIndex(tileTable, bl, br, tr, tl)].Tile;
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
                if (worldClimate == (int)MapsFile.Climates.Desert2 && LowestPointFound(30, heightmapData, maxTerrainHeight, hx, hy, hDim, uB, index, tdDim, tileData)) {
                    tileData[index] = water;
                    return;
                }

                if (worldClimate == (int)MapsFile.Climates.Desert && LowestPointFound(80, heightmapData, maxTerrainHeight, hx, hy, hDim, uB, index, tdDim, tileData)) {
                    tileData[index] = water;
                    return;
                }

                /* // Create Rivers
                if (LowestPointFound(80, heightmapData, maxTerrainHeight, hx, hy, hDim, uB, index, tdDim, tileData)) {
                    tileData[index] = water;
                    return;
                } */

                // Rock Mountain Face
                if (SteepnessWithinLimits(true, Mathf.Clamp(90f - ((height / maxTerrainHeight)/0.85f * 100f),40f,90f), heightmapData, maxTerrainHeight, hx, hy, hDim, uB, index, tdDim, tileData)) {
                    tileData[index] = stone;
                    return;
                }

                int rnd = JobRand.Next(25,35);
                if (tileData[index] == stone && SteepnessWithinLimits(false, Mathf.Clamp(90f - ((height / maxTerrainHeight)/0.85f * 100f),40f,90f), heightmapData, maxTerrainHeight, hx, hy, hDim, uB, index, tdDim, tileData)) {
                    tileData[index] = grass;
                    return;
                }

                // Max angle for dirt patches
                rnd = JobRand.Next(20,25);
                if (tileData[index] == dirt && SteepnessWithinLimits(false, rnd, heightmapData, maxTerrainHeight, hx, hy, hDim, uB, index, tdDim, tileData)) {
                    tileData[index] = dirt;
                    return;
                }

                if (tileData[index] == dirt && !SteepnessWithinLimits(false, rnd, heightmapData, maxTerrainHeight, hx, hy, hDim, uB, index, tdDim, tileData)) {
                    tileData[index] = grass;
                    return;
                }
            }
        }

        void CreateLookupTable() {
            lookupTable = new byte[1];
            lookupTable[0] = MakeLookup(0, false, false);
        }

        // Encodes a byte with Daggerfall tile neighbours
        static int FindTileIndex(TileObject[] array, int bl, int br, int tr, int tl)
        {
            for (int i = 0; i < array.Length; ++i)
                if (array[i].Bl == bl && array[i].Br == br && array[i].Tr == tr && array[i].Tl == tl)
                    return i;

            return 0;
        }

        // Encodes a byte with Daggerfall tile lookup
        static byte MakeLookup(int index, bool rotate, bool flip)
        {
            if (index > 63)
                throw new IndexOutOfRangeException("Index out of range. Valid range 0-255");
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
                    JobA.Idx(hy + 5, hx + 5, hDim) > upperBound)
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
                    return (JobRand.Next(0, 100) <= newChance);
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
                return (diff >= steepness);
            } else {
                return (diff <= steepness);
            }
        }
    }
}
