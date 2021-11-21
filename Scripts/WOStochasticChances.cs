using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WOStochasticChances
{
    // ------------------------------------
    // GENERAL Vegetation Chances
    // ------------------------------------
    // Chance for different terrain layouts
    public float mapStyle = 0;
    public float[] mapStyleChance = { 30f, 35f, 40f, 45f, 50f, 55f };

    // ------------------------------------
    // TEMPERATE Climate Vegetation Chances
    // ------------------------------------
    // Chance for Mushrom Circle
    public float temperateMushroomRingChance;
    // Distribution Limits
    public float[] tempForestLimit = new float[2];
    // Noise Parameters
    public float tempForestFrequency;
    public float tempForestAmplitude;
    public float tempForestPersistence;
    public int tempForestOctaves;

    // ------------------------------------
    // MOUNTAIN Climate Vegetation Chances
    // ------------------------------------
    // Chance for Stone Circle
    public float mountainStoneCircleChance;
    // Distribution Limits
    public float[] mountForestLimit = new float[2];
    // Noise Parameters
    public float mountForestFrequency;
    public float mountForestAmplitude;
    public float mountForestPersistence;
    public int mountForestOctaves;

    // ------------------------------------
    // DESERT Climate Vegetation Chances
    // ------------------------------------
    // Chance for Dead Tree instead of Cactus
    public float desert2DirtChance;
    public float desert1DirtChance;
    // Chance for Flowers
    public float desert2GrassChance1;
    public float desert1GrassChance1;
    // Chance for Plants
    public float desert2GrassChance2;
    public float desert1GrassChance2;

    // ------------------------------------
    // HILLS Climate Vegetation Chances
    // ------------------------------------
    // Chance for Dead Tree/Rocks instead of Needle Tree
    public float woodlandHillsDirtChance;
    // Chance for Stone Circle
    public float woodlandHillsStoneCircleChance;

    public WOStochasticChances()
    {
        SetupMapStyle();
    }

    void SetupMapStyle()
    {
        int rnd = Random.Range(0, 6);
        if (rnd < 1)
            mapStyle = mapStyleChance[0];
        else if (rnd < 2)
            mapStyle = mapStyleChance[1];
        else if (rnd < 3)
            mapStyle = mapStyleChance[2];
        else if (rnd < 4)
            mapStyle = mapStyleChance[3];
        else if (rnd < 5)
            mapStyle = mapStyleChance[4];
        else
            mapStyle = mapStyleChance[5];

        // ------------------------------------
        // TEMPERATE Climate Vegetation Chances
        // ------------------------------------
        temperateMushroomRingChance = 0.025f;
        tempForestLimit[0] = Random.Range(0.35f, 0.45f);
        tempForestLimit[1] = tempForestLimit[0] + Random.Range(0.25f, 0.35f);
        tempForestFrequency = 0.01f;
        tempForestAmplitude = 0.9f;
        tempForestPersistence = 0.4f; //Random.Range(0.375f, 0.425f);
        tempForestOctaves = 3;

        // ------------------------------------
        // MOUNTAIN Climate Vegetation Chances
        // ------------------------------------
        mountainStoneCircleChance = 0.025f;
        mountForestLimit[0] = Random.Range(0.30f, 0.40f);
        mountForestLimit[1] = tempForestLimit[0] + Random.Range(0.2f, 0.25f);
        mountForestFrequency = Random.Range(0.04f, 0.06f);
        mountForestAmplitude = 0.9f;
        mountForestPersistence = Random.Range(0.35f, 0.45f);
        mountForestOctaves = Random.Range(2, 3);

        // ------------------------------------ 
        // DESERT Climate Vegetation Chances
        // ------------------------------------
        desert2DirtChance = Random.Range(0, 1);
        desert1DirtChance = Random.Range(1, 6);
        desert2GrassChance1 = Random.Range(0, 10);
        desert1GrassChance1 = Random.Range(0, 30);
        desert2GrassChance2 = Random.Range(10, 15);
        desert1GrassChance2 = Random.Range(30, 50);

        // ------------------------------------
        // HILLS Climate Vegetation Chances
        // ------------------------------------
        woodlandHillsDirtChance = Random.Range(20, 30);
        woodlandHillsStoneCircleChance = 0.075f;
    }
}
