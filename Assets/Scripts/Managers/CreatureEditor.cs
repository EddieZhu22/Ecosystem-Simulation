using System.Collections;
using System.Collections.Generic;
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
    public GameObject[] Torsos;
    public GameObject[] Heads;
    public GameObject[] Necks;
    public GameObject[] Eyes;
    public GameObject[] Legs;

    [Header("Positions")]
    public Transform headHeight;
    public Vector3[] HeadPos;
    public Vector3[] TorsoScale;
    public Vector3[] TorsoPos;

    [Header("Weights")]
    public float[] LegWeights;
    public float[] TorsoWeights;
    public float[] HeadWeights;
    public float[] NeckWeights;
    public float[] EyeWeights;
    public float[] minSpeed;
    public float[] maxSpeed;
    public float[] minStorage;
    public float[] maxStorage;

    [Header("Text Strings/UI")]
    public string[] LegsTextStr;
    public string[] TorsoTextStr;
    public string[] HeadsTextStr;
    public string[] NeckTextStr;
    public Slider legsSlider, eyesSlider, torsoHeightSlider, speedSlider, lookRadiusSlider, maxOffSpringSlider, sizeSlider, storageSlider, colorSlider, urgeSlider;
    public TMP_Text LegsText, TorsoText, HeadsText, NeckText;
    public Text Speed, LookRadius, maxOffspring, Size, ReproductiveUrge, Storage;
    public InputField[] inputs;
    public Toggle predatorToggle;

    [HideInInspector]
    public int legNum = 1, legprevnum, headNum = 1, headprevnum, neckNum = 1, neckprevnum, torsoNum = 1, torsoprevnum, predator, diet;
    [Header("Materials")]
    public Material mat;
    private GameObject Creature;
    void Start()
    {
        ui = GameObject.Find("Canvas").GetComponent<GameUI>();
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        Creature = GameObject.Find("Creature");
        SortLegs();
        SortHeads();
    }
    void Update()
    {
        Creature.transform.localScale = new Vector3(sizeSlider.value / 20, sizeSlider.value / 20, sizeSlider.value / 20);
        for (int i = 0; i < Creature.GetComponent<CreatureProceduralAnimation>().moveDist.Length; i++)
        {
            Creature.GetComponent<CreatureProceduralAnimation>().moveDist[i] = 0;
        }
        // methods
        UI();
        SortLegs();
        SortHeads();
        SortNecks();
        SortTorso();
        SortNumEyes();
        SortNumLegs();
        SortTorsoHeight();
        //SetDetails();
    }
    public void SetDetails()
    {
        manager.CreatureDetails[ui.selected, 0] = energyOutput;
        manager.CreatureDetails[ui.selected, 1] = totalWeight;
        manager.CreatureDetails[ui.selected, 2] = totalHeight;

        manager.CreatureDetails[ui.selected, 3] = speedSlider.value;
        manager.CreatureDetails[ui.selected, 4] = lookRadiusSlider.value;
        manager.CreatureDetails[ui.selected, 5] = maxOffSpringSlider.value;
        manager.CreatureDetails[ui.selected, 6] = sizeSlider.value;
        manager.CreatureDetails[ui.selected, 7] = colorSlider.value;
        manager.CreatureDetails[ui.selected, 8] = storageSlider.value;
        manager.CreatureDetails[ui.selected, 9] = urgeSlider.value;

        //GETTING MUTATED!
        manager.CreatureDetails[ui.selected, 10] = headNum;
        manager.CreatureDetails[ui.selected, 11] = neckNum;
        manager.CreatureDetails[ui.selected, 12] = legNum;
        manager.CreatureDetails[ui.selected, 13] = legs;
        manager.CreatureDetails[ui.selected, 14] = eyes;
        manager.CreatureDetails[ui.selected, 15] = torsoHeight;
        manager.CreatureDetails[ui.selected, 16] = predator;
        manager.CreatureDetails[ui.selected, 17] = diet;
    }
    void UI()
    {
        HeadsText.text = HeadsTextStr[headNum - 1];
        NeckText.text = NeckTextStr[neckNum - 1];
        TorsoText.text = TorsoTextStr[torsoNum - 1];
        LegsText.text = LegsTextStr[legNum - 1];
        eyes = Mathf.RoundToInt(eyesSlider.value);
        legs = Mathf.RoundToInt(legsSlider.value);
        energyOutput = sizeSlider.value * (speedSlider.value + lookRadiusSlider.value);
        torsoHeight = torsoHeightSlider.value;
        totalWeight = (LegWeights[legNum - 1] * legs + HeadWeights[headNum - 1] + EyeWeights[eyes - 1] + TorsoWeights[torsoNum - 1] + NeckWeights[neckNum - 1]) * sizeSlider.value + storageSlider.value;
        totalHeight = headHeight.position.y;
        speedSlider.minValue = minSpeed[legNum - 1] * legs / totalWeight;
        speedSlider.maxValue = maxSpeed[legNum - 1] * legs / totalWeight;
        storageSlider.minValue = minStorage[torsoNum - 1];
        storageSlider.maxValue = maxStorage[torsoNum - 1];
        lookRadiusSlider.minValue = (eyes * 4) - 2;
        lookRadiusSlider.maxValue = 4 * eyes + 1;
        maxOffSpringSlider.minValue = 2;
        maxOffSpringSlider.maxValue = Mathf.RoundToInt(totalWeight / 30);
        mat.color = new Color(0, Mathf.Abs(colorSlider.value), 0);
        Speed.text = "Speed: " + speedSlider.value;
        Storage.text = "Storage: " + storageSlider.value;
        LookRadius.text = "Look Distance: " + lookRadiusSlider.value;
        maxOffspring.text = "Maximum OffSpring: " + maxOffSpringSlider.value;
        Size.text = "Size: " + sizeSlider.value;
        ReproductiveUrge.text = "Reprocutive Urge: " + urgeSlider.value;
        inputs[0].placeholder.GetComponent<Text>().text = (totalWeight).ToString() + " kg";
        inputs[1].placeholder.GetComponent<Text>().text = (totalHeight * 5).ToString() + " m";
        inputs[2].placeholder.GetComponent<Text>().text = (energyOutput).ToString() + " E/day";
        if (predatorToggle.isOn == false)
        {
            predator = 0;
        }
        if (predatorToggle.isOn == true)
        {
            predator = 1;
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
            Legs = GameObject.FindGameObjectsWithTag("edleg");

            legprevnum = legNum;
            Creature.GetComponent<CreatureProceduralAnimation>().Editor();
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
            Heads[i].transform.parent.localPosition = HeadPos[neckNum - 1];
        }
    }
    void SortTorso()
    {
        Torsos[0].transform.localScale = TorsoScale[torsoNum - 1];
        Torsos[0].transform.localPosition = TorsoPos[torsoNum - 1];
    }
    void SortNecks()
    {
        if (neckprevnum != neckNum)
        {
            for (int i = 0; i < Necks.Length; i++)
            {
                if (Necks[i].tag == "Neck " + neckNum.ToString())
                {
                    Necks[i].SetActive(true);
                }
                else
                {
                    Necks[i].SetActive(false);
                }
            }
            //done = true;
            headprevnum = headNum;
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
    public void neckRButton()
    {
        try
        {
            if (neckNum < NeckTextStr.Length)
            {
                neckNum++;
            }
        }
        catch
        {

        }
    }
    public void neckLButton()
    {
        try
        {
            if (neckNum - 1 > 0)
            {
                neckNum--;
            }
        }
        catch
        {

        }
    }
    public void torsoRButton()
    {
        try
        {
            if (torsoNum < TorsoTextStr.Length)
            {
                torsoNum++;
            }
        }
        catch
        {

        }
    }
    public void torsoLButton()
    {
        try
        {
            if (torsoNum - 1 > 0)
            {
                torsoNum--;
            }
        }
        catch
        {

        }
    }
}
