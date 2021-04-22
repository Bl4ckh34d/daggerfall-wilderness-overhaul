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

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Generates texture tiles for terrains and uses marching squares for tile transitions.
    /// These features are very much in early stages of development.
    /// </summary>
    public class WOTerrainTexturing : ITerrainTexturing
    {
        // Use same seed to ensure continuous tiles
        const int seed = 417028;
        const byte water = 0;
        const byte dirt = 1;
        const byte grass = 2;
        const byte stone = 3;

        const float desert1Frequency = 0.02f;
        const float desert1Amplitude = 0.3f;
        const float desert1Persistance = 0.5f;
        const int desert1Octaves = 5;
        const float desert1UpperWaterSpread = -1.0f;
        const float desert1LowerGrassSpread = 0.4f;
        const float desert1UpperGrassSpread = 0.5f;

        const float desert2Frequency = 0.02f;
        const float desert2Amplitude = 0.3f;
        const float desert2Persistance = 0.55f;
        const int desert2Octaves = 5;
        const float desert2UpperWaterSpread = -1.0f;
        const float desert2LowerGrassSpread = 0.35f;
        const float desert2UpperGrassSpread = 0.5f;

        const float mountainFrequency = 0.025f;
        const float mountainAmplitude = 0.3f;
        const float mountainPersistance = 0.95f;
        const int mountainOctaves = 5;
        const float mountainUpperWaterSpread = 0.0f;
        const float mountainLowerGrassSpread = 0.35f;
        const float mountainUpperGrassSpread = 0.95f;

        const float rainforestFrequency = 0.035f;
        const float rainforestAmplitude = 0.4f;
        const float rainforestPersistance = 0.8f;
        const int rainforestOctaves = 5;
        const float rainforestUpperWaterSpread = 0.0f;
        const float rainforestLowerGrassSpread = 0.35f;
        const float rainforestUpperGrassSpread = 0.95f;

        const float swampFrequency = 0.02f;
        const float swampAmplitude = 0.3f;
        const float swampPersistance = 0.5f;
        const int swampOctaves = 5;
        const float swampUpperWaterSpread = -1.0f;
        const float swampLowerGrassSpread = 0.4f;
        const float swampUpperGrassSpread = 0.5f;

        const float subtropicalFrequency = 0.02f;
        const float subtropicalAmplitude = 0.3f;
        const float subtropicalPersistance = 0.5f;
        const int subtropicalOctaves = 5;
        const float subtropicalUpperWaterSpread = -1.0f;
        const float subtropicalLowerGrassSpread = 0.4f;
        const float subtropicalUpperGrassSpread = 0.5f;

        const float mountainWoodsFrequency = 0.035f;
        const float mountainWoodsAmplitude = 0.4f;
        const float mountainWoodsPersistance = 0.8f;
        const int mountainWoodsOctaves = 5;
        const float mountainWoodsUpperWaterSpread = 0.0f;
        const float mountainWoodsLowerGrassSpread = 0.35f;
        const float mountainWoodsUpperGrassSpread = 0.95f;

        const float woodlandsFrequency = 0.035f;
        const float woodlandsAmplitude = 0.4f;
        const float woodlandsPersistance = 0.8f;
        const int woodlandsOctaves = 5;
        const float woodlandsUpperWaterSpread = 0.0f;
        const float woodlandsLowerGrassSpread = 0.35f;
        const float woodlandsUpperGrassSpread = 0.95f;

        const float hauntedWoodsFrequency = 0.035f;
        const float hauntedWoodsAmplitude = 0.4f;
        const float hauntedWoodsPersistance = 0.8f;
        const int hauntedWoodsOctaves = 5;
        const float hauntedWoodsUpperWaterSpread = 0.0f;
        const float hauntedWoodsLowerGrassSpread = 0.35f;
        const float hauntedWoodsUpperGrassSpread = 0.95f;

        const float oceanFrequency = 0.1f;
        const float oceanAmplitude = 0.95f;
        const float oceanPersistance = 0.3f;
        const int oceanOctaves = 5;
        const float oceanUpperWaterSpread = 0.0f;
        const float oceanLowerGrassSpread = 0.35f;
        const float oceanUpperGrassSpread = 0.95f;

        public static float treeLine = UnityEngine.Random.Range(0.815f, 0.835f);

        protected static readonly int tileDataDim = MapsFile.WorldMapTileDim + 1;
        protected static readonly int assignTilesDim = MapsFile.WorldMapTileDim;

        protected byte[] lookupTable;
        static public TileObject[] tileList;

        public WOTerrainTexturing()
        {
            CreateLookupTable();
        }

        public virtual JobHandle ScheduleAssignTilesJob(ITerrainSampler terrainSampler, ref MapPixelData mapData, JobHandle dependencies, bool march = true)
        {
            // Cache tile data to minimise noise sampling during march.
            NativeArray<byte> tileData = new NativeArray<byte>(tileDataDim * tileDataDim, Allocator.TempJob);

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

        #region Marching Squares - WIP

        // Very basic marching squares for water > dirt > grass > stone transitions.
        // Cannot handle water > grass or water > stone, etc.
        // Will improve this at later date to use a wider range of transitions.
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

                    tilemapData[index] = GetTileByNeighbours(tl, tr, br, bl);
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
                float woodlandsPersistanceRnd = woodlandsPersistance + ((height / maxTerrainHeight) * 1.4f) - 0.30f;
                float mountainWoodsPersistanceRnd = mountainWoodsPersistance + ((height / maxTerrainHeight) * 1.5f) - 0.35f;
                float hauntedWoodsPersistanceRnd = hauntedWoodsPersistance + ((height / maxTerrainHeight) * 1.5f) - 0.35f;
                float mountainsPersistanceRnd;
                if ((height / maxTerrainHeight) + JobRand.Next(-5, 5) / 1000f > treeLine)
                {
                    mountainsPersistanceRnd = mountainPersistance - treeLine + ((height / maxTerrainHeight) * 1.2f);
                }
                else
                {
                    mountainsPersistanceRnd = mountainPersistance - (1 - (height / maxTerrainHeight)) * 0.4f;
                }
                float desert1PersistanceRnd = desert1Persistance + ((height / maxTerrainHeight) * 2) - 0.25f;
                float desert2PersistanceRnd = desert2Persistance + ((height / maxTerrainHeight) * 2) - 0.25f;
                float subtropicalPersistanceRnd = subtropicalPersistance + ((height / maxTerrainHeight) * 2) - 0.25f;
                float swampPersistanceRnd = swampPersistance + ((height / maxTerrainHeight) * 3) - 0.25f;
                float rainforestPersistanceRnd = rainforestPersistance + ((height / maxTerrainHeight) * 1.5f) - 0.35f;

                switch (worldClimate)
                {
                    case (int)MapsFile.Climates.Desert:
                        return GetNoise(worldX, worldY, desert1Frequency, desert1Amplitude, desert1PersistanceRnd, desert1Octaves, seed);
                    case (int)MapsFile.Climates.Desert2:
                        return GetNoise(worldX, worldY, desert2Frequency, desert2Amplitude, desert2PersistanceRnd, desert2Octaves, seed);
                    case (int)MapsFile.Climates.Mountain:
                        return GetNoise(worldX, worldY, mountainFrequency, mountainAmplitude, mountainsPersistanceRnd, mountainOctaves, seed);
                    case (int)MapsFile.Climates.Rainforest:
                        return GetNoise(worldX, worldY, rainforestFrequency, rainforestAmplitude, rainforestPersistance, rainforestOctaves, seed);
                    case (int)MapsFile.Climates.Swamp:
                        return GetNoise(worldX, worldY, swampFrequency, swampAmplitude, swampPersistance, swampOctaves, seed);
                    case (int)MapsFile.Climates.Subtropical:
                        return GetNoise(worldX, worldY, subtropicalFrequency, subtropicalAmplitude, subtropicalPersistanceRnd, subtropicalOctaves, seed);
                    case (int)MapsFile.Climates.MountainWoods:
                        return GetNoise(worldX, worldY, mountainWoodsFrequency, mountainWoodsAmplitude, mountainWoodsPersistanceRnd, mountainWoodsOctaves, seed);
                    case (int)MapsFile.Climates.Woodlands:
                        return GetNoise(worldX, worldY, woodlandsFrequency, woodlandsAmplitude, woodlandsPersistanceRnd, woodlandsOctaves, seed);
                    case (int)MapsFile.Climates.HauntedWoodlands:
                        return GetNoise(worldX, worldY, hauntedWoodsFrequency, hauntedWoodsAmplitude, hauntedWoodsPersistanceRnd, hauntedWoodsOctaves, seed);
                    case (int)MapsFile.Climates.Ocean:
                        return GetNoise(worldX, worldY, oceanFrequency, oceanAmplitude, oceanPersistance, oceanOctaves, seed);
                }
                return GetNoise(worldX, worldY, 0.1f, 0.95f, 0.3f, 5, seed); //worldX, worldY, 0.05f, 0.9f, 0.4f, 3, seed
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

                // Height sample for ocean and beach tiles
                int hx = (int)Mathf.Clamp(hDim * ((float)x / (float)tdDim), 0, hDim - 1);
                int hy = (int)Mathf.Clamp(hDim * ((float)y / (float)tdDim), 0, hDim - 1);
                float height = heightmapData[JobA.Idx(hy, hx, hDim)] * maxTerrainHeight;  // x & y swapped in heightmap for TerrainData.SetHeights()

                // Ocean texture
                if (height <= oceanElevation)
                {
                    tileData[index] = water;
                    return;
                }
                // Beach texture
                // Adds a little +/- randomness to threshold so beach line isn't too regular
                if (height <= beachElevation + (JobRand.Next(-100000000, -55000000) / 10000000f))
                {
                    tileData[index] = dirt;
                    return;
                }

                // Get latitude and longitude of this tile
                int latitude = (int)(mapPixelX * MapsFile.WorldMapTileDim + x);
                int longitude = (int)(MapsFile.MaxWorldTileCoordZ - mapPixelY * MapsFile.WorldMapTileDim + y);

                // Set texture tile using weighted noise
                float weight = 0;

                weight += NoiseWeight(latitude, longitude, height);

                switch (worldClimate)
                {
                    case (int)MapsFile.Climates.Desert:
                        tileData[index] = GetWeightedRecord(weight, desert1UpperWaterSpread, desert1LowerGrassSpread, desert1UpperGrassSpread);
                        break;
                    case (int)MapsFile.Climates.Desert2:
                        tileData[index] = GetWeightedRecord(weight, desert2UpperWaterSpread, desert2LowerGrassSpread, desert2UpperGrassSpread);
                        break;
                    case (int)MapsFile.Climates.Mountain:
                        tileData[index] = GetWeightedRecord(weight, mountainUpperWaterSpread, mountainLowerGrassSpread, mountainUpperGrassSpread);
                        break;
                    case (int)MapsFile.Climates.Rainforest:
                        tileData[index] = GetWeightedRecord(weight, rainforestUpperWaterSpread, rainforestLowerGrassSpread, rainforestUpperGrassSpread);
                        break;
                    case (int)MapsFile.Climates.Swamp:
                        tileData[index] = GetWeightedRecord(weight, swampUpperWaterSpread, swampLowerGrassSpread, swampUpperGrassSpread);
                        break;
                    case (int)MapsFile.Climates.Subtropical:
                        tileData[index] = GetWeightedRecord(weight, subtropicalUpperWaterSpread, subtropicalLowerGrassSpread, subtropicalUpperGrassSpread);
                        break;
                    case (int)MapsFile.Climates.MountainWoods:
                        tileData[index] = GetWeightedRecord(weight, mountainWoodsUpperWaterSpread, mountainWoodsLowerGrassSpread, mountainWoodsUpperGrassSpread);
                        break;
                    case (int)MapsFile.Climates.Woodlands:
                        tileData[index] = GetWeightedRecord(weight, woodlandsUpperWaterSpread, woodlandsLowerGrassSpread, woodlandsUpperGrassSpread);
                        break;
                    case (int)MapsFile.Climates.HauntedWoodlands:
                        tileData[index] = GetWeightedRecord(weight, hauntedWoodsUpperWaterSpread, hauntedWoodsLowerGrassSpread, hauntedWoodsUpperGrassSpread);
                        break;
                    case (int)MapsFile.Climates.Ocean:
                        tileData[index] = GetWeightedRecord(weight, oceanUpperWaterSpread, oceanLowerGrassSpread, oceanUpperGrassSpread);
                        break;
                }
                // Check for lowest local point in desert to place oasis
                if (worldClimate == (int)MapsFile.Climates.Desert2 &&
                    LowestPointFound(30, heightmapData, maxTerrainHeight, hx, hy, hDim, uB, index, tdDim, tileData))
                {
                    tileData[index] = water;
                }
                if (worldClimate == (int)MapsFile.Climates.Desert &&
                    LowestPointFound(80, heightmapData, maxTerrainHeight, hx, hy, hDim, uB, index, tdDim, tileData))
                {
                    tileData[index] = water;
                }
                // Rock Mountain Face
                if (SteepnessTooHigh(55f, heightmapData, maxTerrainHeight, hx, hy, hDim, uB, index, tdDim, tileData))
                {
                    tileData[index] = stone;
                }
            }
        }

        // Creates lookup table
        void CreateLookupTable()
        {
            tileList = new TileObject[88];
            lookupTable = new byte[1];
            lookupTable[0] = MakeLookup(0,  false, false);
            //water > dirt
            tileList[0] = new TileObject(MakeLookup(0,  false, false) ,0,0,0,0); //w,w,w,w
            tileList[1] = new TileObject(MakeLookup(5,  true,  false) ,0,0,0,1); //w,w,w,d
            tileList[2] = new TileObject(MakeLookup(5,  false, true)  ,0,0,1,0); //w,w,d,w
            tileList[3] = new TileObject(MakeLookup(6,  true,  false) ,0,0,1,1); //w,w,d,d
            tileList[4] = new TileObject(MakeLookup(5,  true,  true)  ,0,1,0,0); //w,d,w,w
            tileList[5] = new TileObject(MakeLookup(48, true,  false) ,0,1,0,1); //w,d,w,d
            tileList[6] = new TileObject(MakeLookup(6,  false, true)  ,0,1,1,0); //w,d,d,w
            tileList[7] = new TileObject(MakeLookup(7,  false, true)  ,0,1,1,1); //w,d,d,d
            tileList[8] = new TileObject(MakeLookup(5,  false, false) ,1,0,0,0); //d,w,w,w
            tileList[9] = new TileObject(MakeLookup(6,  false, false) ,1,0,0,1); //d,w,w,d
            tileList[10] = new TileObject(MakeLookup(48, false, false) ,1,0,1,0); //d,w,d,w
            tileList[11] = new TileObject(MakeLookup(7,  true,  false) ,1,0,1,1); //d,w,d,d
            tileList[12] = new TileObject(MakeLookup(6,  true,  true)  ,1,1,0,0); //d,d,w,w
            tileList[13] = new TileObject(MakeLookup(7,  false, false) ,1,1,0,1); //d,d,w,d
            tileList[14] = new TileObject(MakeLookup(7,  true,  true)  ,1,1,1,0); //d,d,d,w
            tileList[15] = new TileObject(MakeLookup(1,  false, false) ,1,1,1,1); //d,d,d,d
            //water > grass
            tileList[16] = new TileObject(MakeLookup(20, true,  false) ,0,0,0,2); //w,w,w,g
            tileList[17] = new TileObject(MakeLookup(20, false, true)  ,0,0,2,0); //w,w,g,w
            tileList[18] = new TileObject(MakeLookup(21, true,  false) ,0,0,2,2); //w,w,g,g
            tileList[19] = new TileObject(MakeLookup(20, true,  true)  ,0,2,0,0); //w,g,w,w
            tileList[20] = new TileObject(MakeLookup(49, true,  false) ,0,2,0,2); //w,g,w,g
            tileList[21] = new TileObject(MakeLookup(21, false, true)  ,0,2,2,0); //w,g,g,w
            tileList[22] = new TileObject(MakeLookup(22, false, true)  ,0,2,2,2); //w,g,g,g
            tileList[23] = new TileObject(MakeLookup(20, false, false) ,2,0,0,0); //g,w,w,w
            tileList[24] = new TileObject(MakeLookup(21, false, false) ,2,0,0,2); //g,w,w,g
            tileList[25] = new TileObject(MakeLookup(49, false, false) ,2,0,2,0); //g,w,g,w
            tileList[26] = new TileObject(MakeLookup(22, true,  false) ,2,0,2,2); //g,w,g,g
            tileList[27] = new TileObject(MakeLookup(21, true,  true)  ,2,2,0,0); //g,g,w,w
            tileList[28] = new TileObject(MakeLookup(22, false, false) ,2,2,0,2); //g,g,w,g
            tileList[29] = new TileObject(MakeLookup(22, true,  true)  ,2,2,2,0); //g,g,g,w
            tileList[30] = new TileObject(MakeLookup(2,  false, false) ,2,2,2,2); //g,g,g,g
            //water > stone
            tileList[31] = new TileObject(MakeLookup(30, true,  false) ,0,0,0,3); //w,w,w,s
            tileList[32] = new TileObject(MakeLookup(30, false, true)  ,0,0,3,0); //w,w,s,w
            tileList[33] = new TileObject(MakeLookup(31, true,  false) ,0,0,3,3); //w,w,s,s
            tileList[34] = new TileObject(MakeLookup(30, true,  true)  ,0,3,0,0); //w,s,w,w
            tileList[35] = new TileObject(MakeLookup(50, true,  false) ,0,3,0,3); //w,s,w,s
            tileList[36] = new TileObject(MakeLookup(31, false, true)  ,0,3,3,0); //w,s,s,w
            tileList[37] = new TileObject(MakeLookup(32, false, true)  ,0,3,3,3); //w,s,s,s
            tileList[38] = new TileObject(MakeLookup(30, false, false) ,3,0,0,0); //s,w,w,w
            tileList[39] = new TileObject(MakeLookup(31, false, false) ,3,0,0,3); //s,w,w,s
            tileList[40] = new TileObject(MakeLookup(50, false, false) ,3,0,3,0); //s,w,s,w
            tileList[41] = new TileObject(MakeLookup(32, true,  false) ,3,0,3,3); //s,w,s,s
            tileList[42] = new TileObject(MakeLookup(31, true,  true)  ,3,3,0,0); //s,s,w,w
            tileList[43] = new TileObject(MakeLookup(32, false, false) ,3,3,0,3); //s,s,w,s
            tileList[44] = new TileObject(MakeLookup(32, true,  true)  ,3,3,3,0); //s,s,s,w
            tileList[45] = new TileObject(MakeLookup(3,  false, false) ,3,3,3,3); //s,s,s,s
            //dirt > grass
            tileList[46] = new TileObject(MakeLookup(10, true,  false) ,1,1,1,2); //d,d,d,g
            tileList[47] = new TileObject(MakeLookup(10, false, true)  ,1,1,2,1); //d,d,g,d
            tileList[48] = new TileObject(MakeLookup(11, true,  false) ,1,1,2,2); //d,d,g,g
            tileList[49] = new TileObject(MakeLookup(10, true,  true)  ,1,2,1,1); //d,g,d,d
            tileList[50] = new TileObject(MakeLookup(51, true,  false) ,1,2,1,2); //d,g,d,g
            tileList[51] = new TileObject(MakeLookup(11, false, true)  ,1,2,2,1); //d,g,g,d
            tileList[52] = new TileObject(MakeLookup(12, false, true)  ,1,2,2,2); //d,g,g,g
            tileList[53] = new TileObject(MakeLookup(10, false, false) ,2,1,1,1); //g,d,d,d
            tileList[54] = new TileObject(MakeLookup(11, false, false) ,2,1,1,2); //g,d,d,g
            tileList[55] = new TileObject(MakeLookup(51, false, false) ,2,1,2,1); //g,d,g,d
            tileList[56] = new TileObject(MakeLookup(12, true,  false) ,2,1,2,2); //g,d,g,g
            tileList[57] = new TileObject(MakeLookup(11, true,  true)  ,2,2,1,1); //g,g,d,d
            tileList[58] = new TileObject(MakeLookup(12, false, false) ,2,2,1,2); //g,g,d,g
            tileList[59] = new TileObject(MakeLookup(12, true,  true)  ,2,2,2,1); //g,g,g,d
            //dirt > stone
            tileList[60] = new TileObject(MakeLookup(25, true,  false) ,1,1,1,3); //d,d,d,s
            tileList[61] = new TileObject(MakeLookup(25, false, true)  ,1,1,3,1); //d,d,s,d
            tileList[62] = new TileObject(MakeLookup(26, true,  false) ,1,1,3,3); //d,d,s,s
            tileList[63] = new TileObject(MakeLookup(25, true,  true)  ,1,3,1,1); //d,s,d,d
            tileList[64] = new TileObject(MakeLookup(52, true,  false) ,1,3,1,3); //d,s,d,s
            tileList[65] = new TileObject(MakeLookup(26, false, true)  ,1,3,3,1); //d,s,s,d
            tileList[66] = new TileObject(MakeLookup(27, false, true)  ,1,3,3,3); //d,s,s,s
            tileList[67] = new TileObject(MakeLookup(25, false, false) ,3,1,1,1); //s,d,d,d
            tileList[68] = new TileObject(MakeLookup(26, false, false) ,3,1,1,3); //s,d,d,s
            tileList[69] = new TileObject(MakeLookup(52, false, false) ,3,1,3,1); //s,d,s,d
            tileList[70] = new TileObject(MakeLookup(27, true,  false) ,3,1,3,3); //s,d,s,s
            tileList[71] = new TileObject(MakeLookup(26, true,  true)  ,3,3,1,1); //s,s,d,d
            tileList[72] = new TileObject(MakeLookup(27, false, false) ,3,3,1,3); //s,s,d,s
            tileList[73] = new TileObject(MakeLookup(27, true,  true)  ,3,3,3,1); //s,s,s,d
            //grass > stone
            tileList[74] = new TileObject(MakeLookup(15, true,  false) ,2,2,2,3); //g,g,g,s
            tileList[75] = new TileObject(MakeLookup(15, false, true)  ,2,2,3,2); //g,g,s,g
            tileList[76] = new TileObject(MakeLookup(16, true,  false) ,2,2,3,3); //g,g,s,s
            tileList[77] = new TileObject(MakeLookup(15, true,  true)  ,2,3,2,2); //g,s,g,g
            tileList[78] = new TileObject(MakeLookup(53, true,  false) ,2,3,2,3); //g,s,g,s
            tileList[79] = new TileObject(MakeLookup(16, false, true)  ,2,3,3,2); //g,s,s,g
            tileList[80] = new TileObject(MakeLookup(17, false, true)  ,2,3,3,3); //g,s,s,s
            tileList[81] = new TileObject(MakeLookup(15, false, false) ,3,2,2,2); //s,g,g,g
            tileList[82] = new TileObject(MakeLookup(16, false, false) ,3,2,2,3); //s,g,g,s
            tileList[83] = new TileObject(MakeLookup(53, false, false) ,3,2,3,2); //s,g,s,g
            tileList[84] = new TileObject(MakeLookup(17, true,  false) ,3,2,3,3); //s,g,s,s
            tileList[85] = new TileObject(MakeLookup(16, true,  true)  ,3,3,2,2); //s,s,g,g
            tileList[86] = new TileObject(MakeLookup(17, false, false) ,3,3,2,3); //s,s,g,s
            tileList[87] = new TileObject(MakeLookup(17, true,  true)  ,3,3,3,2); //s,s,s,g

            //water > dirt > grass
            /* tileList[88] = new TileObject(MakeLookup(X, X, X) ,0,0,2,1); //w,w,g,d
            tileList[89] = new TileObject(MakeLookup(X, X, X) ,0,2,0,1); //w,g,w,d
            tileList[90] = new TileObject(MakeLookup(X, X, X) ,0,2,2,1); //w,g,g,d
            tileList[91] = new TileObject(MakeLookup(X, X, X) ,2,0,0,1); //g,w,w,d
            tileList[92] = new TileObject(MakeLookup(X, X, X) ,2,0,2,1); //g,w,g,d
            tileList[93] = new TileObject(MakeLookup(X, X, X) ,2,2,0,1); //g,g,w,d

            tileList[94] = new TileObject(MakeLookup(X, X, X) ,0,0,1,2); //w,w,d,g
            tileList[95] = new TileObject(MakeLookup(X, X, X) ,0,2,1,0); //w,g,d,w
            tileList[96] = new TileObject(MakeLookup(X, X, X) ,0,2,1,2); //w,g,d,g
            tileList[97] = new TileObject(MakeLookup(X, X, X) ,2,0,1,0); //g,w,d,w
            tileList[98] = new TileObject(MakeLookup(X, X, X) ,2,0,1,2); //g,w,d,g
            tileList[99] = new TileObject(MakeLookup(X, X, X) ,2,2,1,0); //g,g,d,w

            tileList[100] = new TileObject(MakeLookup(X, X, X) ,0,1,0,2); //w,d,w,g
            tileList[101] = new TileObject(MakeLookup(X, X, X) ,0,1,2,0); //w,d,g,w
            tileList[102] = new TileObject(MakeLookup(X, X, X) ,0,1,2,2); //w,d,g,g
            tileList[103] = new TileObject(MakeLookup(X, X, X) ,2,1,0,0); //g,d,w,w
            tileList[104] = new TileObject(MakeLookup(X, X, X) ,2,1,0,2); //g,d,w,g
            tileList[105] = new TileObject(MakeLookup(X, X, X) ,2,1,2,0); //g,d,g,w

            tileList[106] = new TileObject(MakeLookup(X, X, X) ,1,0,0,2); //d,w,w,g
            tileList[107] = new TileObject(MakeLookup(X, X, X) ,1,0,2,0); //d,w,g,w
            tileList[108] = new TileObject(MakeLookup(X, X, X) ,1,0,2,2); //d,w,g,g
            tileList[109] = new TileObject(MakeLookup(X, X, X) ,1,2,0,0); //d,g,w,w
            tileList[110] = new TileObject(MakeLookup(X, X, X) ,1,2,0,2); //d,g,w,g
            tileList[111] = new TileObject(MakeLookup(X, X, X) ,1,2,2,0); //d,g,g,w

            tileList[112] = new TileObject(MakeLookup(X, X, X) ,0,2,1,1); //w,g,d,d
            tileList[113] = new TileObject(MakeLookup(X, X, X) ,2,0,1,1); //g,w,d,d

            tileList[114] = new TileObject(MakeLookup(X, X, X) ,0,1,1,2); //w,d,d,g
            tileList[115] = new TileObject(MakeLookup(X, X, X) ,2,1,1,0); //g,d,d,w

            tileList[116] = new TileObject(MakeLookup(X, X, X) ,1,1,0,2); //d,d,w,g
            tileList[117] = new TileObject(MakeLookup(X, X, X) ,1,1,2,0); //d,d,g,w

            tileList[118] = new TileObject(MakeLookup(X, X, X) ,0,1,2,1); //w,d,g,d
            tileList[119] = new TileObject(MakeLookup(X, X, X) ,2,1,0,1); //g,d,w,d

            tileList[120] = new TileObject(MakeLookup(X, X, X) ,1,0,1,2); //d,w,d,g
            tileList[121] = new TileObject(MakeLookup(X, X, X) ,1,2,1,0); //d,g,d,w

            tileList[122] = new TileObject(MakeLookup(X, X, X) ,1,0,2,1); //d,w,g,d
            tileList[123] = new TileObject(MakeLookup(X, X, X) ,1,2,0,1); //d,g,w,d

            //water > grass > stone
            tileList[124] = new TileObject(MakeLookup(X, X, X) ,0,0,2,3); //w,w,g,s
            tileList[125] = new TileObject(MakeLookup(X, X, X) ,0,2,0,3); //w,g,w,s
            tileList[126] = new TileObject(MakeLookup(X, X, X) ,0,2,2,3); //w,g,g,s
            tileList[127] = new TileObject(MakeLookup(X, X, X) ,2,0,0,3); //g,w,w,s
            tileList[128] = new TileObject(MakeLookup(X, X, X) ,2,0,2,3); //g,w,g,s
            tileList[129] = new TileObject(MakeLookup(X, X, X) ,2,2,0,3); //g,g,w,s

            tileList[130] = new TileObject(MakeLookup(X, X, X) ,0,0,3,2); //w,w,s,g
            tileList[131] = new TileObject(MakeLookup(X, X, X) ,0,2,3,0); //w,g,s,w
            tileList[132] = new TileObject(MakeLookup(X, X, X) ,0,2,3,2); //w,g,s,g
            tileList[133] = new TileObject(MakeLookup(X, X, X) ,2,0,3,0); //g,w,s,w
            tileList[134] = new TileObject(MakeLookup(X, X, X) ,2,0,3,2); //g,w,s,g
            tileList[135] = new TileObject(MakeLookup(X, X, X) ,2,2,3,0); //g,g,s,w

            tileList[136] = new TileObject(MakeLookup(X, X, X) ,0,3,0,2); //w,s,w,g
            tileList[137] = new TileObject(MakeLookup(X, X, X) ,0,3,2,0); //w,s,g,w
            tileList[138] = new TileObject(MakeLookup(X, X, X) ,0,3,2,2); //w,s,g,g
            tileList[139] = new TileObject(MakeLookup(X, X, X) ,2,3,0,0); //g,s,w,w
            tileList[140] = new TileObject(MakeLookup(X, X, X) ,2,3,0,2); //g,s,w,g
            tileList[141] = new TileObject(MakeLookup(X, X, X) ,2,3,2,0); //g,s,g,w

            tileList[142] = new TileObject(MakeLookup(X, X, X) ,3,0,0,2); //s,w,w,g
            tileList[143] = new TileObject(MakeLookup(X, X, X) ,3,0,2,0); //s,w,g,w
            tileList[144] = new TileObject(MakeLookup(X, X, X) ,3,0,2,2); //s,w,g,g
            tileList[145] = new TileObject(MakeLookup(X, X, X) ,3,2,0,0); //s,g,w,w
            tileList[146] = new TileObject(MakeLookup(X, X, X) ,3,2,0,2); //s,g,w,g
            tileList[147] = new TileObject(MakeLookup(X, X, X) ,3,2,2,0); //s,g,g,w

            tileList[148] = new TileObject(MakeLookup(X, X, X) ,0,2,3,3); //w,g,s,s
            tileList[149] = new TileObject(MakeLookup(X, X, X) ,2,0,3,3); //g,w,s,s

            tileList[150] = new TileObject(MakeLookup(X, X, X) ,0,3,3,2); //w,s,s,g
            tileList[151] = new TileObject(MakeLookup(X, X, X) ,2,3,3,0); //g,s,s,w

            tileList[152] = new TileObject(MakeLookup(X, X, X) ,3,3,0,2); //s,s,w,g
            tileList[153] = new TileObject(MakeLookup(X, X, X) ,3,3,2,0); //s,s,g,w

            tileList[154] = new TileObject(MakeLookup(X, X, X) ,0,3,2,3); //w,s,g,s
            tileList[155] = new TileObject(MakeLookup(X, X, X) ,2,3,0,3); //g,s,w,s

            tileList[156] = new TileObject(MakeLookup(X, X, X) ,3,0,3,2); //s,w,s,g
            tileList[157] = new TileObject(MakeLookup(X, X, X) ,3,2,3,0); //s,g,s,w

            tileList[158] = new TileObject(MakeLookup(X, X, X) ,3,0,2,3); //s,w,g,s
            tileList[159] = new TileObject(MakeLookup(X, X, X) ,3,2,0,3); //s,g,w,s

            //dirt > grass > stone
            tileList[160] = new TileObject(MakeLookup(X, X, X) ,1,1,2,3); //d,d,g,s
            tileList[161] = new TileObject(MakeLookup(X, X, X) ,1,2,1,3); //d,g,d,s
            tileList[162] = new TileObject(MakeLookup(X, X, X) ,1,2,2,3); //d,g,g,s
            tileList[163] = new TileObject(MakeLookup(X, X, X) ,2,1,1,3); //g,d,d,s
            tileList[164] = new TileObject(MakeLookup(X, X, X) ,2,1,2,3); //g,d,g,s
            tileList[165] = new TileObject(MakeLookup(X, X, X) ,2,2,1,3); //g,g,d,s

            tileList[166] = new TileObject(MakeLookup(X, X, X) ,1,1,3,2); //d,d,s,g
            tileList[167] = new TileObject(MakeLookup(X, X, X) ,1,2,3,1); //d,g,s,d
            tileList[168] = new TileObject(MakeLookup(X, X, X) ,1,2,3,2); //d,g,s,g
            tileList[169] = new TileObject(MakeLookup(X, X, X) ,2,1,3,1); //g,d,s,d
            tileList[170] = new TileObject(MakeLookup(X, X, X) ,2,1,3,2); //g,d,s,g
            tileList[171] = new TileObject(MakeLookup(X, X, X) ,2,2,3,1); //g,g,s,d

            tileList[172] = new TileObject(MakeLookup(X, X, X) ,1,3,1,2); //d,s,d,g
            tileList[173] = new TileObject(MakeLookup(X, X, X) ,1,3,2,1); //d,s,g,d
            tileList[174] = new TileObject(MakeLookup(X, X, X) ,1,3,2,2); //d,s,g,g
            tileList[175] = new TileObject(MakeLookup(X, X, X) ,2,3,1,1); //g,s,d,d
            tileList[176] = new TileObject(MakeLookup(X, X, X) ,2,3,1,2); //g,s,d,g
            tileList[177] = new TileObject(MakeLookup(X, X, X) ,2,3,2,1); //g,s,g,d

            tileList[178] = new TileObject(MakeLookup(X, X, X) ,3,1,1,2); //s,d,d,g
            tileList[179] = new TileObject(MakeLookup(X, X, X) ,3,1,2,1); //s,d,g,d
            tileList[180] = new TileObject(MakeLookup(X, X, X) ,3,1,2,2); //s,d,g,g
            tileList[181] = new TileObject(MakeLookup(X, X, X) ,3,2,1,1); //s,g,d,d
            tileList[182] = new TileObject(MakeLookup(X, X, X) ,3,2,1,2); //s,g,d,g
            tileList[183] = new TileObject(MakeLookup(X, X, X) ,3,2,2,1); //s,g,g,d

            tileList[184] = new TileObject(MakeLookup(X, X, X) ,1,2,3,3); //d,g,s,s
            tileList[185] = new TileObject(MakeLookup(X, X, X) ,2,1,3,3); //g,d,s,s

            tileList[186] = new TileObject(MakeLookup(X, X, X) ,1,3,3,2); //d,s,s,g
            tileList[187] = new TileObject(MakeLookup(X, X, X) ,2,3,3,1); //g,s,s,d

            tileList[188] = new TileObject(MakeLookup(X, X, X) ,3,3,1,2); //s,s,d,g
            tileList[189] = new TileObject(MakeLookup(X, X, X) ,3,3,2,1); //s,s,g,d

            tileList[190] = new TileObject(MakeLookup(X, X, X) ,1,3,2,3); //d,s,g,s
            tileList[191] = new TileObject(MakeLookup(X, X, X) ,2,3,1,3); //g,s,d,s

            tileList[192] = new TileObject(MakeLookup(X, X, X) ,3,1,3,2); //s,d,s,g
            tileList[193] = new TileObject(MakeLookup(X, X, X) ,3,2,3,1); //s,g,s,d

            tileList[194] = new TileObject(MakeLookup(X, X, X) ,3,1,2,3); //s,d,g,s
            tileList[195] = new TileObject(MakeLookup(X, X, X) ,3,2,1,3); //s,g,d,s */
        }

        // Encodes a byte with Daggerfall tile lookup
        static byte MakeLookup(int index, bool rotate, bool flip)
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

        static bool SteepnessTooHigh(float steepness, NativeArray<float> heightmapData, float maxTerrainHeight, int hx, int hy, int hDim, int upperBound, int index, int tdDim, NativeArray<byte> tileData)
        {
            if (JobA.Row(index, tdDim) <= 0 ||
                JobA.Col(index, tdDim) + 1 >= tdDim ||
                JobA.Row(index, tdDim) + 1 >= tdDim ||
                JobA.Col(index, tdDim) <= 0)
            {
                return false;
            }
            else
            {
                if (JobA.Idx(hy, hx, hDim) < 0 ||
                    JobA.Idx(hy, hx, hDim) > upperBound ||
                    JobA.Idx(hy, hx + 1, hDim) < 0 ||
                    JobA.Idx(hy, hx + 1, hDim) > upperBound ||
                    JobA.Idx(hy + 1, hx, hDim) < 0 ||
                    JobA.Idx(hy + 1, hx, hDim) > upperBound ||
                    JobA.Idx(hy + 1, hx + 1, hDim) < 0 ||
                    JobA.Idx(hy + 1, hx + 1, hDim) > upperBound
                    )
                {
                    return false;
                }
                else
                {
                    float minSmpl = 0;
                    float maxSmpl = 0;
                    float smpl = minSmpl = maxSmpl = heightmapData[JobA.Idx(hy, hx, hDim)] * maxTerrainHeight;
                    for (int a = 0; a <= 1; a++)
                    {
                        for (int b = 0; b <= 1; b++)
                        {
                            smpl = heightmapData[JobA.Idx(hy + a, hx + b, hDim)] * maxTerrainHeight;

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

                    if (diff > steepness)
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

        static byte GetTileByNeighbours(int bl, int br, int tr, int tl)
        {
            TileObject tLO;
            tLO = Array.Find(tileList, obj => obj.bl == bl && obj.br == br && obj.tr == tr && obj.tl == tl);
            if (tLO == null)
                tLO = new TileObject(WOTerrainTexturing.MakeLookup(0,  false, false) ,0,0,0,0);
            return tLO.tile;
        }
        #endregion
    }

    public class TileObject
    {
        public byte tile { get; set; }
        public int bl { get; set; }
        public int br { get; set; }
        public int tr { get; set; }
        public int tl { get; set; }

        public TileObject(byte Tile, int Bl, int Br, int Tr, int Tl)
        {
            tile = Tile;
            bl = Bl;
            br = Br;
            tr = Tr;
            tl = Tl;
        }
    }
}