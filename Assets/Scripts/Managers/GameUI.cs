using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
public class GameUI : MonoBehaviour
{
    [SerializeField] GameObject[] mainContent;

    [SerializeField] private float currSpeed;

    public Dropdown cameraSettings, screenRes;

    public Toggle NN;

    public Slider GameSpeed, MutationStrength, MutationChance, Eyes, Legs, Torso, MutationStrength2, MutationChance2, TerrainStrength, TerrainLength, TerrainWidth;

    public InputField seedInput, numOfPlants, numOfAnimals;

    public Button apply, replace, apply2, replace2;

    public Text avgData, StrengthLabel, ChanceLabel, StrengthLabel2, ChanceLabel2, EyesLabel, LegsLabel, TorsoLabel;

    public TMP_Text inspectorText;

    public TMP_Text Time_Text, Date_Text, Stats, Stats2;

    public Animator anim;
    public GameObject pause, play, terrain, transitionObject, editorSettings, editorSettings2, mainSettings, SlotSprite, InspectorSettings;
    public GameManager manager;
    public CreatureEditor editor;
    public PlantEditor editor2;

    public bool isPlay, isTransition, vacant, vacant2, spawned, spawned2, isCollapsed, dataCollapsed;
    private bool[] isPopupVisible = new bool[5];

    public Sprite[] Icons, Icons2;
    public GameObject[] Slots, Slots2, Data;
    private GameObject[] creatures, enviroment;

    public GlobalUISound globalUISound;


    public int scene, selected, selected2, num, num2;

    public Camera cam1, cam2, cam3, IconCam, IconCam2;

    public Creature inspectorCreature;

    public Plant inspectorPlant;

    public PageView Page;

    public int type;
    public enum PageView
    {
        None,
        Terrain,
        Creature,
        Plants,
        Data,
        Settings,
    }
    private void Start()
    {
        //GameManager = GameObject.Find("GameManager");
        editorSettings.SetActive(false);
        mainSettings.SetActive(true);
        GameSpeed.value = 1f;
        currSpeed = 1;
        cam1.enabled = true;
        cam2.enabled = false;
        cam3.enabled = false;
        scene = 0;
        InvokeRepeating("UpdateTime", 0.1f, 0.1f);
        isPlay = true;
    }
    void Update()
    {

        manager.Gamespeed = GameSpeed.value;
        Spawn();
        checkScene();
    }
    private void UpdateTime()
    {
        Time_Text.text = manager.tManager.hours.ToString() + ":" + manager.tManager.minutes.ToString("00");
        Date_Text.text = "Year " + manager.tManager.years.ToString() + ", Day " + manager.tManager.days.ToString();
    }

    public void Collapse()
    {
        isCollapsed = (isCollapsed == true) ? isCollapsed = false : isCollapsed = true;
        if (isCollapsed)
        {
            anim.SetBool("Collapsed", true);
        }
        else
        {
            anim.SetBool("Collapsed", false);
        }
    }
    private void Spawn()
    {
        if (spawned == true)
        {
            if (num <= int.Parse(numOfPlants.text))
            {
                manager.SpawnTrees();
                num++;
            }
            else
            {
                spawned = false;
            }
        }
        if (spawned2 == true)
        {
            for (int i = 0; i < int.Parse(numOfAnimals.text); i++)
            {
                manager.SpawnAnimals();
            }
            spawned2 = false;
        }
    }

    private void checkScene()
    {
        if (scene == 0)
        {
            //terrain.GetComponent<TerrainGenerator>().heightMapSettings.noiseSettings.seed = int.Parse(seedInput.text);

            SideMenu();
            GameSpeed.interactable = true;
            //strengthText
            StrengthLabel.text = "Mutation Strength: " + Mathf.RoundToInt(MutationStrength.value * 100) + "%";
            ChanceLabel.text = "Mutation Chance: " + Mathf.RoundToInt(MutationChance.value * 100) + "%";
            manager.CreatureMutationChance = MutationChance.value;
            manager.CreatureMutationStrength = MutationStrength.value;

            StrengthLabel2.text = "Mutation Strength: " + Mathf.RoundToInt(MutationStrength2.value * 100) + "%";
            ChanceLabel2.text = "Mutation Chance: " + Mathf.RoundToInt(MutationChance2.value * 100) + "%";
            manager.PlantMutationChance = MutationChance2.value;
            manager.PlantMutationStrength = MutationStrength2.value;
            // Stats
            //Debug.Log(manager.CreatureDetails[selected,0]);
            if (manager.CreatureDetails[selected].Count > 0)
            {
                if (manager.CreatureDetails[selected]["energy"] > 0)
                    Stats.text = "Energy Output: " + manager.CreatureDetails[selected]["energy"] +
              "\nTotal Weight: " + manager.CreatureDetails[selected]["weight"] +
              "\nTotal Height: " + manager.CreatureDetails[selected]["height"] +
              "\nSpeed: " + manager.CreatureDetails[selected]["speed"] +
              "\nLook Radius: " + manager.CreatureDetails[selected]["look radius"] +
              "\nFertility: " + manager.CreatureDetails[selected]["max offspring"] +
              "\nSize: " + manager.CreatureDetails[selected]["size"] +
              "\nTorso Dimensions X: " + manager.CreatureDetails[selected]["torso dimensions x"] +
              "\nTorso Dimensions Y: " + manager.CreatureDetails[selected]["torso dimensions y"] +
              "\nTorso Dimensions Z: " + manager.CreatureDetails[selected]["torso dimensions z"] +
              "\nHead Position X: " + manager.CreatureDetails[selected]["head position x"] +
              "\nHead Position Y: " + manager.CreatureDetails[selected]["head position y"] +
              "\nHead Position Z: " + manager.CreatureDetails[selected]["head position z"] +
              "\nEye Color: " + manager.CreatureDetails[selected]["eye color"] +
              "\nStorage: " + manager.CreatureDetails[selected]["storage"] +
              "\nReproductive Urge: " + manager.CreatureDetails[selected]["reproductive urge"] +
              "\nHead: " + manager.CreatureDetails[selected]["head"] +
              "\nLeg: " + manager.CreatureDetails[selected]["leg"] +
              "\nLegs: " + manager.CreatureDetails[selected]["legs"] +
              "\nEyes: " + manager.CreatureDetails[selected]["eyes"] +
              "\nTorso Height: " + manager.CreatureDetails[selected]["torso height"] +
              "\nIs Predator: " + manager.CreatureDetails[selected]["is predator"] +
              "\nDiet: " + manager.CreatureDetails[selected]["diet"];
            }
            else
            {
                Stats.text = "";
            }
            /*string gender = "";
            switch ((int)manager.PlantDetails[selected, 11])
            {
                case 0:
                    gender = "Asexual";
                    break;
                case 1:
                    gender = "Male or Female";
                    break;
                case 2:
                    gender = "Both Male and Female";
                    break;
                default:
                    gender = "";
                    break;
            }*/
            string seedType = "";
            switch ((int)manager.PlantDetails[selected2, 12])
            {
                case 1:
                    seedType = "Fruit Seeds";
                    break;
                case 2:
                    seedType = "Flying Seeds";
                    break;
                default:
                    seedType = "";
                    break;
            }
            if (manager.PlantDetails[selected2, 0] > 0)
            {
                Stats2.text = "Light Consumption: " + manager.PlantDetails[selected2, 0] +
                "\nWater Consumption: " + manager.PlantDetails[selected2, 1] +
                "\nWeight: " + manager.PlantDetails[selected2, 2] +
                "\nHeight: " + manager.PlantDetails[selected2, 3] +
                "\nFirst Branch Height: " + manager.PlantDetails[selected2, 4] +
                "\nDistortion Level: " + manager.PlantDetails[selected2, 5] +
                "\nTrunk Thickness: " + manager.PlantDetails[selected2, 6] +
                "\nFloor Height: " + manager.PlantDetails[selected2, 7] +
                "\nTwistiness: " + manager.PlantDetails[selected2, 8] +
                "\nLeaves Size: " + manager.PlantDetails[selected2, 9] +
                "\nHardness: " + manager.PlantDetails[selected2, 10] +
                "\nSeed Type: " + seedType;
            }
            else
            {
                Stats2.text = "";
            }

        }
        if (scene == 1)
        {
            Time.timeScale = 1;
            GameSpeed.value = 1f;
            EyesLabel.text = "Eyes: " + Mathf.RoundToInt(Eyes.value);
            LegsLabel.text = "Legs: " + Mathf.RoundToInt(Legs.value);
            TorsoLabel.text = "Torso Height: " + Torso.value;

        }
        if (scene == 2)
        {
        }
        if (scene == 3)
        {
            InspectorSettings.SetActive(true);
            editorSettings.SetActive(false);
            editorSettings2.SetActive(false);
            mainSettings.SetActive(false);
            SlotSprite.SetActive(false);

            if (type == 6)
            {
                string creatureInfo = "Time Alive: " + inspectorCreature.timeAlive +
                                      "\nGeneration: " + inspectorCreature.generation +
                                      "\nFound Mate: " + inspectorCreature.foundMate +
                                      "\nMated: " + inspectorCreature.mated +
                                      "\nReady To Mate: " + inspectorCreature.readyToMate +
                                      "\nCurrent Action: " + inspectorCreature.action.ToString() +
                                      "\nMax Storage: " + inspectorCreature.maxStorage +
                                      "\nFood: " + inspectorCreature.food +
                                      "\nWater: " + inspectorCreature.water +
                                      "\nLook Radius: " + inspectorCreature.lookRad +
                                      "\n--Genes--" +
                                      "\nEnergy Output: " + inspectorCreature.genes.Genes["energy"] +
                                      "\nTotal Weight: " + inspectorCreature.genes.Genes["weight"] +
                                      "\nTotal Height: " + inspectorCreature.genes.Genes["height"] +
                                      "\nSpeed: " + inspectorCreature.genes.Genes["speed"] +
                                      "\nLook Radius: " + inspectorCreature.genes.Genes["look radius"] +
                                      "\nFertility: " + inspectorCreature.genes.Genes["max offspring"] +
                                      "\nSize: " + inspectorCreature.genes.Genes["size"] +
                                      "\nTorso Dimensions X: " + inspectorCreature.genes.Genes["torso dimensions x"] +
                                      "\nTorso Dimensions Y: " + inspectorCreature.genes.Genes["torso dimensions y"] +
                                      "\nTorso Dimensions Z: " + inspectorCreature.genes.Genes["torso dimensions z"] +
                                      "\nHead Position X: " + inspectorCreature.genes.Genes["head position x"] +
                                      "\nHead Position Y: " + inspectorCreature.genes.Genes["head position y"] +
                                      "\nHead Position Z: " + inspectorCreature.genes.Genes["head position z"] +
                                      "\nEye Color: " + inspectorCreature.genes.Genes["eye color"] +
                                      "\nStorage: " + inspectorCreature.genes.Genes["storage"] +
                                      "\nReproductive Urge: " + inspectorCreature.genes.Genes["reproductive urge"] +
                                      "\nHead: " + inspectorCreature.genes.Genes["head"] +
                                      "\nLeg: " + inspectorCreature.genes.Genes["leg"] +
                                      "\nLegs: " + inspectorCreature.genes.Genes["legs"] +
                                      "\nEyes: " + inspectorCreature.genes.Genes["eyes"] +
                                      "\nTorso Height: " + inspectorCreature.genes.Genes["torso height"] +
                                      "\nIs Predator: " + inspectorCreature.genes.Genes["is predator"] +
                                      "\nDiet: " + inspectorCreature.genes.Genes["diet"];

                // Set the text
                inspectorText.text = creatureInfo;
            }
            if (type == 3)  // Replace with the appropriate condition to check if the object is a plant
            {

                string plantInfo = "Generation: " + inspectorPlant.generation +
                       "\nAge: " + inspectorPlant.age +
                       "\nEnergy: " + inspectorPlant.energy +
                       "\nReady to Mate: " + inspectorPlant.readyToMate +
                       "\nLight Consumption: " + inspectorPlant.details.genes[0] +
                       "\nWater Consumption: " + inspectorPlant.details.genes[1] +
                       "\nWeight: " + inspectorPlant.details.genes[2] +
                       "\nHeight: " + inspectorPlant.details.genes[3] +
                       "\nFirst Branch Height: " + inspectorPlant.details.genes[4] +
                       "\nDistortion: " + inspectorPlant.details.genes[5] +
                       "\nTrunk Thickness: " + inspectorPlant.details.genes[6] +
                       "\nFloor Height: " + inspectorPlant.details.genes[7] +
                       "\nTwistiness: " + inspectorPlant.details.genes[8] +
                       "\nLeaves Size: " + inspectorPlant.details.genes[9] +
                       "\nHardness: " + inspectorPlant.details.genes[10] +
                       "\nGender: " + inspectorPlant.details.genes[11] +
                       "\nSeed Number: " + inspectorPlant.details.genes[12];

                // Set the text
                inspectorText.text = plantInfo;
            }


        }
        if (scene == 0)
        {
            InspectorSettings.SetActive(false);
            editorSettings.SetActive(false);
            editorSettings2.SetActive(false);
            mainSettings.SetActive(true);
            //InspectorSettings.SetActive(false);
            //editorSettings.SetActive(false);
            //mainSettings.SetActive(true);
            //editorSettings2.SetActive(false);
            //cam1.enabled = true;
            //cam2.enabled = false;
            //cam3.enabled = false;
        }
    }
    public void GeneratePlants()
    {
        num = 0;
        spawned = true;
    }
    public void GenerateAnimals()
    {
        num2 = 0;
        spawned2 = true;
    }
    /*
    public void playSim()
    {
        GameSpeed.value = currSpeed;
        isPlay = true;
        // find all objs with pause script
        Pause[] objsToUnPause = GameObject.FindObjectsOfType<Pause>();
        //iterate through pause array
        for (int i = 0; i < objsToUnPause.Length; i++)
        {
            objsToUnPause[i].UnPauseObj();
        }
        pause.SetActive(true);
        play.SetActive(false);
    }
    public void pauseSim()
    {
        currSpeed = GameSpeed.value;

        isPlay = false;
        // find all objs with pause script
        Pause[] objsToPause = GameObject.FindObjectsOfType<Pause>();
        //iterate through pause array
        for (int i = 0; i < objsToPause.Length; i++)
        {
            objsToPause[i].PauseObj();
        }
        pause.SetActive(false);
        play.SetActive(true);
    }*/
    public void EnterCreatureEditor()
    {
        StartCoroutine(transition(1));
        //Debug.Log("success");
    }
    public void ExitcCreatureEditor()
    {
        Icons[selected] = IconCam.gameObject.GetComponent<CreateIcons>().CaptureScreen();
        Slots[selected].gameObject.transform.parent.gameObject.GetComponent<Image>().sprite = Icons[selected];
        editor.SetDetails();
        StartCoroutine(transition(0));
        //Debug.Log("success");
    }
    public void EnterPlantEditor()
    {
        StartCoroutine(transition(2));
        //Debug.Log("success");
    }
    public void ExitPlantEditor()
    {
        Icons2[selected2] = IconCam2.gameObject.GetComponent<CreateIcons>().CaptureScreen();
        Slots2[selected2].gameObject.transform.parent.gameObject.GetComponent<Image>().sprite = Icons2[selected2];
        editor2.SetDetails();
        StartCoroutine(transition(0));
        //Debug.Log("success");
    }
    private bool isTransitioning = false;

    public IEnumerator transition(int sceneNum)
    {
        if (isTransitioning)
        {
            yield break;  // Exit the coroutine
        }
        //pauseSim();
        isTransition = true;
        transitionObject.GetComponent<Animator>().SetBool("transition", true);
        GameSpeed.value = 1f;
        isTransitioning = true;
        yield return new WaitForSeconds(1f);
        scene = sceneNum;
        //smth
        if (scene == 1)
        {
            GameSpeed.value = 1f;
            editorSettings.SetActive(true);
            editorSettings2.SetActive(false);
            mainSettings.SetActive(false);
            cam1.enabled = false;
            cam2.enabled = true;
            cam3.enabled = false;
            SlotSprite.SetActive(false);
        }
        if (scene == 2)
        {
            GameSpeed.value = 1f;
            editorSettings.SetActive(false);
            editorSettings2.SetActive(true);
            mainSettings.SetActive(false);
            cam1.enabled = false;
            cam2.enabled = false;
            cam3.enabled = true;
            SlotSprite.SetActive(false);
        }
        if (scene == 0)
        {
            GameSpeed.value = 1f;
            editorSettings.SetActive(false);
            mainSettings.SetActive(true);
            editorSettings2.SetActive(false);
            cam1.enabled = true;
            cam2.enabled = false;
            cam3.enabled = false;
            SlotSprite.SetActive(false);
        }

        globalUISound.AttachSoundToUI();

        yield return new WaitForSeconds(1f);
        isTransitioning = false;  // Reset the flag
        transitionObject.GetComponent<Animator>().SetBool("transition", false);
    }
    public void SideMenu()
    {
        for (int i = 0; i < mainContent.Length; i++)
        {
            mainContent[i].SetActive(false);
            isPopupVisible[i] = false;
        }
        manager.TerrainTool.enabled = (Page == PageView.Terrain);
        switch (Page)
        {
            case PageView.None:
                foreach(var data in Data) data.SetActive(false);
                SlotSprite.SetActive(false);
                screenRes.gameObject.SetActive(false);
                break;

            case PageView.Terrain:
                mainContent[0].SetActive(true);
                isPopupVisible[0] = true;
                SlotSprite.SetActive(false);
                screenRes.gameObject.SetActive(false);
                foreach (var data in Data) data.SetActive(false);
                break;

            case PageView.Creature:
                for (int i = 0; i < Slots.Length + 1; i++)
                    SlotSprite.SetActive(true);
                mainContent[1].SetActive(true);
                isPopupVisible[1] = true;
                screenRes.gameObject.SetActive(false);


                foreach (var data in Data) data.SetActive(false);
                break;

            case PageView.Plants:
                mainContent[2].SetActive(true);
                isPopupVisible[2] = true;
                screenRes.gameObject.SetActive(false);
                for (int i = 0; i < Slots.Length + 1; i++)
                    SlotSprite.SetActive(true);
                foreach (var data in Data) data.SetActive(false);
                break;

            case PageView.Data:
                mainContent[3].SetActive(true);
                isPopupVisible[3] = true;
                SlotSprite.SetActive(false);
                screenRes.gameObject.SetActive(false);
                foreach (var data in Data) data.SetActive(true);
                break;

            case PageView.Settings:
                mainContent[4].SetActive(true);
                isPopupVisible[4] = true;
                SlotSprite.SetActive(false);
                screenRes.gameObject.SetActive(true);
                foreach (var data in Data) data.SetActive(false);
                break;

            default:
                break;
        }


        if (Slots[selected].gameObject.transform.parent.gameObject.GetComponent<Image>().sprite.name == "UIMask")
        {
            apply.interactable = true;
            replace.interactable = false;
            vacant = true;
        }
        else
        {
            vacant = false;
            apply.interactable = false;
            replace.interactable = true;
        }
        if (Slots2[selected2].gameObject.transform.parent.gameObject.GetComponent<Image>().sprite.name == "UIMask")
        {
            apply2.interactable = true;
            replace2.interactable = false;
            vacant2 = true;
        }
        else
        {
            vacant2 = false;
            apply2.interactable = false;
            replace2.interactable = true;
        }
    }

    public void EnterTerrainBar()
    {
        if (!isPopupVisible[0])
        {
            Page = PageView.Terrain;
            isPopupVisible[0] = true;
        }
        else
        {
            Page = PageView.None;
            isPopupVisible[0] = false;
        }
    }
    public void EnterCreatureBar()
    {
        if (!isPopupVisible[1])
        {
            Page = PageView.Creature;
            isPopupVisible[1] = true;
        }
        else
        {
            Page = PageView.None;
            isPopupVisible[1] = false;
        }
    }
    public void EnterPlantBar()
    {
        if (!isPopupVisible[2])
        {
            Page = PageView.Plants;
            isPopupVisible[2] = true;
        }
        else
        {
            Page = PageView.None;
            isPopupVisible[2] = false;
        }
    }
    public void EnterDataBar()
    {
        if (!isPopupVisible[3])
        {
            Page = PageView.Data;
            isPopupVisible[3] = true;
        }
        else
        {
            Page = PageView.None;
            isPopupVisible[3] = false;
        }
    }
    public void EnterSettingsBar()
    {
        if (!isPopupVisible[4])
        {
            Page = PageView.Settings;
            isPopupVisible[4] = true;
        }
        else
        {
            Page = PageView.None;
            isPopupVisible[4] = false;
        }
    }
    // UNUSED

    public void SetAllInactive()
    {
        creatures = GameObject.FindGameObjectsWithTag("creature");
        enviroment = GameObject.FindGameObjectsWithTag("food");
        for (int i = 0; i < creatures.Length; i++)
        {
            creatures[i].SetActive(false);
        }
        for (int i = 0; i < enviroment.Length; i++)
        {
            enviroment[i].SetActive(false);
        }
    }
    public void SetAllActive()
    {
        for (int i = 0; i < creatures.Length; i++)
        {
            creatures[i].SetActive(true);
        }
        for (int i = 0; i < enviroment.Length; i++)
        {
            enviroment[i].SetActive(true);
        }
    }
    public void SlotSelected()
    {
        switch (Page)
        {
            case PageView.Creature:
                for (int i = 0; i < Slots.Length + 1; i++)
                {
                    //Debug.Log(EventSystem.current.currentSelectedGameObject.name);

                    if (EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.name.Contains(i.ToString()))
                    {
                        selected = i - 1;
                        SlotSprite.transform.position = Slots[i - 1].gameObject.transform.position;
                    }
                }
                break;
            case PageView.Plants:
                for (int i = 0; i < Slots2.Length + 1; i++)
                {
                    //Debug.Log(EventSystem.current.currentSelectedGameObject.name);

                    if (EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.name.Contains(i.ToString()))
                    {
                        selected2 = i - 1;
                        SlotSprite.transform.position = Slots2[i - 1].gameObject.transform.position;
                    }
                }
                break;
        }
    }
    public void reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    // enumerables
}
