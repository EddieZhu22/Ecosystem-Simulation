using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreatureEditor : MonoBehaviour
{
    public GameManager manager;
    public GameUI ui;

    [Header("Amount of Body Parts")]
    public int legs = 4;
    public int eyes = 5;
    public int heads = 1;

    [Header("Height Settings")]
    public float torsoHeight;
    public float neckHeight;
    public float totalWeight;
    public float totalHeight;
    public float energyOutput;

    [Header("Component Objects")]
    public GameObject[] LegComponents;
    public Transform Torso;
    public GameObject[] Heads;
    public GameObject[] Eyes;
    public GameObject[] Legs;

    [Header("Positions")]
    public Transform headHeight;
    public Vector3 TorsoDimensions, HeadPosition;

    [Header("Weights")]
    public float[] LegWeights;
    public float[] HeadWeights;
    public float[] EyeWeights;
    public float[] minSpeed;
    public float[] maxSpeed;
    public float storage;
    [Header("Text Strings/UI")]
    public string[] LegsTextStr;
    public string[] HeadsTextStr;
    public InputField[] TorsoDimensionsText, HeadPositionText;
    public Slider legsSlider, eyesSlider, torsoHeightSlider, speedSlider, lookRadiusSlider, maxOffSpringSlider, sizeSlider, colorSlider, urgeSlider;
    public TMP_Text LegsText, HeadsText;
    public Text Speed, LookRadius, maxOffspring, Size, ReproductiveUrge, Storage;
    public InputField[] inputs;
    public Toggle predatorToggle;

    [HideInInspector]
    public int legNum = 1, legprevnum, headNum = 1, headprevnum, neckprevnum, predator, diet;
    [Header("Materials")]
    public Material mat;
    public GameObject Creature;
    void Start()
    {
        //SortLegs();
        SortHeads();
    }
    void Update()
    {
        Creature.transform.localScale = new Vector3(sizeSlider.value / 20, sizeSlider.value / 20, sizeSlider.value / 20);
        for (int i = 0; i < Creature.GetComponent<CreatureProceduralAnimation>().moveDist.Length; i++)
        {
            Creature.GetComponent<CreatureProceduralAnimation>().moveDist[i] = 0;
        }
        Creature.GetComponent<CreatureProceduralAnimation>().Set();
        // methods
        UI();
        SortLegs();
        SortHeads();
        SortNumEyes();
        SortNumLegs();
        SortTorsoHeight();
        //SetDetails();
    }
    public void SetDetails()
    {
        manager.CreatureDetails[ui.selected].Clear();    
        /*0*/manager.CreatureDetails[ui.selected].Add("energy", energyOutput);
        /*1*/manager.CreatureDetails[ui.selected].Add("weight", totalWeight);
        /*2*/manager.CreatureDetails[ui.selected].Add("height", totalHeight);

        /*3*/manager.CreatureDetails[ui.selected].Add("speed", speedSlider.value);
        /*4*/manager.CreatureDetails[ui.selected].Add("look radius", lookRadiusSlider.value);
        /*5*/manager.CreatureDetails[ui.selected].Add("max offspring", maxOffSpringSlider.value);
        /*6*/manager.CreatureDetails[ui.selected].Add("size", sizeSlider.value);
        /*7*/manager.CreatureDetails[ui.selected].Add("torso dimensions x", TorsoDimensions.x);
        /*8*/manager.CreatureDetails[ui.selected].Add("torso dimensions y", TorsoDimensions.y);
        /*9*/manager.CreatureDetails[ui.selected].Add("torso dimensions z", TorsoDimensions.z);
        /*10*/manager.CreatureDetails[ui.selected].Add("head position x", HeadPosition.x);
        /*11*/manager.CreatureDetails[ui.selected].Add("head position y", HeadPosition.y);
        /*12*/manager.CreatureDetails[ui.selected].Add("head position z", HeadPosition.z);

        /*13*/manager.CreatureDetails[ui.selected].Add("eye color", colorSlider.value);
        /*14*/manager.CreatureDetails[ui.selected].Add("storage", storage);
        /*15*/manager.CreatureDetails[ui.selected].Add("reproductive urge", urgeSlider.value);

        /*16*/manager.CreatureDetails[ui.selected].Add("head", headNum);
        /*17*/manager.CreatureDetails[ui.selected].Add("leg", legNum);
        /*18*/manager.CreatureDetails[ui.selected].Add("legs", legs);
        /*19*/manager.CreatureDetails[ui.selected].Add("eyes", eyes);
        /*20*/manager.CreatureDetails[ui.selected].Add("torso height", torsoHeight);
        /*21*/manager.CreatureDetails[ui.selected].Add("is predator", predator);
        /*22*/manager.CreatureDetails[ui.selected].Add("diet", diet);
    }
    void UI()
    {
        Torso.localScale = new Vector3(TorsoDimensions.x,TorsoDimensions.y,TorsoDimensions.z);
        TorsoDimensions = new Vector3(float.Parse(TorsoDimensionsText[0].text), float.Parse(TorsoDimensionsText[1].text), float.Parse(TorsoDimensionsText[2].text));
        try
        {
            HeadPosition = new Vector3(float.Parse(HeadPositionText[0].text), float.Parse(HeadPositionText[1].text), float.Parse(HeadPositionText[2].text));
        }
        catch { }
        storage = TorsoDimensions.x * TorsoDimensions.y * TorsoDimensions.x * TorsoDimensions.z / 100; 
        HeadsText.text = HeadsTextStr[headNum - 1];
        LegsText.text = LegsTextStr[legNum - 1];
        eyes = Mathf.RoundToInt(eyesSlider.value);
        legs = Mathf.RoundToInt(legsSlider.value);
        energyOutput = sizeSlider.value * (speedSlider.value + lookRadiusSlider.value);
        torsoHeight = torsoHeightSlider.value;
        totalWeight = (LegWeights[legNum - 1] * legs + HeadWeights[headNum - 1] + EyeWeights[eyes - 1]) * sizeSlider.value + storage;
        totalHeight = headHeight.position.y / 1000;
        speedSlider.minValue = minSpeed[legNum - 1] * legs / totalWeight;
        speedSlider.maxValue = maxSpeed[legNum - 1] * legs / totalWeight;
        lookRadiusSlider.minValue = (eyes * 4) - 2;
        lookRadiusSlider.maxValue = 4 * eyes + 1;
        maxOffSpringSlider.minValue = 2;
        maxOffSpringSlider.maxValue = Mathf.RoundToInt(totalWeight / 30);
        Speed.text = "Speed: " + speedSlider.value;
        Storage.text = "Storage: " + storage;
        LookRadius.text = "Look Distance: " + lookRadiusSlider.value;
        maxOffspring.text = "Maximum OffSpring: " + maxOffSpringSlider.value;
        Size.text = "Size: " + sizeSlider.value;
        ReproductiveUrge.text = "Reprocutive Urge: " + urgeSlider.value;
        inputs[0].placeholder.GetComponent<Text>().text = (totalWeight).ToString() + " kg";
        inputs[1].placeholder.GetComponent<Text>().text = (totalHeight * 5).ToString() + " m";
        inputs[2].placeholder.GetComponent<Text>().text = (energyOutput).ToString() + " E/day";

        if (predatorToggle.isOn == false)
        {
            mat.color = new Color(0, Mathf.Abs(colorSlider.value), 0);
            predator = 0;
            diet = 0;
        }
        if (predatorToggle.isOn == true)
        {
            mat.color = new Color(Mathf.Abs(colorSlider.value), 0, 0);
            diet = 1;
            predator = 1;
        }
        if (manager.settings.showCreatureDetails == true)
        {
            Torso.GetComponent<Renderer>().material = mat;
        }
        else
        {
            Torso.GetComponent<Renderer>().material = Heads[0].GetComponent<Renderer>().material;
        }
    }
    void SortLegs()
    {
        if (legprevnum != legNum)
        {
            for (int i = 0; i < LegComponents.Length; i++)
            {
                if (LegComponents[i].tag == "Leg " + legNum.ToString())
                {
                    LegComponents[i].SetActive(true);
                }
                else
                {
                    LegComponents[i].SetActive(false);
                }
            }
            for (int i = 0; i < Legs.Length; i++)
            {
                Legs[i].SetActive(true);
            }
            //Legs = GameObject.FindGameObjectsWithTag("edleg");

            legprevnum = legNum;
        }
    }
    void SortHeads()
    {
        if (headprevnum != headNum)
        {
            for (int i = 0; i < Heads.Length; i++)
            {
                if (Heads[i].tag == "Head " + headNum.ToString())
                {
                    Heads[i].SetActive(true);
                }
                else
                {
                    Heads[i].SetActive(false);
                }
            }
            //done = true;
            headprevnum = headNum;
        }
        for (int i = 0; i < Heads.Length; i++)
        {
            Heads[i].transform.parent.localPosition = HeadPosition;
        }
    }

    void SortNumEyes()
    {
        for (int i = 0; i < Eyes.Length; i++)
        {
            if (eyes > i)
            {
                Eyes[i].SetActive(true);
            }
            else
            {
                Eyes[i].SetActive(false);
            }
        }
    }
    void SortNumLegs()
    {
        for (int i = 0; i < Legs.Length; i++)
        {
            if (legs > i)
            {
                Legs[i].SetActive(true);
            }
            else
            {
                Legs[i].SetActive(false);
            }
        }
    }
    void SortTorsoHeight()
    {
        Creature.GetComponent<CreatureProceduralAnimation>().offset = torsoHeight;
    }
    public void headRButton()
    {
        try
        {
            if (headNum < HeadsTextStr.Length)
            {
                headNum++;
            }
        }
        catch
        {

        }
    }
    public void headLButton()
    {
        try
        {
            if (headNum - 1 > 0)
            {
                headNum--;
            }
        }
        catch
        {

        }
    }
    public void legRButton()
    {
        try
        {
            if (legNum < LegsTextStr.Length)
            {
                legNum++;
            }
        }
        catch
        {

        }
    }
    public void legLButton()
    {
        try
        {
            if (legNum - 1 > 0)
            {
                legNum--;
            }
        }
        catch
        {

        }
    }
}
