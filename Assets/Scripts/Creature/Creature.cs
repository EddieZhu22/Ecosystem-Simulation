using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
public class Creature : Entity
{

    public CreatureGenes genes;
    [SerializeField] private Renderer[] colorManager;

    [HideInInspector] public CreatureEditor editor;
    [HideInInspector] public CreatureProceduralAnimation anim;


    public GameObject[] LegComponents, Torsos, Heads, Necks, Eyes, Legs, armatures;

    public TimeManager TimeManager;
    public GameObject torsoBody;
    private float accuracy;
    private int refractoryTime = 500;

    public float mutationStrength, mutationChance;
    public LayerMask creatureLayerMask;

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
    public Transform mate;
    private Dictionary<string, object> finalChromosome;
    [HideInInspector] public List<object> chromosome;
    public List<Collider> coll, mateColl;
    private enum state
    {
        Food, //0
        Water, //1
        Reproduction, //2
        Wander, //3
        Run
    }

    private void Start() => SetDetails();

    private void Update()
    {
        if (Time.time % accuracy == 0 || accuracy == 0)
        {
            lookRad = Mathf.Abs(Mathf.Sin((TimeManager.time * 24 / 7.64f) - (1.575f - (1.575f * (float)genes.Genes["eye color"])))) * ((float)genes.Genes["look radius"] / 2) + 2;
        }


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
            if (food < (((-(float)genes.Genes["size"]) * maxStorage) + maxStorage) / 3)
            {
                action = state.Food;
            }
        }
        if (action != state.Reproduction)
        {
            if (food >= -((float)genes.Genes["reproductive urge"] * maxStorage) + maxStorage && age > 10 && mated == false)
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
        if (Manager.tManager.day % 365 == 0)
        {
            age++;
        }
        //torsoBody.transform.localScale += new Vector3(0.003f * Time.deltaTime * Time.timeScale, 0.003f * Time.deltaTime * Time.timeScale, 0.003f * Time.deltaTime * Time.timeScale);
        //torsoBody.transform.localScale = new Vector3(Mathf.Clamp(torsoBody.transform.localScale.x, 0.5f, (float)genes.Genes["size"] + 0.25f), Mathf.Clamp(torsoBody.transform.localScale.y, 0.5f, (float)genes.Genes["size"] + 0.25f), Mathf.Clamp(torsoBody.transform.localScale.z, 0.5f, (float)genes.Genes["size"] + 0.25f));
        if (food <= 0 || transform.position.y < -10)
        {
            Die(transform.parent.gameObject);
        }
    }


    private Transform FindClosestItem(Transform obj)
    {
        // Create sphere to collide using lookRadius
        List<Collider> hitColliders = Physics.OverlapSphere(transform.position, lookRad, 8).ToList();
        coll = hitColliders;
        if ((int)genes.Genes["is predator"] == 1) hitColliders.RemoveAll(x => x.CompareTag("predator"));

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

    private void EatFood()
    {
        if (target == null) action = state.Wander;

        else
        {
            if (transform.position.y > Manager.settings.waterHeight + 0.5f)
            {
                if (Vector3.Distance(transform.position, target.position) > 1.5)
                {
                    Vector3 toTarget = target.transform.position - transform.position;

                    // Look at target
                    transform.LookAt(target.transform.position);

                    Vector3 angle = transform.localEulerAngles;

                    transform.localEulerAngles = new Vector3(0, angle.y + 90, 0);

                    //move towards target
                    transform.Translate(toTarget * (float)genes.Genes["speed"] / 25f * Time.deltaTime, Space.World);

                    //subtract food
                    food -= (float)genes.Genes["energy"] / 70 * Time.deltaTime;
                }
                else if (Vector3.Distance(transform.position, target.position) <= 1.5)
                {
                    if (food < (float)genes.Genes["storage"])
                    {
                        if ((int)genes.Genes["diet"] == 0)
                        {
                            //food gain
                            food += 0.045f * Time.timeScale;
                            target.transform.localScale -= new Vector3(0.0035f * Time.timeScale, 0.0035f * Time.timeScale, 0.0035f * Time.timeScale);
                            if (t < 10)
                            {
                                target.GetComponent<Plant>().Fruit(gameObject, digestionDuration);
                                t++;
                            }
                        }
                        if ((int)genes.Genes["diet"] == 1)
                        {
                            Creature prey = target.GetComponent<Creature>();
                            prey.def -= attack * Time.timeScale;
                            target.localScale -= new Vector3(attack / prey.maxDef, attack / prey.maxDef, attack / prey.maxDef);
                            if (prey.def < 0)
                            {
                                Destroy(target.root.gameObject);
                                food += (float)genes.Genes["storage"];
                            }
                            food += (float)genes.Genes["storage"] / 10 * Time.deltaTime;
                            food -= (float)genes.Genes["energy"] / 60 * Time.deltaTime;
                            //water -= (float) genes.Genes["energy"] / 70000 * Time.deltaTime;
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
        food -= (float)genes.Genes["energy"] / 140 * Time.deltaTime;
        //water -= genes.Genes["energy"] / 10000 * Time.deltaTime;
        tick -= Time.deltaTime;
        if (tick < 0)
        {
            if (transform.position.y > Manager.settings.waterHeight)
            {
                randomDir = new Vector3(0, UnityEngine.Random.Range(-30, 30), 0) + transform.eulerAngles;
                transform.eulerAngles = new Vector3(0, Mathf.LerpAngle(transform.eulerAngles.y, randomDir.y, (float)(genes.Genes["speed"]) * Time.deltaTime), 0);
            }
            tick = 1;
        }
        if (transform.position.y <= Manager.settings.waterHeight + 0.5f)
        {
            transform.Rotate(0, 80f / (float)genes.Genes["speed"] * Time.deltaTime, 0);
        }

        transform.position += -transform.right * (float)genes.Genes["speed"] * Time.deltaTime / 5;
    }

    private void Run()
    {

        // Create sphere to collide using lookRadius
        List<Collider> predatorHitColliders = Physics.OverlapSphere(transform.position, lookRad, creatureLayerMask).ToList();
        predatorHitColliders.RemoveAll(x => x.CompareTag("prey"));
        if (predatorHitColliders.Count > 0)
        {
            food -= (float)genes.Genes["energy"] / 70 * Time.deltaTime;
            //water -= (float) genes.Genes["energy"] / 10000 * Time.deltaTime;
            tick -= Time.deltaTime;
            if (tick < 0)
            {
                randomDir = -predatorHitColliders[0].transform.eulerAngles;
                tick = 1;
            }
            transform.eulerAngles = new Vector3(0, Mathf.LerpAngle(transform.eulerAngles.y, randomDir.y, ((float)genes.Genes["speed"]) * Time.deltaTime), 0);
            transform.position += -transform.right * (float)genes.Genes["speed"] * Time.deltaTime / 5;
        }
        else
        {
            action = state.Food;
        }


    }
    public void Reproduce()
    {
        List<object> chromosome = genes.Genes.Values.ToList();
        if (genes.gender == 1)
        {
            for (int k = 0; k < (float)genes.Genes["max offspring"]; k++)
            {
                //food -= 1;
                for (int i = 0; i < chromosome.Count; i++)
                {
                    if (i >= 3 && i < 10)
                    {
                        float rand = UnityEngine.Random.Range(0, 1);
                        if (rand < mutationChance)
                        {
                            float strength = UnityEngine.Random.Range(-mutationStrength * Convert.ToSingle(chromosome[i]), mutationStrength * Convert.ToSingle(chromosome[i]));
                            chromosome[i] = Convert.ToSingle(chromosome[i]) + strength;
                        }
                    }
                    else
                    {
                        int num = UnityEngine.Random.Range(0, 1);

                        if (num == 0)
                        {
                            chromosome[i] = Convert.ToSingle(chromosome[i]);
                        }
                        if (num == 1)
                        {
                            chromosome[i] = mate.GetComponent<Creature>().chromosome[i];
                        }
                    }
                }
            }
            finalChromosome = chromosome.ToDictionary(x => genes.Genes.Keys.ToString(), x => x);
            Manager.CreateCreatures(torsoBody.transform.position, finalChromosome, true, transform.parent.name);
            food -= 2;
        }
        //refractoryPeriod = 0;
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
                    //rb.AddRelativeForce(0, 0, genes.Genes["speed"] * 200);
                    transform.Translate(toTarget * (float)genes.Genes["speed"] / 10 * Time.deltaTime, Space.World);
                    food -= (float)genes.Genes["energy"] / 70 * Time.deltaTime;
                    //water -= genes.Genes["energy"] / 70000 * Time.deltaTime;
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
        if (receiver.readyToMate == true && receiver.GetComponent<CreatureGenes>().gender != genes.gender && receiver.mated == false && mated == false && receiver.tag == gameObject.tag)
        {
            mate = receiver.transform;
            receiver.mate = transform;
            foundMate = true;
            receiver.foundMate = true;
        }
    }
    public void SetDetails()
    {
        genes.gender = gender(0, 2);
        food = 3;
        water = Mathf.Infinity; // Not Implemented
        action = state.Wander;
        maxStorage = (float)genes.Genes["size"] * (float)genes.Genes["storage"];
        genes = GetComponent<CreatureGenes>();
        gameObject.tag = Convert.ToInt32(genes.Genes["is predator"]) == 0 ? "prey" : "predator";

        // clamp 
        genes.Genes["max offspring"] = Mathf.Clamp((float)genes.Genes["max offspring"], 1f, 10f);
        //genes.Genes["size"] = Mathf.Clamp((float)genes.Genes["size"], 0.5f, 1.25f);
        genes.Genes["eye color"] = Mathf.Clamp((float)genes.Genes["eye color"], -1f, 1f);
        genes.Genes["reproductive urge"] = Mathf.Clamp((float)genes.Genes["reproductive urge"], 0f, 1f);
        genes.Genes["look radius"] = Mathf.Clamp((float)genes.Genes["look radius"], 2, 26);

        int eyes = 0;
        for (int i = 0; i < Eyes.Length; i++)
        {
            //Debug.Log(genes.lookRadius);
            //Debug.Log(((i + 1) * 4) - 2);
            //Debug.Log(1 + ((i + 1) * 4));
            if ((float)genes.Genes["look radius"] >= ((i + 1) * 4) - 2)
            {
                Debug.Log("Added Eyes");
                eyes++;
            }
            else
            {
                Eyes[i].SetActive(false);
            }
        }
        genes.Genes["eyes"] = eyes;
        for (int i = 0; i < editor.HeadWeights.Length; i++)
        {
            Heads[i].SetActive(false);
        }
        Heads[(int)genes.Genes["head"] - 1].SetActive(true);
        Heads[(int)genes.Genes["head"] - 1].transform.parent.localPosition = (Vector3) genes.Genes["head position"];
        

        //Debug.Log(Heads[(int)genes.head - 1].transform.parent.localPosition);
        //Genes["neck"]s.Genes["energy"] = Necks[(int)genes.neck];

        transform.localScale = new Vector3(((float)genes.Genes["size"]), ((float)genes.Genes["size"]), ((float)genes.Genes["size"]));
        anim.torsoMesh.transform.localScale = (Vector3) genes.Genes["torso dimensions"];
        //calculate total Genes["energy"] expense
        genes.Genes["energy"] = (float)genes.Genes["size"] * ((float)genes.Genes["speed"] + (float)genes.Genes["look radius"]);
        //calculate total Genes["weight"]
        //cakculate max gestation
        if ((float)genes.Genes["max offspring"] > Mathf.RoundToInt((float)genes.Genes["weight"] / 30))
        {
            genes.Genes["max offspring"] = Mathf.RoundToInt((float)genes.Genes["weight"] / 30);
        }
        //setting legs to zero
        genes.Genes["speed"] = Mathf.Clamp(Convert.ToSingle(genes.Genes["speed"]), editor.maxSpeed[Mathf.RoundToInt(Convert.ToSingle(genes.Genes["leg"])) - 1] / Convert.ToSingle(genes.Genes["weight"]), editor.maxSpeed[Mathf.RoundToInt(Convert.ToSingle(genes.Genes["leg"])) - 1] * Convert.ToSingle(genes.Genes["legs"]) / Convert.ToSingle(genes.Genes["weight"]));
        int legs = 0;
        for (int i = 0; i < 6; i++)
        {
            print(Mathf.RoundToInt(Convert.ToSingle(genes.Genes["legs"])) - 1);
            if (Convert.ToSingle(genes.Genes["speed"]) > editor.minSpeed[Mathf.RoundToInt(Convert.ToSingle(genes.Genes["leg"])) - 1] * (i + 1) / Convert.ToSingle(genes.Genes["weight"]))
            {
                //Legs[i].SetActive(true);
                //adding legs
                legs++;
            }
            else
            {
                //Legs[i].SetActive(false);
            }
            legs = Mathf.Clamp(legs, 2, 6);
            genes.Genes["legs"] = legs;
        }

        //Genes["weight"] 
        genes.Genes["weight"] = (editor.LegWeights[Mathf.RoundToInt(Convert.ToSingle(genes.Genes["leg"])) - 1] * (Convert.ToSingle(genes.Genes["legs"]) - 1)
            + editor.HeadWeights[Mathf.RoundToInt(Convert.ToSingle(genes.Genes["head"])) - 1]
            + editor.EyeWeights[Mathf.RoundToInt(Convert.ToSingle(genes.Genes["eyes"])) - 1])
            * Convert.ToSingle(genes.Genes["size"])
            + Convert.ToSingle(genes.Genes["storage"]);

        // set Genes["speed"] value
        genes.Genes["speed"] = Mathf.Clamp(Convert.ToSingle(genes.Genes["speed"]), editor.maxSpeed[Mathf.RoundToInt(Convert.ToSingle(genes.Genes["leg"])) - 1] / Convert.ToSingle(genes.Genes["weight"]), editor.maxSpeed[Mathf.RoundToInt(Convert.ToSingle(genes.Genes["leg"])) - 1] * Convert.ToSingle(genes.Genes["legs"]) / Convert.ToSingle(genes.Genes["weight"]));
        //clamp Genes["speed"] values.
        genes.Genes["speed"] = Mathf.Clamp(Convert.ToSingle(genes.Genes["speed"]), 0, 25);

        if ((int)genes.Genes["legs"] == 0)
        {
            //Destroy(gameObject);
        }

        def = Convert.ToSingle(genes.Genes["weight"]) * Convert.ToSingle(genes.Genes["size"]);
        maxDef = Convert.ToSingle(genes.Genes["weight"]) * Convert.ToSingle(genes.Genes["size"]);
        attack = Convert.ToSingle(genes.Genes["speed"]) * (2 - Convert.ToSingle(genes.Genes["size"]));

        float startRot = UnityEngine.Random.Range(0, 360);
        anim.SetPosition(anim.Neck.transform,anim.Head.transform.position,anim.NeckPosition.position);
        transform.eulerAngles = new Vector3(0, startRot, 0);
    }

}
