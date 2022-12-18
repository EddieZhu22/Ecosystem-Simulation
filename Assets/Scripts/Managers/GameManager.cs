using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameSettings settings;
    public TreeGenerator generator;
    public TreeDB database;
    public GameUI UI;
    public TimeManager tManager;
    public CreatureEditor cEditor;
    public PlantEditor pEditor;
    public PlantManager plantManager;


    public GameObject creaturePrefab, plantPrefab;//holds bot prefab
    public LayerMask rayLayer;
    public float timeframe, tick;
    [Range(0.0001f, 1f)] public float CreatureMutationChance = 0.01f;

    [Range(0f, 1f)] public float CreatureMutationStrength = 0.5f;

    [Range(0.0001f, 1f)] public float PlantMutationChance = 0.01f;

    [Range(0f, 1f)] public float PlantMutationStrength = 0.5f;

    [Range(0.1f, 10f)] public float Gamespeed = 1f;

    public Dictionary<int,Dictionary<string,object>> CreatureDetails = new Dictionary<int,Dictionary<string,object>>();

    public float[,] PlantDetails = new float[9, 14];

    public Dictionary<string,object> details = new Dictionary<string, object>();
    public List<float> details2;


    public LayerMask plantmask;
    [SerializeField] private Vector3 mapSize;

    public int numOfPlants;

    public float[] averages1, averages2, averages3;

    public List<Creature> creatures;

    public GameObject[] prey, predator;

    public Plant[] plants;

    void Start()
    {
        Gamespeed = 0.1f;
        InitializeGame();
    }

    void Update()
    {
        // slowly lerp through time, as to not destroy the game.
        tick -= Time.deltaTime;
        //set variables        

        Time.timeScale = Gamespeed;

        /*
        // Every so often update the UI and averages of the data. 
        if (tick < 0)
        {
            float[] temp1 = new float[18];
            float[] temp2= new float[18];
            float[] temp3 = new float[18];
            plants = GameObject.FindObjectsOfType<PlantBehavior>();
            prey = GameObject.FindGameObjectsWithTag("prey");
            predator = GameObject.FindGameObjectsWithTag("predator");
            for (int i = 0; i < creatures.Count; i++)
            {
                for(int k = 0; k < 18; k++)
                {
                    temp1[k] = creatures[i].stats.genes[k] + temp1[k];
             
                }
            }
            for (int i = 0; i < prey.Length; i++)
            {
                for (int k = 0; k < 18; k++)
                {
                    temp2[k] = prey[i].GetComponent<Creature>().stats.genes[k] + temp2[k];
                }
            }
            for (int i = 0; i < predator.Length; i++)
            {
                for (int k = 0; k < 18; k++)
                {
                    temp3[k] = predator[i].GetComponent<Creature>().stats.genes[k] + temp3[k];
                }
            }
            if (creatures.Count > 0)
            {
                for (int i = 0; i < 18; i++)
                {
                    averages1[i] = temp1[i] / creatures.Count;
                    averages2[i] = temp2[i] / prey.Length;
                    averages3[i] = temp3[i] / predator.Length;
                }
            }
            if(creatures.Count <= 0)
            {
                for (int i = 0; i < 18; i++)
                {
                    averages1[i] = 0;
                }
            }
            if (prey.Length <= 0)
            {
                for (int i = 0; i < 18; i++)
                {
                    averages2[i] = 0;
                }
            }
            if (predator.Length < 0)
            {
                for (int i = 0; i < 18; i++)
                {
                    averages3[i] = 0;
                }
            }
            UI.avgData.text = "Speed: " + averages1[3] + ", Look Radius: " + averages1[4]  + ", Gestation: " + averages1[5] + ", Creatures: " + creatures.Count + ", Plants: " + plants.Length;
            tick = -1;
        }
        // Set values instantly.
        CreatureMutationChance = UI.MutationChance.value;
        CreatureMutationStrength = UI.MutationStrength.value;
        PlantMutationChance = UI.MutationChance2.value;
        PlantMutationStrength = UI.MutationStrength2.value;*/
        settings.mode = UI.cameraSettings.value;
        //set camera values instantly.
        if (Input.GetKeyDown("h"))
        {
            float p = 0;
            if(UI.gameObject.activeSelf == false)
            {
                p = 1;
                UI.gameObject.SetActive(true);
            }
            else if(p == 0)
            {
                UI.gameObject.SetActive(false);
            }
        }
        if(Input.GetKeyDown("k"))
        {
            DitzelGames.FastIK.FastIKFabric[] ikArr = FindObjectsOfType<DitzelGames.FastIK.FastIKFabric>();
            for(int i = 0; i < ikArr.Length; i++)
            {
                ikArr[i].enabled = false;
            }
        }
    }
    private void InitializeGame()
    {
        for(int i = 0; i < 9; i++)
            CreatureDetails.Add(i,new Dictionary<string, object>());
    }
    private Vector3 randomPlacement()
    {
        return new Vector3(Random.Range(-mapSize.x / 2, mapSize.x / 2), Random.Range(-mapSize.y / 2, mapSize.y / 2), Random.Range(-mapSize.z / 2, mapSize.z / 2)); 
    }
    public void CreateCreatures(Vector3 pos, Dictionary<string,object> genes, bool external, string CreatureName)
    {
        if (external == false)
        {
            genes = CreatureDetails[UI.selected];
        }
        GameObject creatureObj = Instantiate(creaturePrefab, pos, Quaternion.identity);
        creatureObj.name = CreatureName + " child";
        Creature creature = creatureObj.GetComponentInChildren<Creature>();
        //Set Creature Details
        creature.genes = creatureObj.GetComponentInChildren<CreatureGenes>();
        creature.genes.Genes = genes;
        creature.Manager = GetComponent<GameManager>();
        creature.mutationChance = CreatureMutationChance;
        creature.mutationStrength = CreatureMutationStrength;
        creature.TimeManager = tManager;
        creature.editor = cEditor;
        creature.anim = creatureObj.GetComponent<CreatureProceduralAnimation>();
        creatures.Add(creature);
    }
    public void SpawnTrees()
    {
        //CreatePlants(details2,randomPlacement(),false);
    }
    public void SpawnAnimals()
    {
        Vector3 random = randomPlacement();
        Vector3 offset = new Vector3(0,50,0);
        if (Physics.Raycast(random + offset , Vector3.down, out RaycastHit hit, Mathf.Infinity, rayLayer))
        {
            CreateCreatures(hit.point, details, false, "Creature");
            //Debug.DrawLine(new Vector3(random.x, 500, random.z), hit.point, Color.cyan);
            //Debug.Log(hit.point);
        }
    }
    public void CreatePlants(List<float> genes, Vector3 pos, bool fromSeed)
    {
        bool match = false;
        if(database.genes.Count != 0 && fromSeed == false)
        {
            for (int i = 0; i < 14; i++)
            {
                genes[i] = PlantDetails[UI.selected2, i];
            }
            for (int i = 0; i < database.genes.Count; i++)
            {
                int num = 0;
                for (int k = 0; k < 14; k++)
                {
                    if (database.genes[i][k] == genes[k])
                    {
                        num++;
                    }
                }
                if (num >= 14)
                {
                    GameObject tree = Instantiate(database.TreeGameObjectDataBase[i], pos, new Quaternion(0, 0, 0, 0));
                    PlantDetails details = tree.AddComponent<PlantDetails>();
                    
                    details.genes = genes.ToArray();
                    Plant plant = tree.AddComponent<Plant>();
                    plant.manager = GetComponent<GameManager>();
                    plant.pManager = GetComponent<PlantManager>();
                    plant.editor = pEditor;

                    plant.generator = generator;

                    tree.AddComponent<MeshCollider>();
                    tree.tag = "food";
                    tree.layer = 3;
                    match = true;
                }

            }
        }
        
        if (match == false || fromSeed == true)
        {
            generator.seed = genes[4];
            generator._recursionLevel = (int)genes[5];
            generator._trunkThickness = genes[6];
            generator._floorHeight = genes[7];
            generator._firstBranchHeight = genes[8];
            generator._twistiness = genes[9];
            generator._branchDensity = genes[10];
            generator._leavesSize = genes[11];
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

            generator.tree.AddComponent<BoxCollider>();
            //generator.tree.AddComponent<MeshCollider>();
            generator.tree.tag = "food";
            generator.tree.layer = 3;
            generator.tree.transform.position = pos;
        }
    }
    
}
