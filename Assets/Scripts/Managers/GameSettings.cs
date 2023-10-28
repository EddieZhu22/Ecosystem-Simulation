using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GameSettings
{
    public enum mode
    {
        Rotate,
        Free,
        Attatched,
    }

    public float volume = 1.0f;

    public mode CameraMode;
    public float waterHeight;
    public int CreatureLOD;

    public int NumGenes;

    public float CreatureFoodDecrease = 1; 
    public float CreatureSpeedMultiplier = 1;
    public float PredatorFoodGainMultiplier = 1;
    public float FoodLossMultiplier = 5;
    public bool showCreatureDetails;
    public bool segregation;
    public float refractoryPeriod = 500;
    public float creatureDigestionDuration = 100;


    public float plantEnergyGainMultiplier = 1;
    public float seedRangeMultiplier = 1;

}
