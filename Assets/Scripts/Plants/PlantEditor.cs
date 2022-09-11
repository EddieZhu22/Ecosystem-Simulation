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
    public float seedType;
    public int seedNum = 1;
    public int genderNum = 1;

    [Header("UI")]
    public Slider recsl;
    public Slider trunksl;
    public Slider floorhsl;
    public Slider firstbranchsl;
    public Slider twistinessl;
    public Slider branchdensitysl;
    public Slider leavesSizesl;
    public Slider seedsl;
    public Text rectxt, trunktxt, floorhtxt, firstbranchtxt, twistinestxt, branchdensitytxt, leavesSizetxt, seedtxt, lightconsumptiontxt, waterconsumptiontxt, weighttxt, heighttxt;
    public TMP_Text GenderText, SeedText;
    public string[] SeedTextStr;
    public string[] GenderTextStr;

    private bool changed;
    void Start()
    {
        init();
    }

    void Update()
    {

        SeedText.text = SeedTextStr[seedNum - 1];
        GenderText.text = GenderTextStr[genderNum - 1];

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
        seedsl.onValueChanged.AddListener(delegate { valChanged(); });
        seedsl.value = 1;
    }
    private void valChanged()
    {
        changed = true;
    }
    private void CreateTree()
    {
        LightConsumption = leavesSizesl.value * trunksl.value;
        waterConsumption = (rootType + 1) * (Weight / 10);
        Weight = (recsl.value * trunksl.value * floorhsl.value * firstbranchsl.value * branchdensitysl.value * leavesSizesl.value) * 100;
        Height = (firstbranchsl.value * floorhsl.value) * 100;

        lightconsumptiontxt.text = "Light Consumption: " + LightConsumption.ToString();
        waterconsumptiontxt.text = "Water Consumption: " + waterConsumption.ToString();
        heighttxt.text = "Height: " + Height.ToString();
        weighttxt.text = "Weight: " + Weight.ToString();

        rectxt.text = "Recursion Level: " + recsl.value;
        trunktxt.text = "Trunk Thickness: " + trunksl.value;
        floorhtxt.text = "Floor Height: : " + floorhsl.value;
        firstbranchtxt.text = "First Branch Height: " + firstbranchsl.value;
        twistinestxt.text = "Twistiness: " + twistinessl.value;
        branchdensitytxt.text = "Branch Density: " + branchdensitysl.value;
        leavesSizetxt.text = "Leaves Size: " + leavesSizesl.value;
        seedtxt.text = "Seed: " + seedsl.value;

        generator._recursionLevel = (int)recsl.value;
        generator._trunkThickness = trunksl.value;
        generator._floorHeight = floorhsl.value;
        generator._firstBranchHeight = firstbranchsl.value;
        generator._twistiness = twistinessl.value;
        generator._branchDensity = branchdensitysl.value;
        generator._leavesSize = leavesSizesl.value;
        generator.seed = seedsl.value;
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

        manager.PlantDetails[ui.selected2, 4] = seedsl.value;
        manager.PlantDetails[ui.selected2, 5] = recsl.value;
        manager.PlantDetails[ui.selected2, 6] = trunksl.value;
        manager.PlantDetails[ui.selected2, 7] = floorhsl.value;
        manager.PlantDetails[ui.selected2, 9] = firstbranchsl.value;
        manager.PlantDetails[ui.selected2, 9] = twistinessl.value;
        manager.PlantDetails[ui.selected2, 10] = branchdensitysl.value;
        manager.PlantDetails[ui.selected2, 11] = leavesSizesl.value;

        manager.PlantDetails[ui.selected2, 12] = genderNum;
        manager.PlantDetails[ui.selected2, 13] = seedNum;
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
