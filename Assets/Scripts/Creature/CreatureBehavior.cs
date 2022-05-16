using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureBehavior : MonoBehaviour
{
    [SerializeField] private Brain brain;
    public CreatureDetails stats;

    [SerializeField] private Transform foodPos;

    [SerializeField] private float year, multiplier;

    private Renderer[] colorManager;

    public CreatureEditor editor;

    public Vector3[] HeadPos, TorsoScale, TorsoPos;

    public GameObject torsoBody;
    private GameManager manager;
    public GameObject[] LegComponents, Torsos, Heads, Necks, Eyes, Legs, armatures;
    public DayNightCycle time;
    private int torsonum;
    private float tick;

    bool toggle;
    private void Start()
    {
        if (time == null)
            time = GameObject.Find("DayNightAndLightController").GetComponent<DayNightCycle>();
        stats.gender = (int)Random.Range(0, 2);
        if (editor == null)
        editor = GameObject.Find("Editor").GetComponent<CreatureEditor>();
        //manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        brain.food = 10;
        brain.water = 1000000;
        brain.action = 0;
        SetDetails();
        transform.eulerAngles = new Vector3(0, Random.Range(-90, 90), 0);
        transform.localScale = new Vector3(stats.genes[6] / 20, stats.genes[6] / 20, stats.genes[6] / 20);
    }

    private void Update()
    {
            brain.lookRad = Mathf.Abs(Mathf.Sin((time.time / 7.64f) - (1.575f - (1.575f * stats.genes[7])))) * (stats.genes[4]/2) + 2;
            //action 0 = Food
            // action 1 = water
            // action 2 = Reproduction
            // action 3 = Idle

            year -= Time.deltaTime;
            brain.Movement();
            if (brain.food >= (1 / (stats.genes[9])) * (brain.maxStorage / 2) && brain.age > 10 && brain.done2 == false)
            {
                toggle = true;
            }
            if (toggle == false)
            {
                brain.FindFood();
                brain.EatFood();
                brain.Run();
            }
            if (brain.food < (1 / (stats.genes[9])) * (brain.maxStorage / 10) && toggle == true)
            {
                toggle = false;
            }
            if (toggle == true)
            {
                if (brain.done2 == false)
                    brain.FindMate();
            }
            if (brain.done2 == true)
            {
                brain.mate = null;
                toggle = false;
                brain.refractoryPeriod += 1 * Time.timeScale;
                if (brain.refractoryPeriod > 100)
                {
                    brain.done = false;
                    brain.done2 = false;
                }
            }
            if (year < 0)
            {
                brain.age++;
                year = 1;
            }
            if (brain.age > 100000)
            {
                //if (manager.NN)
                //    brain.NNReproduce();
                //else
                if (brain.food > (1 / stats.genes[9] * 5))
                {
                    if (brain.done2 == false)
                        brain.FindMate();
                }
                else
                {
                    brain.action = 0;
                    brain.mate = null;
                }
            }
            else
            {
                torsoBody.transform.localScale += new Vector3(0.003f * Time.deltaTime * Time.timeScale, 0.003f * Time.deltaTime * Time.timeScale, 0.003f * Time.deltaTime * Time.timeScale);
                torsoBody.transform.localScale = new Vector3(Mathf.Clamp(torsoBody.transform.localScale.x, 0.5f, stats.genes[6] + 0.25f), Mathf.Clamp(torsoBody.transform.localScale.y, 0.5f, stats.genes[6] + 0.25f), Mathf.Clamp(torsoBody.transform.localScale.z, 0.5f, stats.genes[6] + 0.25f));
            }
            if (brain.food <= 0 || brain.age > 100000)
            {
                brain.Die();
            }
    }

    private void SetDetails()
    {
        stats.genes[6] = Mathf.Clamp(stats.genes[6], 0.5f, 1.25f);
        stats.genes[7] = Mathf.Clamp(stats.genes[4],-1f,1f);
        colorManager = GameObject.FindObjectsOfType<Renderer>();
        GameObject[] eyes = new GameObject[colorManager.Length];
        for (int i = 0; i < colorManager.Length; i++)
        {
            if (colorManager[i].transform.IsChildOf(transform) && !colorManager[i].gameObject.name.Contains("eye"))
            {
                //colorManager[i].material.SetColor("_Color", new Vector4(brain.stats.genes[7], brain.stats.genes[7], brain.stats.genes[7], 1));
            }
            if (colorManager[i].gameObject.name.Contains("eye") && colorManager[i].transform.IsChildOf(transform))
            {
                eyes[i] = colorManager[i].gameObject;
                colorManager[i].material.SetColor("_Color", new Vector4(0, Mathf.Abs(brain.stats.genes[7]), 0, 1));
            }
        }
        List<GameObject> gameObjectList6 = new List<GameObject>(eyes);
        gameObjectList6.RemoveAll(x => x == null);
        eyes = gameObjectList6.ToArray();
        List<GameObject> gameObjectList5 = new List<GameObject>(eyes);
        for (int i = 0; i < eyes.Length; i++)
        {
            if (eyes[i].name.Contains(i.ToString()))
            {
                gameObjectList5[i] = eyes[i];
            }
        }
        eyes = gameObjectList5.ToArray();
        stats.genes[14] = 0;
        for (int i = 0; i < eyes.Length; i++)
        {
            //Debug.Log(stats.genes[4]);
            //Debug.Log(((i + 1) * 4) - 2);
            //Debug.Log(1 + ((i + 1) * 4));
            if (stats.genes[4] >= ((i+1) * 4)-2)
            {
                Debug.Log("Added Eyes");
                stats.genes[14]++;
            }
            else
            {
                eyes[i].SetActive(false);
            }
        }
        for (int i = 0; i < editor.HeadWeights.Length; i++)
        {
            Heads[i].SetActive(false);
        }
        Heads[(int)stats.genes[10] - 1].SetActive(true);
        for (int i = 0; i < editor.NeckWeights.Length; i++)
        {
            Necks[i].SetActive(false);
        }
        Necks[(int)stats.genes[11] - 1].SetActive(true);
        Heads[(int)stats.genes[10] - 1].transform.parent.localPosition = HeadPos[(int)stats.genes[11] - 1];
        //Necks[0] = Necks[(int)stats.genes[11]];
        for (int i = 0; i < editor.TorsoWeights.Length; i++)
        {
            if (stats.genes[8] <= editor.maxStorage[i] && stats.genes[8] >= editor.minStorage[i])
            {
                Torsos[0].transform.localPosition = TorsoPos[i];
                Torsos[0].transform.localScale = TorsoScale[i];
                torsonum = i;
            }
        }
        transform.localScale = new Vector3((stats.genes[6]) / 20, (stats.genes[6]) / 20, (stats.genes[6]) / 20);
        for (int i = 0; i < LegComponents.Length; i++)
        {
            if (LegComponents[i].tag == "Leg " + stats.genes[12].ToString())
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
        Legs = GameObject.FindGameObjectsWithTag("leg");
        armatures = GameObject.FindGameObjectsWithTag("ar");
        GameObject[] moveTargets = GameObject.FindGameObjectsWithTag("mt");
        for (int i = 0; i < Legs.Length; i++)
        {
            if (Legs[i].transform.root != transform)
            {
                Legs[i] = null;
            }
        }
        List<GameObject> gameObjectList = new List<GameObject>(Legs);
        gameObjectList.RemoveAll(x => x == null);
        Legs = gameObjectList.ToArray();

        GameObject[] gameObjectList3 = new GameObject[Legs.Length];
        for (int i = 0; i < Legs.Length; i++)
        {
            for (int k = 0; k < Legs.Length; k++)
            {
                if (Legs[i].name.Contains(k.ToString()))
                {
                    gameObjectList3[k] = Legs[i];
                }
            }
        }

        Legs = gameObjectList3;
        for (int i = 0; i < armatures.Length; i++)
        {
            if (armatures[i].transform.root != transform)
            {
                armatures[i] = null;
            }
        }
        List<GameObject> gameObjectList2 = new List<GameObject>(armatures);
        gameObjectList2.RemoveAll(x => x == null);
        armatures = gameObjectList2.ToArray();
        GameObject[] gameObjectList4 = new GameObject[armatures.Length];
        for (int i = 0; i < armatures.Length; i++)
        {
            for (int k = 0; k < armatures.Length; k++)
            {
                if (armatures[i].transform.parent.gameObject.name.Contains(k.ToString()))
                {
                    gameObjectList4[k] = armatures[i];
                }
            }
        }

        armatures = gameObjectList4;
        //calculate total energy expense
        stats.genes[0] = stats.genes[6] * (stats.genes[3] + stats.genes[4]);
        //calculate total weight
        //cakculate max gestation
        if (stats.genes[5] > Mathf.RoundToInt(stats.genes[1] / 30))
        {
            stats.genes[5] = Mathf.RoundToInt(stats.genes[1] / 30);
        }
        //setting legs to zero
        stats.genes[3] = Mathf.Clamp(stats.genes[3], editor.maxSpeed[Mathf.RoundToInt(stats.genes[12]) - 1] / stats.genes[1], editor.maxSpeed[Mathf.RoundToInt(stats.genes[12]) - 1] * stats.genes[13] / stats.genes[1]);
        stats.genes[13] = 0;
        for (int i = 0; i < Legs.Length; i++)
        {
            stats.genes[1] = (editor.LegWeights[Mathf.RoundToInt(stats.genes[12]) - 1] * (i + 1) + editor.HeadWeights[Mathf.RoundToInt(stats.genes[10]) - 1] + editor.EyeWeights[Mathf.RoundToInt(stats.genes[14]) - 1] + editor.TorsoWeights[torsonum] + editor.NeckWeights[Mathf.RoundToInt(stats.genes[11]) - 1]) * stats.genes[6] + stats.genes[8];
            if (stats.genes[3] > editor.minSpeed[Mathf.RoundToInt(stats.genes[12]) - 1] * (i+1) / stats.genes[1])
            {
                Legs[i].SetActive(true);
                //adding legs
                stats.genes[13]++;
            }
            else
            {
                Legs[i].SetActive(false);
            }
        }
        GetComponent<CreatureProceduralAnimation>().Set();
        stats.genes[3] = Mathf.Clamp(stats.genes[3], editor.maxSpeed[Mathf.RoundToInt(stats.genes[12]) - 1] / stats.genes[1], editor.maxSpeed[Mathf.RoundToInt(stats.genes[12]) - 1] * stats.genes[13] / stats.genes[1]);

        stats.genes[1] = (editor.LegWeights[Mathf.RoundToInt(stats.genes[12]) - 1] * (stats.genes[13]) + editor.HeadWeights[Mathf.RoundToInt(stats.genes[10]) - 1] + editor.EyeWeights[Mathf.RoundToInt(stats.genes[14]) - 1] + editor.TorsoWeights[torsonum] + editor.NeckWeights[Mathf.RoundToInt(stats.genes[11]) - 1]) * stats.genes[6] + stats.genes[8];
        //clamp speed values.
        stats.genes[4] = Mathf.Clamp(stats.genes[4], 0, 25);
        if(stats.genes[13] == 0)
        {
            Destroy(gameObject);
        }
        brain.def = stats.genes[1] * stats.genes[6];
        brain.maxDef = stats.genes[1] * stats.genes[6];
        brain.attack = stats.genes[3] * (2 - stats.genes[6]);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(torsoBody.transform.position, brain.stats.genes[4]);
    }
}
