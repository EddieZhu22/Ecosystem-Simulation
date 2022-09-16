using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : Entity, IManager
{

    private CreatureDetails stats;

    [SerializeField] private float year;

    private Renderer[] colorManager;

    public CreatureEditor editor;

    public Vector3[] HeadPos, TorsoScale, TorsoPos;

    public GameObject torsoBody;
    public GameObject[] LegComponents, Torsos, Heads, Necks, Eyes, Legs, armatures;
    public TimeManager time;

    //private variables
    private int torsonum;
    private float accuracy;
    private bool toggle;
    private int refractoryTime = 500;

    //Overall
    public float mutationStrength, mutationChance;
    public LayerMask raycastMask, raycastMask2;//Mask for the sensors
    private float[] input = new float[5];//input to the neural network
    //By Creature

    public float maxSpeed;

    public float maxStorage, food, water, lookRad;

    public float digestionDuration = 1000, t, refractoryPeriod;

    public bool foundMate, mated, readyToMate;
    private float tick;
    public float action, attack, def, maxDef;
    private int foodCollected;
    private Rigidbody rb;
    private Vector3 randomDir;
    //Objects
    private GameObject Settings, foodObj;
    public GameObject prefab, head;
    public Transform target, parent1, parent2, mate;
    public NeuralNetwork network;
    public Collider[] colliders, colliders2, collider3;
    public float[] distanceColl, allele1, allele2, allele3, allele4, finalAllele, finalChromosome1, finalChromosome2, finalChromosome;

    public GameObject Offset;
    [HideInInspector] public float num;

    private Vector3 toTarget;
    private void Start()
    {
        if (time == null)
            time = GameObject.Find("GameManager").GetComponent<TimeManager>();
        accuracy = time.accuracy;
        stats.gender = gender(0,2);
        if (editor == null)
            editor = GameObject.Find("Creature Editor").GetComponent<CreatureEditor>();
        food = 10;
        water = Mathf.Infinity; // Not Implemented
        action = 0;
        SetDetails();
        maxStorage = stats.genes[6] * stats.genes[8];
        stats = GetComponent<CreatureDetails>();
        if (stats.genes[16] == 0)
        {
            gameObject.tag = "prey";
        }
        if (stats.genes[16] == 1)
        {
            gameObject.tag = "predator";
        }
        if (time == null)
            time = GameObject.Find("GameManager").GetComponent<TimeManager>();
        if (time.day == 0)
        {
            SetGenes();
        }
        Restart();
        if (parent1 != null)
        {
            finalAllele = new float[parent1.GetComponent<CreatureDetails>().genes.Length / 2];
        }
        digestionDuration = 100;
    }

    private void Update()
    {
        if (Time.time % accuracy == 0 || accuracy == 0)
        {
            lookRad = Mathf.Abs(Mathf.Sin((time.time * 24 / 7.64f) - (1.575f - (1.575f * stats.genes[7])))) * (stats.genes[4] / 2) + 2;
        }
        //action 0 = Food
        // action 1 = water
        // action 2 = Reproduction
        // action 3 = Idle

        year -= Time.deltaTime;
        
        Movement();
        if (food >= -(stats.genes[9]*maxStorage) + maxStorage && age > 10 && mated == false)
        {
            toggle = true;
        }
        if (toggle == false)
        {
            if(Time.time % accuracy == 0 || accuracy == 0)
            {
                FindFood();
            }
            EatFood();
            Run();
        }
        if (food < (-(stats.genes[9] * maxStorage) + maxStorage)/3 && toggle == true)
        {
            toggle = false;
        }
        if (toggle == true)
        {
            if (mated == false)
                FindMate();
        }
        if (mated == true)
        {
            mate = null;
            toggle = false;
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
        if (age > 100000)
        {
            //if (manager.NN)
            //    NNReproduce();
            //else
            if (food > (1 / stats.genes[9] * 5))
            {
                if (mated == false)
                    FindMate();
            }
            else
            {
                action = 0;
                mate = null;
            }
        }
        else
        {
            torsoBody.transform.localScale += new Vector3(0.003f * Time.deltaTime * Time.timeScale, 0.003f * Time.deltaTime * Time.timeScale, 0.003f * Time.deltaTime * Time.timeScale);
            torsoBody.transform.localScale = new Vector3(Mathf.Clamp(torsoBody.transform.localScale.x, 0.5f, stats.genes[6] + 0.25f), Mathf.Clamp(torsoBody.transform.localScale.y, 0.5f, stats.genes[6] + 0.25f), Mathf.Clamp(torsoBody.transform.localScale.z, 0.5f, stats.genes[6] + 0.25f));
        }
        if (food <= 0 || age > 100000)
        {
            Die();
        }
    }

    private void SetDetails()
    {
        // clamp 
        stats.genes[5] = Mathf.Clamp(stats.genes[5], 1f, 10f);
        stats.genes[6] = Mathf.Clamp(stats.genes[6], 0.5f, 1.25f);
        stats.genes[7] = Mathf.Clamp(stats.genes[7], -1f, 1f);
        stats.genes[9] = Mathf.Clamp(stats.genes[9], 0f, 1f);
        stats.genes[4] = Mathf.Clamp(stats.genes[4], 2, 26);
        {
            colorManager = GameObject.FindObjectsOfType<Renderer>();
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
            eyes = gameObjectList5.ToArray();
            stats.genes[14] = 0;
            for (int i = 0; i < eyes.Length; i++)
            {
                //Debug.Log(stats.genes[4]);
                //Debug.Log(((i + 1) * 4) - 2);
                //Debug.Log(1 + ((i + 1) * 4));
                if (stats.genes[4] >= ((i + 1) * 4) - 2)
                {
                    //Debug.Log("Added Eyes");
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
        for (int i = 0; i < Legs.Length; i++)
        {

            if (stats.genes[3] > editor.minSpeed[Mathf.RoundToInt(stats.genes[12]) - 1] * (i + 1) / stats.genes[1])
            {
                Legs[i].SetActive(true);
                //adding legs
                stats.genes[13]++;
            }
            else
            {
                Legs[i].SetActive(false);
            }
            stats.genes[13] = Mathf.Clamp(stats.genes[13], 2, 6);
        }
        // Set Animation
        GetComponent<CreatureProceduralAnimation>().Set();

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
            Destroy(gameObject);
        }

        def = stats.genes[1] * stats.genes[6];
        maxDef = stats.genes[1] * stats.genes[6];
        attack = stats.genes[3] * (2 - stats.genes[6]);

        transform.eulerAngles = new Vector3(0, Random.Range(-90, 90), 0);
        transform.localScale = new Vector3(stats.genes[6] / 20, stats.genes[6] / 20, stats.genes[6] / 20);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(torsoBody.transform.position, stats.genes[4]);
    }


    public void FindFood()
    {
        if (stats.genes[16] == 0)
        {
            if (action == 0 || action == 1)
            {
                // Create sphere to collide using lookRadius
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, lookRad, 8);

                //visual represetation/debugging
                //colliders = hitColliders;

                // set min distance and distanceCollider
                distanceColl = new float[hitColliders.Length];
                float minDis = Mathf.Infinity;

                //find distance of each element in array
                for (int i = 0; i < hitColliders.Length; i++)
                {
                    distanceColl[i] = Vector3.Distance(transform.position, hitColliders[i].transform.position);
                }

                // find min distance of array
                minDis = Mathf.Min(distanceColl);

                // iterate through new array to see if the min distance is equal to find the index, and since both hitcolliders and distance colliders index are equal, you can just replace.
                for (int i = 0; i < hitColliders.Length; i++)
                {
                    if (minDis == distanceColl[i])
                    {
                        target = hitColliders[i].transform;
                    }
                }
            }
        }
        if (stats.genes[16] == 1)
        {
            if (action == 0 || action == 1)
            {
                // Create sphere to collide using lookRadius
                Collider[] hitColliders2 = Physics.OverlapSphere(transform.position, lookRad, raycastMask2);
                List<Collider> collList = new List<Collider>(hitColliders2);
                collList.RemoveAll(x => x.CompareTag("predator"));
                hitColliders2 = collList.ToArray();
                //visual represetation/debugging
                collider3 = hitColliders2;

                // set min distance and distanceCollider
                distanceColl = new float[hitColliders2.Length];
                float minDis = Mathf.Infinity;

                //find distance of each element in array
                for (int i = 0; i < hitColliders2.Length; i++)
                {
                    distanceColl[i] = Vector3.Distance(transform.position, hitColliders2[i].transform.position);
                }

                // find min distance of array
                minDis = Mathf.Min(distanceColl);

                // iterate through new array to see if the min distance is equal to find the index, and since both hitcolliders and distance colliders index are equal, you can just replace.
                for (int i = 0; i < hitColliders2.Length; i++)
                {
                    if (minDis == distanceColl[i])
                    {
                        target = hitColliders2[i].transform;
                    }
                }
            }
        }
    }

    public void EatFood()
    {
        if (stats.genes[16] == 0)
        {
            if (target != null && action != 2)
            {
                if (Vector3.Distance(transform.position, target.position) > 1.5)
                {
                    toTarget = target.transform.position - transform.position;
                    transform.LookAt(target.transform.position);
                    Vector3 angle = transform.localEulerAngles;
                    transform.localEulerAngles = new Vector3(0, angle.y + 90, 0);
                    //rb.AddRelativeForce(0, 0, stats.genes[3] * 200);
                    transform.Translate(toTarget * stats.genes[3] / 25f * Time.deltaTime, Space.World);
                    food -= stats.genes[0] / 70 * Time.deltaTime;
                    water -= stats.genes[0] / 100000 * Time.deltaTime;
                }
                else if (Vector3.Distance(transform.position, target.position) <= 1.5)
                {
                    if (food < stats.genes[8])
                    {
                        food += 0.045f * Time.timeScale;
                        target.transform.localScale -= new Vector3(0.0035f * Time.timeScale, 0.0035f * Time.timeScale, 0.0035f * Time.timeScale);
                        if (t < 10)
                        {
                            target.GetComponent<PlantBehavior>().Fruit(gameObject, digestionDuration);
                            t++;
                        }
                    }

                }

            }
            else if (target == null && action != 2)
            {
                Wander();
            }
        }
        if (stats.genes[16] == 1)
        {
            if (target != null && action != 2)
            {
                if (Vector3.Distance(transform.position, target.position) > 0.25f)
                {
                    toTarget = target.transform.position - transform.position;
                    transform.LookAt(target.transform.position);
                    Vector3 angle = transform.localEulerAngles;
                    transform.localEulerAngles = new Vector3(0, angle.y + 90, 0);
                    transform.Translate(toTarget * stats.genes[3] / 10f * Time.deltaTime, Space.World);
                    food -= stats.genes[0] / 70 * Time.deltaTime;
                    water -= stats.genes[0] / 1000 * Time.deltaTime;
                }
                if (Vector3.Distance(transform.position, target.position) <= 1.5)
                {
                    if (food < stats.genes[8])
                    {
                        if (stats.genes[17] == 0)
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

                }

            }
            else if (target == null && action != 2)
            {
                Wander();
                //Log("Wander_Success");
            }
        }
    }
    private void Wander()
    {
        food -= stats.genes[0] / 140 * Time.deltaTime;
        water -= stats.genes[0] / 10000 * Time.deltaTime;
        tick -= Time.deltaTime;
        if (tick < 0)
        {
            randomDir = new Vector3(0, Random.Range(60, -60), 0) + transform.eulerAngles;
            tick = 1;
        }
        transform.eulerAngles = new Vector3(0, Mathf.LerpAngle(transform.eulerAngles.y, randomDir.y, (stats.genes[3]) * Time.deltaTime), 0);
        rb.AddRelativeForce(-stats.genes[3] * Time.timeScale * 80, 0, 0);
    }
    public void Movement()
    {
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
        }
        //rb.AddForce(0, -1000, 0);
    }
    public void Run()
    {
        if (stats.genes[16] == 0)
        {
            // Create sphere to collide using lookRadius
            Collider[] hitColliders2 = Physics.OverlapSphere(transform.position, lookRad, raycastMask2);
            List<Collider> collList = new List<Collider>(hitColliders2);
            collList.RemoveAll(x => x.CompareTag("prey"));
            hitColliders2 = collList.ToArray();
            if (hitColliders2.Length > 0)
            {
                action = 3;
                food -= stats.genes[0] / 70 * Time.deltaTime;
                water -= stats.genes[0] / 10000 * Time.deltaTime;
                tick -= Time.deltaTime;
                if (tick < 0)
                {
                    randomDir = -hitColliders2[0].transform.eulerAngles;
                    tick = 1;
                }
                transform.eulerAngles = new Vector3(0, Mathf.LerpAngle(transform.eulerAngles.y, randomDir.y, (stats.genes[3]) * Time.deltaTime), 0);
                rb.AddRelativeForce(-stats.genes[3] * Time.timeScale * 80, 0, 0);
            }
            else
            {
                action = 0;
            }

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
                        if ((int)Random.Range(0, 1) == 0)
                        {
                            finalChromosome[i] = stats.genes[i];
                        }
                        if ((int)Random.Range(0, 1) == 1)
                        {
                            finalChromosome[i] = mate.GetComponent<Creature>().stats.genes[i];
                        }
                    }
                }
            }
            GameObject Creature = Instantiate(prefab, transform.position, Quaternion.identity);
            stats.genes = finalChromosome;
        }
        refractoryPeriod = 0;
        mated = true;
        readyToMate = false;
        action = 0;
    }

    public void FindMate()
    {
        if (mated == false)
        {
            readyToMate = true;
            // Create sphere to collide using lookRadius
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, lookRad, raycastMask2);

            colliders2 = hitColliders;

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

            /*if (food < (1 / (stats.genes[9] * 2)) * (maxStorage / 2))
            {
                readyToMate = false;
                Wander();
            }*/
            if (foundMate == true)
            {
                if (mate != null)
                {
                    if (Vector3.Distance(transform.position, mate.position) <= lookRad)
                    {
                        toTarget = mate.transform.position - transform.position;
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
                    }
                }
                else
                {
                    mate = null;
                    action = 0;
                    Wander();
                }
            }

        }
        if (mated == true)
        {
            action = 0;
        }
    }
    public void Communicate(Creature receiver)
    {
        if (receiver.readyToMate == true && receiver.GetComponent<CreatureDetails>().gender != stats.gender && receiver.mated == false && mated == false && receiver.tag == gameObject.tag)
        {
            mate = receiver.transform;
            receiver.mate = transform;
            foundMate = true;
        }
    }
    private void Meiosis()
    {
        //set chromosomes equal to parents
        finalChromosome1 = parent1.gameObject.GetComponent<CreatureDetails>().genes;
        finalChromosome2 = parent2.gameObject.GetComponent<CreatureDetails>().genes;
        allele1 = new float[finalChromosome1.Length / 2];
        allele2 = new float[(finalChromosome1.Length) / 2];
        allele3 = new float[finalChromosome1.Length / 2];
        allele4 = new float[(finalChromosome1.Length) / 2];
        for (int i = 3; i < stats.genes.Length; i++)
        {
            //homologous chromosomes cross over
            float gene1 = parent1.gameObject.GetComponent<CreatureDetails>().genes[i];
            float gene2 = parent2.gameObject.GetComponent<CreatureDetails>().genes[i];
            float rand = Random.Range(0, 1);
            if (rand <= mutationChance)
            {
                finalChromosome1[i] = gene2;
                finalChromosome2[i] = gene1;
            }
        }
        //Meiosis II
        for (int i = 0; i < stats.genes.Length; i++)
        {
            if (i < stats.genes.Length / 2)
            {
                allele1[i] = finalChromosome1[i];
                allele3[i] = finalChromosome2[i];
            }
            if (i >= stats.genes.Length / 2)
            {
                allele2[i - stats.genes.Length / 2] = finalChromosome1[i];
                allele4[i - stats.genes.Length / 2] = finalChromosome2[i];
            }
        }
        float alleleTemp = (int)Random.Range(1, 5);
        if (alleleTemp == 1)
        {
            finalAllele = allele1;
            num = 1;
        }
        else if (alleleTemp == 2)
        {
            finalAllele = allele2;
            num = 2;
        }
        else if (alleleTemp == 3)
        {
            finalAllele = allele3;
            num = 3;
        }
        else if (alleleTemp == 4)
        {
            finalAllele = allele4;
            num = 4;
        }
    }
    public void Die()
    {
        Destroy(transform.parent.gameObject);
    }
    public void SetGenes()
    {
        stats.genes[5] = Random.Range(0, 5);
        stats.genes[4] = Random.Range(0, 25);
        stats.genes[3] = Random.Range(0, 200);
    }
    public void Restart()
    {
        //energy = 1;
        age = 0;
        rb = GetComponent<Rigidbody>();
        Settings = GameObject.Find("GameManager");
        mutationStrength = Settings.GetComponent<GameManager>().MutationStrength;
        mutationChance = Settings.GetComponent<GameManager>().MutationChance;
        //finalChromosome = stats.genes;
        /*for (int i = 0; i < finalChromosome.Length; i++)
        {
            if (i >= 3 && i < 10)
            {
                finalChromosome[i] = stats.genes[i];
                float rand = Random.Range(0, 1);
                if (rand < mutationChance)
                {
                    float strength = Random.Range(-mutationStrength * stats.genes[i], mutationStrength * stats.genes[i]);
                    finalChromosome[i] = finalChromosome[i] + strength;
                }
            }
        }
        stats.genes = finalChromosome;
        */
    }

}
