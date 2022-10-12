using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Creature : Entity
{

    public CreatureDetails stats;

    [SerializeField] private float year;

    [SerializeField] private Renderer[] colorManager;

    [HideInInspector] public CreatureEditor editor;
    [HideInInspector] public CreatureProceduralAnimation anim;


    public GameObject[] LegComponents, Torsos, Heads, Necks, Eyes, Legs, armatures;

    public TimeManager TimeManager;
    public GameObject torsoBody;

    private int torsonum;
    private float accuracy;
    private int refractoryTime = 500;

    public float mutationStrength, mutationChance;
    public LayerMask creatureLayerMask;

    public float maxSpeed;

    public float maxStorage, food, water, lookRad;

    public float digestionDuration = 100, t, refractoryPeriod;

    public bool foundMate, mated, readyToMate;
    private float tick;
    public float attack, def, maxDef;
    private state action;
    private Vector3 randomDir;
    //Objects
    public GameManager Manager;
    [SerializeField] private GameObject prefab, head;
    [SerializeField] private Transform target;
    public Transform parent1, parent2, mate;
    private float[] finalChromosome;

    public GameObject Offset;

    public List<Collider> coll, mateColl;
    private enum state
    {
        Food, //0
        Water, //1
        Reproduction, //2
        Wander, //3
        Run
    }

    private void Start()
    {
        stats.gender = gender(0, 2);
        food = 3;
        water = Mathf.Infinity; // Not Implemented
        action = state.Wander;
        maxStorage = stats.genes[6] * stats.genes[8];
        stats = GetComponent<CreatureDetails>();
        gameObject.tag = stats.genes[16] == 0 ? "prey" : "predator";
    }

    private void Update()
    {
        if (Time.time % accuracy == 0 || accuracy == 0)
        {
            lookRad = Mathf.Abs(Mathf.Sin((TimeManager.time * 24 / 7.64f) - (1.575f - (1.575f * stats.genes[7])))) * (stats.genes[4] / 2) + 2;
        }


        year -= Time.deltaTime;


        if (action == state.Food)
        {
            if (Time.time % accuracy == 0 || accuracy == 0)
            {
                if (target == null && FindClosestItem(target) != null)
                {
                    target = FindClosestItem(target);
                }
                else
                {
                    EatFood();
                }
            }
        }
        if (action == state.Water) { }
        if (action == state.Wander)
        {
            Wander();
        }
        if (action == state.Run)
        {
            Run();
        }
        if (action == state.Reproduction)
        {
            if (mated == false)
                FindMate();
            if (food < (-(stats.genes[9] * maxStorage) + maxStorage) / 3)
            {
                action = state.Food;
            }
        }
        if (action != state.Reproduction)
        {
            if (food >= -(stats.genes[9] * maxStorage) + maxStorage && age > 10 && mated == false)
            {
                action = state.Reproduction;
            }
        }

        if (mated == true)
        {
            mate = null;
            action = state.Food;
            refractoryPeriod += 1 * Time.timeScale;
            if (refractoryPeriod > refractoryTime)
            {
                foundMate = false;
                mated = false;
            }
        }
        if (year < 0)
        {
            age++;
            year = 1;
        }
        torsoBody.transform.localScale += new Vector3(0.003f * Time.deltaTime * Time.timeScale, 0.003f * Time.deltaTime * Time.timeScale, 0.003f * Time.deltaTime * Time.timeScale);
        torsoBody.transform.localScale = new Vector3(Mathf.Clamp(torsoBody.transform.localScale.x, 0.5f, stats.genes[6] + 0.25f), Mathf.Clamp(torsoBody.transform.localScale.y, 0.5f, stats.genes[6] + 0.25f), Mathf.Clamp(torsoBody.transform.localScale.z, 0.5f, stats.genes[6] + 0.25f));
        if (food <= 0 || transform.position.y < -10)
        {
            Die();
        }
    }


    public Transform FindClosestItem(Transform obj)
    {
        // Create sphere to collide using lookRadius
        List<Collider> hitColliders = Physics.OverlapSphere(transform.position, lookRad, 8).ToList();
        coll = hitColliders;
        if (stats.genes[16] == 1) hitColliders.RemoveAll(x => x.CompareTag("predator"));

        // set min distance and distanceCollider
        List<float> distColliders = new List<float>();
        float minDis = Mathf.Infinity;

        //find distance of each element in array
        for (int i = 0; i < hitColliders.Count; i++)
        {
            distColliders.Add(Vector3.Distance(transform.position, hitColliders[i].transform.position));
        }

        // find min distance of array
        if (distColliders.Count > 0)
        {
            distColliders.Sort();
            minDis = distColliders[0];
        }

        // iterate through new array to see if the min distance is equal to find the index, and since both hitcolliders and distance colliders index are equal, you can just replace.
        for (int i = 0; i < hitColliders.Count; i++)
        {
            if (minDis == distColliders[i])
            {
                obj = hitColliders[i].transform;
                return obj;
            }
        }
        return null;
    }

    public void EatFood()
    {
        if (target == null)
        {
            action = state.Wander;
        }
        else
        {
            if (transform.position.y > Manager.settings.waterHeight + 0.5f)
            {
                if (Vector3.Distance(transform.position, target.position) > 1.5)
                {
                    Vector3 toTarget = target.transform.position - transform.position;
                    transform.LookAt(target.transform.position);
                    Vector3 angle = transform.localEulerAngles;
                    transform.localEulerAngles = new Vector3(0, angle.y + 90, 0);
                    transform.Translate(toTarget * stats.genes[3] / 25f * Time.deltaTime, Space.World);
                    food -= stats.genes[0] / 70 * Time.deltaTime;
                    water -= stats.genes[0] / 100000 * Time.deltaTime;
                }
                else if (Vector3.Distance(transform.position, target.position) <= 1.5)
                {
                    if (food < stats.genes[8])
                    {
                        if (stats.genes[17] == 0)
                        {
                            food += 0.045f * Time.timeScale;
                            target.transform.localScale -= new Vector3(0.0035f * Time.timeScale, 0.0035f * Time.timeScale, 0.0035f * Time.timeScale);
                            if (t < 10)
                            {
                                target.GetComponent<PlantBehavior>().Fruit(gameObject, digestionDuration);
                                t++;
                            }
                        }
                        if (stats.genes[17] == 1)
                        {
                            Creature prey = target.GetComponent<Creature>();
                            prey.def -= attack * Time.timeScale;
                            target.localScale -= new Vector3(attack / prey.maxDef, attack / prey.maxDef, attack / prey.maxDef);
                            if (prey.def < 0)
                            {
                                Destroy(target.root.gameObject);
                                food += stats.genes[8];
                            }
                            food += stats.genes[8] / 10 * Time.deltaTime;
                            food -= stats.genes[0] / 60 * Time.deltaTime;
                            water -= stats.genes[0] / 70000 * Time.deltaTime;
                        }

                    }
                    else
                    {
                        action = state.Reproduction;
                    }
                }
            }
            else
            {
                //transform.Rotate(0, 80f / stats.genes[3] * Time.deltaTime, 0);
                target = null;
            }
        }
    }
    private void Wander()
    {
        if (readyToMate == false)
        {
            List<Collider> hitColliders = Physics.OverlapSphere(transform.position, lookRad, 8).ToList();
            if (hitColliders.Count > 0)
            {
                action = state.Food;
            }

        }
        food -= stats.genes[0] / 140 * Time.deltaTime;
        //water -= stats.genes[0] / 10000 * Time.deltaTime;
        tick -= Time.deltaTime;
        if (tick < 0)
        {
            if (transform.position.y > Manager.settings.waterHeight)
            {
                randomDir = new Vector3(0, Random.Range(-30, 30), 0) + transform.eulerAngles;
                transform.eulerAngles = new Vector3(0, Mathf.LerpAngle(transform.eulerAngles.y, randomDir.y, (stats.genes[3]) * Time.deltaTime), 0);
            }
            tick = 1;
        }
        if (transform.position.y <= Manager.settings.waterHeight + 0.5f)
        {
            transform.Rotate(0, 80f / stats.genes[3] * Time.deltaTime, 0);
        }

        //GetComponent<Creature>().enabled = false;
        transform.position += -transform.right * stats.genes[3] * Time.deltaTime / 5;
        //rb.AddRelativeForce(-stats.genes[3] * Time.deltaTime * 8000, 0, 0);
    }

    public void Run()
    {

        // Create sphere to collide using lookRadius
        List<Collider> predatorHitColliders = Physics.OverlapSphere(transform.position, lookRad, creatureLayerMask).ToList();
        predatorHitColliders.RemoveAll(x => x.CompareTag("prey"));
        if (predatorHitColliders.Count > 0)
        {
            food -= stats.genes[0] / 70 * Time.deltaTime;
            water -= stats.genes[0] / 10000 * Time.deltaTime;
            tick -= Time.deltaTime;
            if (tick < 0)
            {
                randomDir = -predatorHitColliders[0].transform.eulerAngles;
                tick = 1;
            }
            transform.eulerAngles = new Vector3(0, Mathf.LerpAngle(transform.eulerAngles.y, randomDir.y, (stats.genes[3]) * Time.deltaTime), 0);
            transform.position += -transform.right * stats.genes[3] * Time.deltaTime / 5;
        }
        else
        {
            action = state.Food;
        }


    }
    public void Reproduce()
    {
        if (stats.gender == 1)
        {
            for (int k = 0; k < stats.genes[5]; k++)
            {
                //food -= 1;
                finalChromosome = new float[stats.genes.Length];
                for (int i = 0; i < finalChromosome.Length; i++)
                {
                    if (i >= 3 && i < 10)
                    {
                        finalChromosome[i] = (stats.genes[i] + mate.GetComponent<Creature>().stats.genes[i]) / 2;
                        float rand = Random.Range(0, 1);
                        if (rand < mutationChance)
                        {
                            float strength = Random.Range(-mutationStrength * stats.genes[i], mutationStrength * stats.genes[i]);
                            finalChromosome[i] = finalChromosome[i] + strength;
                        }
                    }
                    else
                    {
                        int num = Random.Range(0, 1);

                        if (num == 0)
                        {
                            finalChromosome[i] = stats.genes[i];
                        }
                        if (num == 1)
                        {
                            finalChromosome[i] = mate.GetComponent<Creature>().stats.genes[i];
                        }
                    }
                }
            }
            Manager.CreateCreatures(torsoBody.transform.position, finalChromosome, true, transform.parent.name);
            food -= 2;
        }
        refractoryPeriod = 0;
        mated = true;
        readyToMate = false;
        action = state.Wander;
    }

    public void FindMate()
    {
        if (mated == false)
        {
            target = null;
            readyToMate = true;
            // Create sphere to collide using lookRadius
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, lookRad, creatureLayerMask);

            mateColl = hitColliders.ToList();
            //find distance of each element in array
            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (foundMate == false)
                {
                    Communicate(hitColliders[i].gameObject.GetComponent<Creature>());
                }
            }
            if (mate == null)
            {
                Wander();
            }

            if (foundMate == true)
            {
                if (mate != null)
                {

                    Vector3 toTarget = mate.transform.position - transform.position;
                    transform.LookAt(mate.transform.position);
                    Vector3 angle = transform.localEulerAngles;
                    transform.localEulerAngles = new Vector3(0, angle.y + 90, 0);
                    //rb.AddRelativeForce(0, 0, stats.genes[3] * 200);
                    transform.Translate(toTarget * stats.genes[3] / 10 * Time.deltaTime, Space.World);
                    food -= stats.genes[0] / 70 * Time.deltaTime;
                    water -= stats.genes[0] / 70000 * Time.deltaTime;
                    if (Vector3.Distance(transform.position, mate.position) < 1)
                    {
                        Reproduce();
                    }
                }
                else
                {
                    mate = null;
                    action = state.Food;
                    action = state.Wander;
                }
            }

        }
        if (mated == true)
        {
            action = state.Food;
        }
    }
    public void Communicate(Creature receiver)
    {
        if (receiver.readyToMate == true && receiver.GetComponent<CreatureDetails>().gender != stats.gender && receiver.mated == false && mated == false && receiver.tag == gameObject.tag)
        {
            mate = receiver.transform;
            receiver.mate = transform;
            foundMate = true;
            receiver.foundMate = true;
        }
    }
    public void Die()
    {
        Destroy(transform.parent.gameObject);
    }
    public void SetDetails()
    {
        // clamp 
        stats.genes[5] = Mathf.Clamp(stats.genes[5], 1f, 10f);
        stats.genes[6] = Mathf.Clamp(stats.genes[6], 0.5f, 1.25f);
        stats.genes[7] = Mathf.Clamp(stats.genes[7], -1f, 1f);
        stats.genes[9] = Mathf.Clamp(stats.genes[9], 0f, 1f);
        stats.genes[4] = Mathf.Clamp(stats.genes[4], 2, 26);
        {
            /*colorManager = GameObject.FindObjectsOfType<Renderer>();
            GameObject[] eyes = new GameObject[colorManager.Length];
            for (int i = 0; i < colorManager.Length; i++)
            {
                if (colorManager[i].transform.IsChildOf(transform) && !colorManager[i].gameObject.name.Contains("eye"))
                {
                    //colorManager[i].material.SetColor("_Color", new Vector4(stats.genes[7], stats.genes[7], stats.genes[7], 1));
                }
                if (colorManager[i].gameObject.name.Contains("eye") && colorManager[i].transform.IsChildOf(transform))
                {
                    eyes[i] = colorManager[i].gameObject;
                    colorManager[i].material.SetColor("_Color", new Vector4(0, Mathf.Abs(stats.genes[7]), 0, 1));
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
            eyes = gameObjectList5.ToArray();*/

            stats.genes[14] = 0;
            for (int i = 0; i < Eyes.Length; i++)
            {
                //Debug.Log(stats.genes[4]);
                //Debug.Log(((i + 1) * 4) - 2);
                //Debug.Log(1 + ((i + 1) * 4));
                if (stats.genes[4] >= ((i + 1) * 4) - 2)
                {
                    Debug.Log("Added Eyes");
                    stats.genes[14]++;
                }
                else
                {
                    Eyes[i].SetActive(false);
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
            //Debug.Log(Heads[(int)stats.genes[10] - 1].transform.parent.localPosition);
            Heads[(int)stats.genes[10] - 1].transform.parent.localPosition = editor.HeadPos[(int)stats.genes[11] - 1];
            //Necks[0] = Necks[(int)stats.genes[11]];
            for (int i = 0; i < editor.TorsoWeights.Length; i++)
            {
                if (stats.genes[8] <= editor.maxStorage[i] && stats.genes[8] >= editor.minStorage[i])
                {
                    Torsos[0].transform.localPosition = editor.TorsoPos[i];
                    Torsos[0].transform.localScale = editor.TorsoScale[i];
                    torsonum = i;
                }
            }
            transform.localScale = new Vector3((stats.genes[6]) / 20, (stats.genes[6]) / 20, (stats.genes[6]) / 20);

        }

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
        for (int i = 0; i < 6; i++)
        {

            if (stats.genes[3] > editor.minSpeed[Mathf.RoundToInt(stats.genes[12]) - 1] * (i + 1) / stats.genes[1])
            {
                //Legs[i].SetActive(true);
                //adding legs
                stats.genes[13]++;
            }
            else
            {
                //Legs[i].SetActive(false);
            }
            stats.genes[13] = Mathf.Clamp(stats.genes[13], 2, 6);
        }

        //weight 
        stats.genes[1] = (editor.LegWeights[Mathf.RoundToInt(stats.genes[12]) - 1] * (stats.genes[13] - 1)
            + editor.HeadWeights[Mathf.RoundToInt(stats.genes[10]) - 1]
            + editor.EyeWeights[Mathf.RoundToInt(stats.genes[14]) - 1]
            + editor.TorsoWeights[torsonum]
            + editor.NeckWeights[Mathf.RoundToInt(stats.genes[11]) - 1])
            * stats.genes[6]
            + stats.genes[8];

        // set speed value
        stats.genes[3] = Mathf.Clamp(stats.genes[3], editor.maxSpeed[Mathf.RoundToInt(stats.genes[12]) - 1] / stats.genes[1], editor.maxSpeed[Mathf.RoundToInt(stats.genes[12]) - 1] * (stats.genes[13] - 1) / stats.genes[1]);

        //clamp speed values.
        stats.genes[3] = Mathf.Clamp(stats.genes[3], 0, 25);

        if (stats.genes[13] == 0)
        {
            //Destroy(gameObject);
        }

        def = stats.genes[1] * stats.genes[6];
        maxDef = stats.genes[1] * stats.genes[6];
        attack = stats.genes[3] * (2 - stats.genes[6]);

        transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);
        transform.localScale = new Vector3(stats.genes[6] / 20, stats.genes[6] / 20, stats.genes[6] / 20);
    }

}
