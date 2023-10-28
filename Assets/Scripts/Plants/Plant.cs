using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Plant : Entity
{
    
    [HideInInspector] public PlantEditor editor;
    [HideInInspector] public TreeGenerator generator;
    [HideInInspector] public GameManager manager;
    [HideInInspector] public TreeDB database;
    public PlantDetails details;
    public PlantManager pManager;
    private GameObject Seed, mate;
    private float range, tick, tick2;
    private float[] distanceColl;
    private Collider[] colliders;
    private Transform target;

    public float multiplier;
    private int life = 1;
    public int generation;
    public float energy;
    [SerializeField] private int type, seeds;

    public bool readyToMate;

    private Terrain terrain;
    private RaycastHit hit;
    public bool isTree;

    public void CopyDataFrom(Plant sourcePlant)
    {
        // Copy each variable from sourcePlant to this instance
        this.editor = sourcePlant.editor;
        this.generator = sourcePlant.generator;
        this.manager = sourcePlant.manager;
        this.database = sourcePlant.database;
        this.details = sourcePlant.details;  // Assuming PlantDetails is a value type or you're okay with a shallow copy
        this.pManager = sourcePlant.pManager;

        this.Seed = sourcePlant.Seed;
        this.mate = sourcePlant.mate;
        this.range = sourcePlant.range;
        this.tick = sourcePlant.tick;
        this.tick2 = sourcePlant.tick2;

        // For arrays, make sure to clone so that it's a separate copy
        if (sourcePlant.distanceColl != null)
        {
            this.distanceColl = (float[])sourcePlant.distanceColl.Clone();
        }
        else
        {
            this.distanceColl = null;
        }

        if (sourcePlant.colliders != null)
        {
            this.colliders = (Collider[])sourcePlant.colliders.Clone();
        }
        else
        {
            this.colliders = null;
        }

        this.target = sourcePlant.target;
        this.life = sourcePlant.life;
        this.energy = sourcePlant.energy;
        this.type = sourcePlant.type;
        this.seeds = sourcePlant.seeds;
        this.readyToMate = sourcePlant.readyToMate;
        this.terrain = sourcePlant.terrain;
        this.hit = sourcePlant.hit;
    }
    void Start()
    {
        init();
    }

    void Update()
    {
        if (Physics.Raycast(transform.position + new Vector3(0, 50, 0), -Vector3.up, out hit, Mathf.Infinity, 128))
        {
            // Assuming terrain is your Terrain object
            if (terrain != null)
            {
                // Get the terrain's texture data at the hit point
                Vector3 terrainPosition = hit.point - terrain.transform.position;
                Vector3 normalizedPosition = new Vector3(terrainPosition.x / terrain.terrainData.size.x, 0, terrainPosition.z / terrain.terrainData.size.z);
                float[,,] alphamap = terrain.terrainData.GetAlphamaps(Mathf.FloorToInt(normalizedPosition.x * terrain.terrainData.alphamapWidth), Mathf.FloorToInt(normalizedPosition.z * terrain.terrainData.alphamapHeight), 1, 1);

                // Check if the rock layer (index 2) is dominant at this position
                if (alphamap[0, 0, 2] > 0.5f) // 0.5f can be adjusted based on your preference
                {
                    Die();
                }
                else
                {
                    // Set the GameObject's position to the hit point's position
                    transform.position = hit.point - new Vector3(0,+0.25f,0);
                }
            }
        }
        if (transform.position.y < manager.settings.waterHeight)
        {
            Die();
        }
        tick -= Time.deltaTime;
        if(tick < 0)
        {
            Collider[] hitColliders1 = Physics.OverlapSphere(transform.position, 5, 8);

            //visual represetation/debugging
            colliders = hitColliders1;
            tick = 3;
        }
        tick -= Time.deltaTime;
        if(tick2 <= 0)
        {

            energy += (((details.genes[0] + details.genes[1]) / (15 * (colliders.Length * colliders.Length + 1))) - ((details.genes[0] + details.genes[1]) / 100.0f))*Time.deltaTime*20 * multiplier;            
            age += Time.deltaTime;
            transform.localScale += new Vector3(0.15f * Time.deltaTime, 0.15f * Time.deltaTime, 0.15f * Time.deltaTime);
            transform.localScale = new Vector3(Mathf.Clamp(transform.localScale.x, -1, 1), Mathf.Clamp(transform.localScale.y, -1, 1), Mathf.Clamp(transform.localScale.z, -1, 1));

        }
        if (transform.localScale.x < 0.01f) // check if object is small
        {
            Die();
        }
        else if (energy < 0)
        {
            Die();
        }
        if (energy > 100 && transform.localScale.x == 1) // arbitrary mating age
        {
            readyToMate = true;
        }
        else
        {
            readyToMate = false;
        }

        if (details.genes[11] == 1)//male or female
        {
            if (readyToMate == true)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    if(colliders[i] != null)
                    {
                        if (colliders[i].GetComponent<Plant>().details.gender != details.gender && colliders[i].GetComponent<Plant>().readyToMate == true && details.gender == 1 && colliders[i].GetComponent<Plant>().details.genes[11] == details.genes[11])
                        {
                            type = Convert.ToInt32(details.genes[11]) - 1;
                            energy -= 100;
                            mate = colliders[i].gameObject;
                            if (type != 0)
                                CreateSeeds();
                            seeds++;
                        }
                    }
                    

                }

            }       
        }
        if (details.genes[11] == 2)//both
        {
            type = Convert.ToInt32(details.genes[12]) -1;
            if (readyToMate == true)
            {
                mate = gameObject;
                energy -= 100;
                if(type != 0)
                    CreateSeeds();
                seeds++;
            }
        }
    }
    private void Die()
    {
        life--;
        if(life >= 0)
            manager.dManager.currentPlantCount--;
        Destroy(gameObject);
    }
    public void CreateSeeds()
    {
        if (seeds > 0)
        {
            //print(type);
            range = (1 - details.genes[10]) * 25 * manager.settings.seedRangeMultiplier;
            Vector3 random = new Vector3(UnityEngine.Random.Range(transform.position.x - range, transform.position.x + range), -12, UnityEngine.Random.Range(transform.position.z - range, transform.position.z + range));
            float chanceToAddSeed = details.genes[10]; // 70% chance to add seed, adjust as needed
            float randomChance = UnityEngine.Random.Range(0f, 1f); // Generate a random float between 0 and 1

            if (randomChance <= chanceToAddSeed)
            {
                pManager.AddSeed(GetComponent<PlantDetails>(), mate.GetComponent<PlantDetails>(), random, generation);
            }


            seeds--;
        }
    }
    public void Fruit(GameObject creature, float digestionDuration, int gen)
    {
        if (seeds > 0)
        {
            if(creature != null)
            {
                pManager.AddSeedFruit(GetComponent<PlantDetails>(), mate.GetComponent<PlantDetails>(), creature.transform, digestionDuration, gen);
            }
            seeds--;
        }
    }
    private void init()
    {
        manager.dManager.currentPlantCount++;
        manager.dManager.Plants.Add(GetComponent<Plant>());
        Seed = editor.seed;
        details = GetComponent<PlantDetails>();
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        details.gender = UnityEngine.Random.Range(0, 2);
        details.genes[11] = 2;

        multiplier = manager.settings.plantEnergyGainMultiplier;
        // Calculate LightConsumption (details.genes[0]) based on leaves size (details.genes[9]) and trunk size (details.genes[6])
        details.genes[0] = details.genes[9] * details.genes[6];

        // Calculate waterConsumption (details.genes[1]) based on root type and weight (details.genes[2])
        details.genes[1] = (1) * (details.genes[2] / 10.0f);

        // Calculate Weight (details.genes[2]) based on various parameters
        details.genes[2] = (3 * details.genes[6] * details.genes[7] * details.genes[4] * 0.25f * details.genes[9]) * 100;

        // Calculate Height (details.genes[3]) based on the first branch length (details.genes[4]) and floor height (details.genes[7])
        details.genes[3] = (details.genes[4] * details.genes[7]) * 100;

        if (Physics.Raycast(transform.position + new Vector3(0, 50, 0), -Vector3.up, out hit, Mathf.Infinity, 128))
        {
            // Assuming terrain is your Terrain object
            terrain = hit.collider.GetComponent<Terrain>();
            if (terrain != null)
            {
                // Get the terrain's texture data at the hit point
                Vector3 terrainPosition = hit.point - terrain.transform.position;
                Vector3 normalizedPosition = new Vector3(terrainPosition.x / terrain.terrainData.size.x, 0, terrainPosition.z / terrain.terrainData.size.z);
                float[,,] alphamap = terrain.terrainData.GetAlphamaps(Mathf.FloorToInt(normalizedPosition.x * terrain.terrainData.alphamapWidth), Mathf.FloorToInt(normalizedPosition.z * terrain.terrainData.alphamapHeight), 1, 1);
                transform.position = hit.point - new Vector3(0, 0.1f, 0);


                float slopeAngle = Vector3.Angle(Vector3.up, hit.normal);

                // Check if the rock layer (index 2) is dominant at this position
                if (alphamap[0, 0, 2] > 0.5f) // 0.5f can be adjusted based on your preference
                {
                    Die();

                }
                else if (slopeAngle > 45)
                {
                    Die();
                }

            }
        }
        if (isTree == false)
            StartCoroutine(generateTreeDelay());

    }
    IEnumerator generateTreeDelay()
    {

        yield return new WaitForSeconds(10);

        generator._firstBranchHeight = details.genes[4];
        generator._recursionLevel = 3;
        generator._trunkThickness = details.genes[6];
        generator._floorHeight = details.genes[7];
        generator._distorsionCone = details.genes[5];
        generator._twistiness = details.genes[8];
        generator._branchDensity = 0.25f;
        generator._leavesSize = details.genes[9];
        generator.gen2();

        manager.database.GenerateTree(generator.tree);
        manager.database.genes.Add(new List<float>());
        for (int k = 0; k < details.genes.Length; k++)
        {
            manager.database.genes[manager.database.genes.Count - 1].Add(details.genes[k]);
        }

        generator.tree.AddComponent<BoxCollider>();
        generator.tree.tag = "food";
        generator.tree.layer = 3;

        PlantDetails detail = generator.tree.AddComponent<PlantDetails>();
        detail.genes = details.genes;
        Plant plant = generator.tree.AddComponent<Plant>();
        plant.CopyDataFrom(GetComponent<Plant>());
        plant.generation = generation;
        plant.manager = manager;
        plant.editor = editor;
        plant.generator = generator;
        plant.pManager = pManager;
        plant.isTree = true;
        plant.transform.localScale = transform.localScale/2;
        generator.tree.AddComponent<BoxCollider>();
        generator.tree.tag = "food";
        generator.tree.layer = 3;
        generator.tree.transform.position = transform.position;

        Destroy(gameObject);
    }
}
