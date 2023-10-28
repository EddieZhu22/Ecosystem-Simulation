using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
public class Creature : Entity
{

    // **Creature Components and Managers**
    [Header("Components and Managers")]
    public CreatureGenes genes;
    [HideInInspector] public CreatureEditor editor;
    [HideInInspector] public CreatureProceduralAnimation anim;
    public GameManager Manager;
    public TimeManager TimeManager;

    // **Body Parts and Renderers**
    [Header("Body Parts and Renderers")]
    [SerializeField] private Renderer[] colorManager;
    public GameObject[] LegComponents, Torsos, Heads, Necks, Eyes, Legs, armatures;
    public GameObject torsoBody;
    [SerializeField] private GameObject prefab, head;
    private Renderer render;

    // **Physical and Biological Attributes**
    [Header("Attributes")]
    public float mutationStrength, mutationChance;
    public float maxStorage, food, water, lookRad;
    public float attack, def, maxDef;
    private float accuracy;
    public float timeAlive;
    public int generation;
    // **Reproduction**
    [Header("Reproduction")]
    public bool foundMate, mated, readyToMate;
    private int refractoryTime = 500;
    public float digestionDuration = 100, t, refractoryPeriod;
    [HideInInspector] public List<float> chromosome;
    private Dictionary<string, float> finalChromosome;
    public int children;
    public float geneticDist;

    // **Environmental and Interactions**
    [Header("Environment and Interaction")]
    public LayerMask creatureLayerMask;
    [SerializeField] private Transform target;
    public Transform mate;
    public List<Collider> coll, mateColl;
    public state action;
    private Vector3 randomDir;
    private float tick;
    private bool avoiding;
    // **Constants and Tags**
    [Header("Constants and Tags")]
    private const string WATER_TAG = "Water";  // Adjust if your water tag is different.
    public LayerMask rayLayer;

    public enum state
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

        if (Manager.settings.showCreatureDetails == true)
            render.material = Eyes[0].GetComponent<Renderer>().material;
        else
        {
            render.material = Heads[0].GetComponent<Renderer>().material;
        }
        HandleLookRadius();
        HandleStateActions();
        HandleMateRefractoryPeriod();
        HandleAging();
        HandleDeathConditions();

        Avoidance();  // From the avoidance system we discussed
    }

    private void HandleLookRadius()
    {
        if (Time.time % accuracy == 0 || accuracy == 0)
        {
            lookRad = Mathf.Abs(Mathf.Sin((TimeManager.time * 24 / 7.64f) - (1.575f - (1.575f * (float)genes.Genes["eye color"])))) * ((float)genes.Genes["look radius"] / 2) + 2;
        }
    }

    private void HandleStateActions()
    {
        switch (action)
        {
            case state.Food:
                HandleFoodState();
                break;
            case state.Water:
                // Currently empty. Implement when needed.
                break;
            case state.Wander:
                Wander();
                break;
            case state.Run:
                Run();
                break;
            case state.Reproduction:
                HandleReproductionState();
                break;
        }
    }

    private void HandleFoodState()
    {
        if (Time.time % accuracy == 0 || accuracy == 0)
        {
            if (target == null && FindClosestItem(target) != null)
            {
                Wander();
                target = FindClosestItem(target);
            }
            else
            {
                EatFood();
            }
        }
    }

    private void HandleReproductionState()
    {
        if (!mated)
            FindMate();

        if (food < (((-(float)genes.Genes["size"]) * maxStorage) + maxStorage) / 3)
        {
            action = state.Food;
        }
    }

    private void HandleMateRefractoryPeriod()
    {
        if (mated)
        {
            mate = null;
            action = state.Wander;
            refractoryPeriod += 1 * Time.deltaTime;
            if (refractoryPeriod > refractoryTime)
            {
                foundMate = false;
                mated = false;
            }
        }
    }


    private float lastKnownDay = -1; 

    private void HandleAging()
    {
        int currentDay = Mathf.FloorToInt(Manager.tManager.day);
        if (currentDay != lastKnownDay)
        {
            lastKnownDay = currentDay;

            age++;  
        }
        timeAlive += Time.deltaTime;
    }

    private void HandleDeathConditions()
    {
        if (food <= 0 || transform.position.y < -15)
        {
            if (Convert.ToSingle(genes.Genes["is predator"]) == 1)
                Manager.dManager.currentPredatorCount--;
            else
                Manager.dManager.currentHerbivoreCount--;
            Die(transform.parent.gameObject);
        }
    }


    private Transform FindClosestItem(Transform obj)
    {
        // Create sphere to collide using lookRadius
        List<Collider> hitColliders = new List<Collider>();
        if (tag == "prey") 
        { 
            hitColliders = Physics.OverlapSphere(transform.position, lookRad, 8).ToList(); 

        
        }
        if (tag == "predator") 
        { 
            hitColliders = Physics.OverlapSphere(transform.position, lookRad, creatureLayerMask).ToList();
            hitColliders.RemoveAll(x => x.CompareTag("predator"));

            foreach (var collider in hitColliders)
            {
                Debug.Log("After removal: " + collider.name);
            }
        }

        coll = hitColliders;

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
                Vector3 targetPosition = hitColliders[i].transform.position;

                // Check if the target is in water or on steep terrain
                if (Physics.Raycast(targetPosition + new Vector3(0, 10, 0), -Vector3.up, out RaycastHit hit, Mathf.Infinity, rayLayer))
                {
                    if (hit.point.y <= Manager.settings.waterHeight || IsSteepTerrain(hit.normal))
                    {
                        // Ignore this target and continue to the next one
                        continue;
                    }
                }

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
                    transform.Translate(toTarget * (float) genes.Genes["speed"] / 25f * Time.deltaTime, Space.World);

                    //subtract food
                    food -= (float) genes.Genes["energy"] / 30 * Time.deltaTime;
                }
                else if (Vector3.Distance(transform.position, target.position) <= 1.5)
                {
                    if (food < (float)genes.Genes["storage"])
                    {
                        if ((int)genes.Genes["diet"] == 0)
                        {
                            Plant plant = target.GetComponent<Plant>();
                            float size = target.GetComponent<BoxCollider>().size.x * target.GetComponent<BoxCollider>().size.y * target.GetComponent<BoxCollider>().size.z;
                            //food gain
                            food += 5/3f * Time.deltaTime;
                            target.transform.localScale -= new Vector3(20f / size * Time.deltaTime, 20f / size * Time.deltaTime, 20f / size * Time.deltaTime);
                            transform.localScale += new Vector3(0.01f,0.01f,0.01f);
                            transform.localScale = new Vector3(Mathf.Clamp(transform.localScale.x, 0, (float)genes.Genes["size"]), Mathf.Clamp(transform.localScale.x, 0, (float)genes.Genes["size"]), Mathf.Clamp(transform.localScale.x, 0, (float)genes.Genes["size"]));

                            if (gameObject != null)
                            {
                                plant.Fruit(gameObject, Manager.settings.creatureDigestionDuration, plant.generation);
                            }
                        }
                        if ((int) genes.Genes["diet"] == 1)
                        {
                            Creature prey = target.GetComponent<Creature>();
                            target.localScale -= new Vector3(attack / prey.maxDef, attack / prey.maxDef, attack / prey.maxDef);
                            if (target.localScale.x < 0)
                            {
                                Destroy(target.root.gameObject);
                                food += (float) prey.genes.Genes["storage"] * Manager.settings.PredatorFoodGainMultiplier; // gain all creatures storage

                            }
                            food += (float) prey.genes.Genes["storage"] / 10 * Time.deltaTime * Manager.settings.PredatorFoodGainMultiplier;
                            food -= (float)genes.Genes["energy"] / 30 * Time.deltaTime * Manager.settings.FoodLossMultiplier;
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
        checkPredators();
    }
    private void Wander()
    {
        // Existing logic for checking food presence
        if (readyToMate == false)
        {
            List<Collider> foodColliders = new List<Collider>();
            if (tag == "prey")
            {
                foodColliders = Physics.OverlapSphere(transform.position, lookRad, 8).ToList();
            }
            else if (tag == "predator")
            {
                foodColliders = Physics.OverlapSphere(transform.position, lookRad, creatureLayerMask).ToList();
            }

            if (foodColliders.Count > 0)
            {
                action = state.Food;
                return;  // Exit the method if food is found
            }
            List<Collider> preyColliders = Physics.OverlapSphere(transform.position, lookRad, 1 << 6).Where(c => c.tag == "prey").ToList();
            List<Collider> predatorColliders = Physics.OverlapSphere(transform.position, lookRad, 1 << 6).Where(c => c.tag == "predator").ToList();

            Vector3 wanderDirection = -transform.right;  // Original wander direction
            Vector3 avgDirection = Vector3.zero;  // Average direction towards similar creatures

            List<Collider> sameTypeColliders = (tag == "prey") ? preyColliders : predatorColliders;
            // New logic for moving towards similar creatures
            if (Manager.settings.segregation == true)
            {


                // Calculate the average direction towards similar creatures
                float totalWeight = 0;
                foreach (Collider creature in sameTypeColliders)
                {
                    float weight = 1 / (Mathf.Abs(creature.GetComponent<Creature>().geneticDist - this.geneticDist) + 1);
                    avgDirection += (creature.transform.position - transform.position).normalized * weight;
                    totalWeight += weight;
                }

                if (totalWeight > 0)
                {
                    avgDirection /= totalWeight;
                }

                // Blend the wander direction and the average direction
                Vector3 finalDirection = Vector3.Lerp(wanderDirection, avgDirection, 0.5f);  // 0.2f is the blending factor

                // Correct for the 90-degree rotation and move
                finalDirection = finalDirection.normalized;
                transform.position += finalDirection * (float)genes.Genes["speed"] * Time.deltaTime / 5 * Manager.settings.CreatureSpeedMultiplier;

            }
            else
            {
                transform.position += -transform.right * (float)genes.Genes["speed"] * Time.deltaTime / 5 * Manager.settings.CreatureSpeedMultiplier;
            }

            // Your existing logic for reducing food, ticking, etc.
            food -= (float)genes.Genes["energy"] / 50 * Time.deltaTime * Manager.settings.CreatureFoodDecrease;
            tick -= Time.deltaTime;

            if (sameTypeColliders.Count == 0 || Manager.settings.segregation == false)
            {
                if (tick < 0)
                {
                    if (transform.position.y > Manager.settings.waterHeight)
                    {
                        randomDir = new Vector3(0, UnityEngine.Random.Range(-30, 30), 0) + transform.eulerAngles;
                        transform.eulerAngles = new Vector3(0, Mathf.LerpAngle(transform.eulerAngles.y, randomDir.y, (float)(genes.Genes["speed"]) * Time.deltaTime), 0);
                    }
                    tick = 1;
                }
            }
        }
        else
        {
            food -= (float)genes.Genes["energy"] / 50 * Time.deltaTime * Manager.settings.CreatureFoodDecrease;
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
            transform.position += -transform.right * (float)genes.Genes["speed"] * Time.deltaTime / 5 * Manager.settings.CreatureSpeedMultiplier;
        }
    }




    private void checkPredators()
    {
        if (tag == "prey")
        {
            List<Collider> predatorHitColliders = Physics.OverlapSphere(transform.position, lookRad, creatureLayerMask).ToList();
            predatorHitColliders.RemoveAll(x => x.CompareTag("prey"));
            if (predatorHitColliders.Count > 0)
            {
                action = state.Run;
            }
        }
    }
    private void Run()
    {
        if(tag == "prey")
        {
            // Create sphere to collide using lookRadius
            List<Collider> predatorHitColliders = Physics.OverlapSphere(transform.position, lookRad, creatureLayerMask).ToList();
            predatorHitColliders.RemoveAll(x => x.CompareTag("prey"));
            if (predatorHitColliders.Count > 0)
            {
                food -= (float)genes.Genes["energy"] / 70 * Time.deltaTime * Manager.settings.CreatureFoodDecrease;
                //water -= (float) genes.Genes["energy"] / 10000 * Time.deltaTime;
                tick -= Time.deltaTime;
                if (tick < 0)
                {
                    randomDir = -predatorHitColliders[0].transform.eulerAngles;
                    tick = 1;
                }
                transform.eulerAngles = new Vector3(0, Mathf.LerpAngle(transform.eulerAngles.y, randomDir.y, ((float)genes.Genes["speed"]) * Time.deltaTime), 0);
                transform.position += -transform.right * (float)genes.Genes["speed"] * Time.deltaTime / 5 * Manager.settings.CreatureSpeedMultiplier;
            }

            else
            {
                action = state.Wander;
            }
        }



    }
    public void Reproduce()
    {
        List<float> chromosome = genes.Genes.Values.ToList();
        if (genes.gender == 1)
        {
            for (int k = 0; k < (float)genes.Genes["max offspring"]; k++)
            {
                //food -= 1;
                for (int i = 0; i < chromosome.Count; i++)
                {
                    int num = UnityEngine.Random.Range(0, 1);

                    if (i >= 3 && i < 15)
                    {
                        if (num == 1)
                        {
                            chromosome[i] = mate.GetComponent<Creature>().chromosome[i];
                        }

                        float rand = UnityEngine.Random.Range(0, 1);
                        if (rand < Manager.CreatureMutationChance)
                        {
                            float strength = UnityEngine.Random.Range(-Manager.CreatureMutationStrength * Convert.ToSingle(chromosome[i]), Manager.CreatureMutationStrength * Convert.ToSingle(chromosome[i]));
                            chromosome[i] = Convert.ToSingle(chromosome[i]) + strength;
                        }
                    }
                    else
                    {
                        if (num == 0)
                        {
                            chromosome[i] = Convert.ToSingle(chromosome[i]);
                        }
                        if (num == 1)
                        {
                            chromosome[i] = Convert.ToSingle(mate.GetComponent<Creature>().chromosome[i]);
                        }
                    }
                }
            }

            var finalChromosome = new Dictionary<string, float>();

            for (int i = 0; i < genes.Genes.Keys.Count; i++)
            {
                //print(chromosome[i]);
                //print(genes.Genes.Keys.ToString()[i]);
                string key = genes.Genes.Keys.ElementAt(i).ToString();
                float value = chromosome.ElementAt(i);
                finalChromosome.Add(key, value);
            }
            int gen = mate.GetComponent<Creature>().generation <= generation ? generation : mate.GetComponent<Creature>().generation;
            gen++;
            Manager.CreateCreatures(torsoBody.transform.position, finalChromosome, true, transform.parent.name, gen);
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
            action = state.Wander;
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
        water = Mathf.Infinity; // Not Implemented
        action = state.Wander;
        maxStorage = (float)genes.Genes["size"] * (float)genes.Genes["storage"];
        food = maxStorage/5;
        genes = GetComponent<CreatureGenes>();
        gameObject.tag = Convert.ToInt32(genes.Genes["is predator"]) == 0 ? "prey" : "predator";
        gameObject.layer = 6;
        // clamp 
        genes.Genes["max offspring"] = Mathf.Clamp((float)genes.Genes["max offspring"], 1f, 10f);
        genes.Genes["size"] = Mathf.Clamp((float)genes.Genes["size"], 0.75f, 3f);
        genes.Genes["torso dimensions x"] = Mathf.Clamp((float)genes.Genes["torso dimensions x"], 5f, 50f);
        genes.Genes["torso dimensions y"] = Mathf.Clamp((float)genes.Genes["torso dimensions y"], 5f, 25f);
        genes.Genes["torso dimensions z"] = Mathf.Clamp((float)genes.Genes["torso dimensions z"], 5f, 25f);
        genes.Genes["eye color"] = Mathf.Clamp((float)genes.Genes["eye color"], -1f, 1f);
        genes.Genes["reproductive urge"] = Mathf.Clamp((float)genes.Genes["reproductive urge"], 0f, 1f);
        genes.Genes["look radius"] = Mathf.Clamp((float)genes.Genes["look radius"], 2, 26);



        render = anim.torsoMesh.GetComponent<Renderer>();

        if (tag == "prey")
        {
            Manager.dManager.Prey.Add(GetComponent<Creature>());
        }
        if (tag == "predator")
        {
            Manager.dManager.Predator.Add(GetComponent<Creature>());
        }

        refractoryPeriod = Manager.settings.refractoryPeriod;


        int eyes = 0;
        for (int i = 0; i < Eyes.Length; i++)
        {
            //Debug.Log(genes.lookRadius);
            //Debug.Log(((i + 1) * 4) - 2);
            //Debug.Log(1 + ((i + 1) * 4));
            if ((float)genes.Genes["look radius"] >= ((i + 1) * 4) - 2)
            {
                //Debug.Log("Added Eyes");
                eyes++;
                if (gameObject.tag == "prey") { Eyes[i].GetComponent<Renderer>().material.color = new Color(0, genes.Genes["eye color"], 0); }
                if (gameObject.tag == "predator") { Eyes[i].GetComponent<Renderer>().material.color = new Color(genes.Genes["eye color"], 0, 0); }

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
        Heads[(int)genes.Genes["head"] - 1].transform.parent.localPosition = new Vector3(genes.Genes["head position x"],genes.Genes["head position y"],genes.Genes["head position z"]);


        //Debug.Log(Heads[(int)genes.head - 1].transform.parent.localPosition);
        //Genes["neck"]s.Genes["energy"] = Necks[(int)genes.neck];

        transform.parent.localScale = new Vector3(((float)genes.Genes["size"] * .05f), ((float)genes.Genes["size"]) * .05f, ((float)genes.Genes["size"]) * .05f);
        anim.torsoMesh.transform.localScale = new Vector3(genes.Genes["torso dimensions x"],genes.Genes["torso dimensions y"],genes.Genes["torso dimensions z"]);
        //calculate total Genes["energy"] expense
        genes.Genes["energy"] = (float) genes.Genes["size"] * ((float)genes.Genes["speed"] + (float)genes.Genes["look radius"]);
        //calculate total Genes["weight"]
        //cakculate max gestation
        if ((float)genes.Genes["max offspring"] > Mathf.RoundToInt((float)genes.Genes["weight"] / 30))
        {
            genes.Genes["max offspring"] = Mathf.RoundToInt((float)genes.Genes["weight"] / 30);
        }
        //setting legs to zero
        genes.Genes["speed"] = Mathf.Clamp(Convert.ToSingle(genes.Genes["speed"]), editor.maxSpeed[Mathf.RoundToInt(Convert.ToSingle(genes.Genes["leg"])) - 1] / Convert.ToSingle(genes.Genes["weight"]), editor.maxSpeed[Mathf.RoundToInt(Convert.ToSingle(genes.Genes["leg"])) - 1] * Convert.ToSingle(genes.Genes["legs"]) / Convert.ToSingle(genes.Genes["weight"]));
        int legs = 0;

        Transform parentTransform = LegComponents[(int)(genes.Genes["leg"]-1) * 2].transform; // The transform of the GameObject on the right side
        int childCount = parentTransform.childCount; // Get the number of children

        Legs = new GameObject[childCount]; // Initialize the array with the number of children

        for (int i = 0; i < childCount; i++)
        {
            Legs[i] = parentTransform.GetChild(i).gameObject; // Populate the array
        }
        for (int i = 0; i < 6; i++)
        {
            //print(Mathf.RoundToInt(Convert.ToSingle(genes.Genes["legs"])) - 1);
            if (Convert.ToSingle(genes.Genes["speed"]) >= editor.minSpeed[Mathf.RoundToInt(Convert.ToSingle(genes.Genes["leg"])) - 1] * (i + 1) / Convert.ToSingle(genes.Genes["weight"]))
            {
                Legs[i].SetActive(true);
                //adding legs
                legs++;
            }
            else
            {
                Legs[i].SetActive(false);
            }

        }
        if (legs < 2)
        {
            Legs[0].SetActive(true);
            Legs[1].SetActive(true);
        }
        legs = Mathf.Clamp(legs, 2, 6);
        genes.Genes["legs"] = legs;
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

        def = Convert.ToSingle(genes.Genes["weight"]) * Convert.ToSingle(genes.Genes["size"]);
        maxDef = Convert.ToSingle(genes.Genes["weight"]) * Convert.ToSingle(genes.Genes["size"]);
        attack = Convert.ToSingle(genes.Genes["speed"]) * (2 - Convert.ToSingle(genes.Genes["size"]));

        float startRot = UnityEngine.Random.Range(0, 360);
        anim.SetPosition(anim.Neck.transform, anim.Head.transform.position, anim.NeckPosition.position,0,20);
        transform.eulerAngles = new Vector3(0, startRot, 0);



        geneticDist = CalculateGeneticDistance(genes.Genes);

    }
    private void Avoidance()
    {
        Vector3 rayStart = transform.position + -transform.right * 3.0f;
        if (Physics.Raycast(rayStart + new Vector3(0, 10, 0), -Vector3.up, out RaycastHit hit, Mathf.Infinity, rayLayer))
        {
            //print(hit.point.y);
            if (hit.point.y <= Manager.settings.waterHeight || IsSteepTerrain(hit.normal))
            {
                avoiding = true;
                Vector3 avoidDirection = Vector3.Reflect(-transform.right, hit.normal);
                SteerAway();
            }
            else
            {
                avoiding = false;
            }
        }
    }
    public float CalculateGeneticDistance(Dictionary<string, float> genes)
    {
        float distanceSquared = 0;
        int index = 0;
        foreach (var gene in genes)
        {
            // Only consider genes between indices 3 and 15
            if (index >= 3 && index <= 15)
            {
                distanceSquared += gene.Value * gene.Value;
            }
            index++;
        }
        return (float)Math.Sqrt(distanceSquared);
    }

    private bool IsSteepTerrain(Vector3 normal)
    {
        // Check the slope of the terrain
        float slopeAngle = Vector3.Angle(Vector3.up, normal);
        return slopeAngle > 45f; // Adjust 45 to the maximum slope angle your creature can handle
    }

    private void SteerAway()
    {
        transform.Rotate(0, 80f / (float)genes.Genes["speed"] * Time.deltaTime, 0);
    }
    void OnDrawGizmos()
    {

        Vector3 rayStart = transform.position - transform.right * 3.0f + new Vector3(0, 10, 0);

        // Cast the ray
        if (Physics.Raycast(rayStart, -Vector3.up, out RaycastHit hit, Mathf.Infinity))
        {
            // Drawing the ray to the exact hit point
            Gizmos.color = Color.red;
            Gizmos.DrawLine(rayStart, hit.point);
        }
    }
}
