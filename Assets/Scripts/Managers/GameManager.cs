using System.Collections.Generic;
using UnityEngine;
using System;
public class GameManager : MonoBehaviour
{
    [Header("Managers and Databases")]
    public GameSettings settings;
    public TreeGenerator generator;
    public DataManager dManager;
    public TreeDB database;
    public GameUI UI;
    public TimeManager tManager;
    public TerrainTools TerrainTool;
    public AudioSource music;

    [Header("Editors")]
    public CreatureEditor cEditor;
    public PlantEditor pEditor;

    [Header("Prefabs and Objects")]
    public GameObject creaturePrefab; // holds bot prefab
    public GameObject Sun;
    public GameObject youngTree;

    [Header("Mutation Settings")]
    [SerializeField, Range(0.0001f, 1f)] public float CreatureMutationChance = 0.01f;
    [SerializeField, Range(0f, 1f)] public float CreatureMutationStrength = 0.5f;
    [SerializeField, Range(0.0001f, 1f)] public float PlantMutationChance = 0.01f;
    [SerializeField, Range(0f, 1f)] public float PlantMutationStrength = 0.5f;

    [Header("Game Speed")]
    [SerializeField, Range(0.1f, 10f)] public float Gamespeed = 1f;

    [Header("Creature Details")]
    public Dictionary<int, Dictionary<string, float>> CreatureDetails = new Dictionary<int, Dictionary<string, float>>();
    public float[] averages1, averages2, averages3;
    public double creatureCount;
    //public List<Creature> creatures;

    [Header("Plant Details")]
    public float[,] PlantDetails = new float[9, 14];
    public GameObject[] prey, predator;
    public Plant[] plants;
    public Material plantLeaves, plantFruit;

    [Header("Other Details")]
    public Dictionary<string, float> details = new Dictionary<string, float>();
    public List<float> details2;

    [Header("Map Details")]
    [SerializeField] private Vector3 mapSize;

    [Header("Raycast Settings")]
    [SerializeField] private LayerMask rayLayer;

    void Start()
    {
        Gamespeed = 0.1f;
        InitializeGame();
    }

    void Update()
    {
        try
        {
            if((Camera.main.transform.position.y - settings.waterHeight) > 0)
            {
                music.volume = settings.volume * Mathf.Min(0.5f,(settings.volume/2f) * (1 / ((Camera.main.transform.position.y - settings.waterHeight) / 15)));
                music.pitch = 1;
            }
            else
            {
                music.volume = settings.volume * 0.5f;
                music.pitch = 0.1f;
            }
        }
        catch { }

        Time.timeScale = Gamespeed;

        details2 = new List<float>(); // Initialize the list if not already done

        for (int j = 0; j < 14; j++) // Assuming the number of columns is 14
        {
            float value = PlantDetails[UI.selected2, j]; // Accessing the second row, column varies
            details2.Add(value);
        }

    }
    private void InitializeGame()
    {
        for(int i = 0; i < 9; i++)
            CreatureDetails.Add(i,new Dictionary<string, float>());
    }
    private Vector3 randomPlacement()
    {
        return new Vector3(UnityEngine.Random.Range(-mapSize.x / 2, mapSize.x / 2), UnityEngine.Random.Range(-mapSize.y / 2, mapSize.y / 2), UnityEngine.Random.Range(-mapSize.z / 2, mapSize.z / 2)); 
    }
    public void CreateCreatures(Vector3 pos, Dictionary<string,float> genes, bool external, string CreatureName, int generation)
    {
        if (external == false)
        {
            genes = CreatureDetails[UI.selected];
        }
        GameObject creatureObj = Instantiate(creaturePrefab, pos, Quaternion.identity);
        creatureObj.name = CreatureName + " child";
        Creature creature = creatureObj.GetComponentInChildren<Creature>();

        creature.generation = generation;
        //Set Creature Details
        creature.genes = creatureObj.GetComponentInChildren<CreatureGenes>();
        creature.genes.Genes = genes;
        if (Convert.ToSingle(genes["is predator"]) == 0) dManager.currentHerbivoreCount++; else dManager.currentPredatorCount++;
        creature.Manager = GetComponent<GameManager>();
        creature.mutationChance = CreatureMutationChance;
        creature.mutationStrength = CreatureMutationStrength;
        creature.TimeManager = tManager;
        creature.editor = cEditor;
        creature.anim = creatureObj.GetComponent<CreatureProceduralAnimation>();
    }
    public void SpawnTrees()
    {
        CreatePlants(details2,randomPlacement(),false,0);
    }
    public void SpawnAnimals()
    {
        Vector3 random = randomPlacement();
        Vector3 offset = new Vector3(0,50,0);
        if (Physics.Raycast(random + offset , Vector3.down, out RaycastHit hit, Mathf.Infinity, rayLayer))
        {
            CreateCreatures(hit.point, details, false, "Creature",0);
        }
    }
    public void CreatePlants(List<float> genes, Vector3 pos, bool fromSeed, int gen)
    {
        generator.leavesMaterial = genes[12] == 1 ? plantFruit : plantLeaves;
        bool match = false;
        if(database.genes.Count != 0 && fromSeed == false)
        {
            for (int i = 0; i < 13; i++)
            {
                genes[i] = PlantDetails[UI.selected2, i];
            }
            for (int i = 0; i < database.genes.Count; i++)
            {
                int num = 0;
                for (int k = 0; k < 13; k++)
                {
                    if (database.genes[i][k] == genes[k])
                    {
                        num++;
                    }
                }
                if (num >= 13)
                {
                    GameObject tree = Instantiate(database.TreeGameObjectDataBase[i], pos, new Quaternion(0, 0, 0, 0));
                    PlantDetails details = tree.AddComponent<PlantDetails>();
                    
                    details.genes = genes.ToArray();
                    Plant plant = tree.AddComponent<Plant>();
                    plant.manager = GetComponent<GameManager>();
                    plant.pManager = GetComponent<PlantManager>();
                    plant.editor = pEditor;
                    plant.isTree = true;
                    plant.generation = gen;
                    plant.generator = generator;

                    tree.AddComponent<BoxCollider>();

                    tree.tag = "food";
                    tree.layer = 3;
                    match = true;
                }

            }
        }
        
        if (match == false || fromSeed == true)
        {
            if (fromSeed == false)
            {
                generator._firstBranchHeight = genes[4];
                generator._recursionLevel = 3;
                generator._trunkThickness = genes[6];
                generator._floorHeight = genes[7];
                generator._distorsionCone = genes[5];
                generator._twistiness = genes[8];
                generator._branchDensity = 0.25f;
                generator._leavesSize = genes[9];
                generator.gen2();

                //Update Tree Object DataBase
                database.GenerateTree(generator.tree);

                // Update gene database
                database.genes.Add(new List<float>());
                for (int k = 0; k < genes.Count; k++)
                {
                    database.genes[database.genes.Count - 1].Add(genes[k]);
                }

                PlantDetails detail = generator.tree.AddComponent<PlantDetails>();
                detail.genes = genes.ToArray();
                Plant plant = generator.tree.AddComponent<Plant>();
                plant.manager = GetComponent<GameManager>();
                plant.editor = pEditor;
                plant.generator = generator;
                plant.pManager = GetComponent<PlantManager>();
                plant.isTree = true;
                plant.generation = gen;
                generator.tree.AddComponent<BoxCollider>();
                //generator.tree.AddComponent<MeshCollider>();
                generator.tree.tag = "food";
                generator.tree.layer = 3;
                generator.tree.transform.position = pos;
            }
            else
            {
                                /*generator._firstBranchHeight = genes[4];
                generator._recursionLevel = 3;
                generator._trunkThickness = genes[6];
                generator._floorHeight = genes[7];
                generator._distorsionCone = genes[5];
                generator._twistiness = genes[8];
                generator._branchDensity = 0.25f;
                generator._leavesSize = genes[9];
                generator.gen2();

                //Update Tree Object DataBase
                database.GenerateTree(generator.tree);

                // Update gene database
                database.genes.Add(new List<float>());
                for (int k = 0; k < genes.Count; k++)
                {
                    database.genes[database.genes.Count - 1].Add(genes[k]);
                }*/
                GameObject sapling = Instantiate(youngTree);

                PlantDetails detail = sapling.AddComponent<PlantDetails>();
                detail.genes = genes.ToArray();
                Plant plant = sapling.AddComponent<Plant>();
                plant.manager = GetComponent<GameManager>();
                plant.editor = pEditor;
                plant.generator = generator;
                plant.pManager = GetComponent<PlantManager>();
                plant.database = database;
                plant.isTree = false;
                plant.generation = gen;

                sapling.AddComponent<BoxCollider>();
                //generator.tree.AddComponent<MeshCollider>();
                sapling.tag = "food";
                sapling.layer = 3;
                sapling.transform.position = pos;
            }

        }
    }
    
}
