using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlantEditor : MonoBehaviour
{
    [Header("References")]
    public GameObject seed;
    public TreeGenerator generator;
    public GameManager manager;
    public GameUI ui;

    [Header("Attributes")]
    public float LightConsumption;
    public float waterConsumption;
    public float Weight;
    public float Height;
    public float rootType;
    public float hardness;
    public float range;
    public float survivalpct;
    public float seedType;
    public int seedNum = 1;
    public int genderNum = 1;
    private int lastSeedNum; // Store the last seed number

    [Header("UI")]
    public Slider recsl;
    public Slider trunksl;
    public Slider floorhsl;
    public Slider firstbranchsl;
    public Slider twistinessl;
    public Slider branchdensitysl;
    public Slider leavesSizesl;
    public Slider distortionsl;
    public Slider hardnesssl;
    public Text rectxt, trunktxt, floorhtxt, firstbranchtxt, twistinestxt, branchdensitytxt, leavesSizetxt, seedtxt, lightconsumptiontxt, waterconsumptiontxt, weighttxt, heighttxt, hardnesstxt, rangetxt, seedSurvivaltxt;
    public TMP_Text GenderText, SeedText;
    public string[] SeedTextStr;
    public string[] GenderTextStr;

    private bool changed;

    private Vector3 originalPos;
    void Start()
    {
        init();
    }

    void Update()
    {

        SeedText.text = SeedTextStr[seedNum - 1];
        GenderText.text = GenderTextStr[genderNum - 1];

        generator.leavesMaterial = seedNum == 1 ? manager.plantFruit : manager.plantLeaves;
        if (seedNum != lastSeedNum)
        {
            // Update lastSeedNum
            lastSeedNum = seedNum;

            // Call gen function to regenerate
            changed = true;
        }

        if (changed == true)
        {
            CreateTree();
            changed = false;
        }
    }
    #region Methods
    private void init()
    {
        recsl.onValueChanged.AddListener(delegate { valChanged(); });
        trunksl.onValueChanged.AddListener(delegate { valChanged(); });
        floorhsl.onValueChanged.AddListener(delegate { valChanged(); });
        firstbranchsl.onValueChanged.AddListener(delegate { valChanged(); });
        twistinessl.onValueChanged.AddListener(delegate { valChanged(); });
        branchdensitysl.onValueChanged.AddListener(delegate { valChanged(); });
        leavesSizesl.onValueChanged.AddListener(delegate { valChanged(); });
        distortionsl.onValueChanged.AddListener(delegate { valChanged(); });
        hardnesssl.onValueChanged.AddListener(delegate { valChanged(); });
    }
    private void valChanged()
    {
        changed = true;
    }
    private void CreateTree()
    {
        LightConsumption = leavesSizesl.value * trunksl.value;
        waterConsumption = (rootType + 1) * (Weight / 10.0f);
        Weight = (recsl.value * trunksl.value * floorhsl.value * leavesSizesl.value) * 100.0f;
        Height = (floorhsl.value) * 100.0f;

        range = (1 - hardness) * 25.0f;
        survivalpct = hardness * 100.0f;

        lightconsumptiontxt.text = "Light Consumption: " + LightConsumption.ToString("F2");
        waterconsumptiontxt.text = "Water Consumption: " + waterConsumption.ToString("F2");

        heighttxt.text = "Height: " + Height.ToString("F2");
        weighttxt.text = "Weight: " + Weight.ToString("F2");

        rectxt.text = "Recursion Level: " + recsl.value.ToString("F2");
        trunktxt.text = "Trunk Thickness: " + trunksl.value.ToString("F2");
        floorhtxt.text = "Floor Height: " + floorhsl.value.ToString("F2");
        firstbranchtxt.text = "First Branch Height: " + firstbranchsl.value.ToString("F2");
        twistinestxt.text = "Twistiness: " + twistinessl.value.ToString("F2");
        branchdensitytxt.text = "Branch Density: " + branchdensitysl.value.ToString("F2");
        leavesSizetxt.text = "Leaves Size: " + leavesSizesl.value.ToString("F2");
        seedtxt.text = "Distortion: " + distortionsl.value.ToString("F2");
        hardnesstxt.text = "Hardness: " + hardnesssl.value.ToString("F2");

        if (seedNum != 1)
        {
            seedSurvivaltxt.text = "Seed Survival Percentage: " + survivalpct.ToString("F2");
            rangetxt.text = "Seed Range: " + range.ToString("F2");
        }
        else
        {
            seedSurvivaltxt.text = "Seed Survival Percentage: N/A%";
            rangetxt.text = "Seed Range: N/A%";
        }

        hardness = hardnesssl.value;
        generator._recursionLevel = (int)recsl.value;
        generator._trunkThickness = trunksl.value;
        generator._floorHeight = floorhsl.value;
        generator._firstBranchHeight = firstbranchsl.value;
        generator._twistiness = twistinessl.value;
        generator._branchDensity = branchdensitysl.value;
        generator._leavesSize = leavesSizesl.value;
        generator._distorsionCone = distortionsl.value;
        generator.gen();
        generator.tree.transform.parent = this.transform;
        generator.tree.transform.localPosition = Vector3.zero;
    }
    public void SetDetails()
    {
        manager.PlantDetails[ui.selected2, 0] = LightConsumption;
        manager.PlantDetails[ui.selected2, 1] = waterConsumption;
        manager.PlantDetails[ui.selected2, 2] = Weight;
        manager.PlantDetails[ui.selected2, 3] = Height; 
        
        
        manager.PlantDetails[ui.selected2, 4] = firstbranchsl.value;
        manager.PlantDetails[ui.selected2, 5] = distortionsl.value;
        manager.PlantDetails[ui.selected2, 6] = trunksl.value;
        manager.PlantDetails[ui.selected2, 7] = floorhsl.value;
        manager.PlantDetails[ui.selected2, 8] = twistinessl.value;
        manager.PlantDetails[ui.selected2, 9] = leavesSizesl.value;
        manager.PlantDetails[ui.selected2, 10] = hardnesssl.value;

        manager.PlantDetails[ui.selected2, 11] = genderNum;
        manager.PlantDetails[ui.selected2, 12] = seedNum;
    }
    public void genderRButton()
    {
        try
        {
            if (genderNum < GenderTextStr.Length)
            {
                genderNum++;
            }
        }
        catch
        {

        }
    }
    public void genderLButton()
    {
        try
        {
            if (genderNum - 1 > 0)
            {
                genderNum--;
            }
        }
        catch
        {

        }
    }
    public void seedRButton()
    {
        try
        {
            if (seedNum < SeedTextStr.Length)
            {
                seedNum++;
            }
        }
        catch
        {

        }
    }
    public void seedLButton()
    {
        try
        {
            if (seedNum - 1 > 0)
            {
                seedNum--;
            }
        }
        catch
        {

        }
    }
    #endregion
}
