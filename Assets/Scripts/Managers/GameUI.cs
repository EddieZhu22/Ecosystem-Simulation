using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public class GameUI : MonoBehaviour
{
    [SerializeField] GameObject[] mainContent;

    [SerializeField] private float currSpeed;

    public Dropdown mainDropDown, cameraSettings;

    public Toggle NN;

    public Slider GameSpeed, MutationStrength, MutationChance, Eyes, Legs, Torso, MutationStrength2, MutationChance2;

    public InputField seedInput, numOfPlants, numOfAnimals;

    public Button apply, replace, apply2, replace2;

    public Text avgData, StrengthLabel, ChanceLabel, EyesLabel, LegsLabel, TorsoLabel;

    public Animator anim;
    public GameObject pause, play, terrain, transitionObject, editorSettings, editorSettings2, mainSettings, SlotSprite, InspectorSettings;
    public GameManager manager;
    public CreatureEditor editor;
    public PlantEditor editor2;

    public bool isPlay, isTransition, vacant, vacant2, spawned, spawned2, isCollapsed;

    public Sprite[] Icons, Icons2;
    public TMP_Text Stats, Stats2;

    public GameObject[] Slots, Slots2;
    private GameObject[] creatures, enviroment;

    public int scene, selected, selected2, num, num2;

    public Camera cam1, cam2, cam3, IconCam, IconCam2;
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
    }
    void Update()
    {

        manager.Gamespeed = GameSpeed.value;

        Spawn();
        checkScene();
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
            if (num2 <= int.Parse(numOfAnimals.text))
            {
                manager.SpawnAnimals();
                num2++;
            }
            else
            {
                spawned2 = false;
            }
        }
    }
    private void checkScene()
    {
        if (scene == 0)
        {
            if (mainDropDown.value == 1 || mainDropDown.value == 2)
            {
                SlotSprite.SetActive(true);
            }
            else
            {
                SlotSprite.SetActive(false);
            }

            //terrain.GetComponent<TerrainGenerator>().heightMapSettings.noiseSettings.seed = int.Parse(seedInput.text);

            SideMenu();

            // Game Speed
            if (isPlay == false)
            {
                GameSpeed.interactable = false;
            }
            else
            {
                GameSpeed.interactable = true;
            }
            //strengthText
            StrengthLabel.text = "Mutation Strength: " + Mathf.RoundToInt(MutationStrength.value * 100) + "%";
            ChanceLabel.text = "Mutation Chance: " + Mathf.RoundToInt(MutationChance.value * 100) + "%";
            // Stats
            //Debug.Log(manager.CreatureDetails[selected,0]);
            Stats.text = "Energy Output: " + manager.CreatureDetails[selected, 0] + "\nTotal Weight:" + manager.CreatureDetails[selected, 1] + "\nTotal Height:" + manager.CreatureDetails[selected, 2] + "\nSpeed: " + manager.CreatureDetails[selected, 3] + "\nLook Radius: " + manager.CreatureDetails[selected, 4] + "\nFertility: " + manager.CreatureDetails[selected, 5] + "\nSize: " + manager.CreatureDetails[selected, 6] + "\nColor: " + manager.CreatureDetails[selected, 7] + "\nReproductive Urge: " + manager.CreatureDetails[selected, 8];
            Stats2.text = "Light Consumption: " + manager.PlantDetails[selected, 0] + "\nWater Consumption:" + manager.PlantDetails[selected, 1] + "\nWeight:" + manager.PlantDetails[selected, 2] + "\nHeight: " + manager.PlantDetails[selected, 3] + "\nRecursion Level: " + manager.PlantDetails[selected, 5] + "\nTrunk Thickness: " + manager.PlantDetails[selected, 6] + "\nFloor Height: " + manager.PlantDetails[selected, 7] + "\nFirst Branch Height: " + manager.PlantDetails[selected, 8] + "\nTwistiness: " + manager.PlantDetails[selected, 9] + "\nBranch Density: " + manager.PlantDetails[selected, 10] + "\nLeaves Size: " + manager.PlantDetails[selected, 11];
        }
        if (scene == 1)
        {
            SlotSprite.SetActive(false);
            Time.timeScale = 1;
            GameSpeed.value = 1f;
            EyesLabel.text = "Eyes: " + Mathf.RoundToInt(Eyes.value);
            LegsLabel.text = "Legs: " + Mathf.RoundToInt(Legs.value);
            TorsoLabel.text = "Torso Height: " + Torso.value;
        }
        if (scene == 2)
        {
            SlotSprite.SetActive(false);
        }
        if (scene == 3)
        {
            InspectorSettings.SetActive(true);
            editorSettings.SetActive(false);
            editorSettings2.SetActive(false);
            mainSettings.SetActive(false);
        }
        if (scene == 0)
        {
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
        //GameSpeed.value = 0;
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
    }
    private void ValueChangeCheck()
    {

    }
    public void EnterCreatureEditor()
    {
        StartCoroutine(transition());
        scene = 1;
        //Debug.Log("success");
    }
    public void ExitcCreatureEditor()
    {
        Icons[selected] = IconCam.gameObject.GetComponent<CreateIcons>().CaptureScreen();
        Slots[selected].gameObject.transform.parent.gameObject.GetComponent<Image>().sprite = Icons[selected];
        editor.SetDetails();
        scene = 0;
        StartCoroutine(transition());
        //Debug.Log("success");
    }
    public void EnterPlantEditor()
    {
        scene = 2;
        StartCoroutine(transition());
        //Debug.Log("success");
    }
    public void ExitPlantEditor()
    {
        Icons2[selected2] = IconCam2.gameObject.GetComponent<CreateIcons>().CaptureScreen();
        Slots2[selected2].gameObject.transform.parent.gameObject.GetComponent<Image>().sprite = Icons2[selected2];
        scene = 0;
        editor2.SetDetails();
        StartCoroutine(transition());
        //Debug.Log("success");
    }
    public IEnumerator transition()
    {
        pauseSim();
        isTransition = true;
        transitionObject.GetComponent<Animator>().SetBool("transition", true);
        yield return new WaitForSeconds(1f);
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
        }

        yield return new WaitForSeconds(1f);
        transitionObject.GetComponent<Animator>().SetBool("transition", false);
    }
    public void SideMenu()
    {
        if (mainDropDown.value == 0)
        {
            mainContent[0].SetActive(true);
            mainContent[1].SetActive(false);
            mainContent[2].SetActive(false);
            mainContent[3].SetActive(false);
            mainContent[4].SetActive(false);
        }
        if (mainDropDown.value == 1)
        {
            mainContent[0].SetActive(false);
            mainContent[1].SetActive(true);
            mainContent[2].SetActive(false);
            mainContent[3].SetActive(false);
            mainContent[4].SetActive(false);
        }
        if (mainDropDown.value == 2)
        {
            mainContent[0].SetActive(false);
            mainContent[1].SetActive(false);
            mainContent[2].SetActive(true);
            mainContent[3].SetActive(false);
            mainContent[4].SetActive(false);
        }
        if (mainDropDown.value == 3)
        {
            mainContent[0].SetActive(false);
            mainContent[1].SetActive(false);
            mainContent[2].SetActive(false);
            mainContent[3].SetActive(true);
            mainContent[4].SetActive(false);
        }
        if (mainDropDown.value == 4)
        {
            mainContent[0].SetActive(false);
            mainContent[1].SetActive(false);
            mainContent[2].SetActive(false);
            mainContent[3].SetActive(false);
            mainContent[4].SetActive(true);
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
        if (mainDropDown.value == 1)
        {
            for (int i = 0; i < Slots.Length + 1; i++)
            {
                //Debug.Log(EventSystem.current.currentSelectedGameObject.name);

                if (EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.name.Contains(i.ToString()))
                {
                    selected = i - 1;
                    SlotSprite.transform.position = Slots[i - 1].gameObject.transform.position;
                }
            }
        }
        if (mainDropDown.value == 2)
        {
            for (int i = 0; i < Slots2.Length + 1; i++)
            {
                //Debug.Log(EventSystem.current.currentSelectedGameObject.name);

                if (EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.name.Contains(i.ToString()))
                {
                    selected2 = i - 1;
                    SlotSprite.transform.position = Slots2[i - 1].gameObject.transform.position;
                }
            }
        }
    }
    // enumerables
}
