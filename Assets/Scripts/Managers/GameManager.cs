using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float timeframe, temp, tick;
    public int populationSize;//creates population size
    public GameObject creaturePrefab, plantPrefab;//holds bot prefab
    public TreeGenerator generator;

    public TreeDB database;
    public LayerMask rayLayer;
    public GameUI UI;
    private GameSettings settings;
    public int[] layers = new int[3] { 5, 3, 2 };//initializing network to the right size

    [Range(0.0001f, 1f)] public float MutationChance = 0.01f;

    [Range(0f, 1f)] public float MutationStrength = 0.5f;

    [Range(0.0001f, 1f)] public float MutationChance2 = 0.01f;

    [Range(0f, 1f)] public float MutationStrength2 = 0.5f;

    [Range(0.1f, 10f)] public float Gamespeed = 1f;

    public float[,] CreatureDetails = new float[9, 18];

    public float[,] PlantDetails = new float[9, 14];

    public float[] details = new float[18], details2 = new float[14];

    //public List<Bot> Bots;
    public List<NeuralNetwork> networks;

    private List<Brain> Brains;

    public bool NN, placed, match;

    public LayerMask plantmask;
    [SerializeField] private Vector3 mapSize;

    public int numOfPlants;

    public float[] averages1, averages2, averages3;

    public Brain[] creatures;

    public GameObject[] prey, predator;

    public PlantBehavior[] plants;

    private int entered;


    void Start()
    {

        //Application.targetFrameRate = 300;
        //if (populationSize % 2 != 0)
        //     populationSize = populationSize + 1;

        //  InitNetworks();
        //CreateBots();
        //  InvokeRepeating("SortNetworks", 0.1f, timeframe);
        temp = Gamespeed;
        Gamespeed = 0.1f;
        UI = GameObject.Find("Canvas").GetComponent<GameUI>();
        settings = GetComponent<GameSettings>();
    }

    public void InitNetworks()
    {
        networks = new List<NeuralNetwork>();
        for (int i = 0; i < populationSize; i++)
        {
            NeuralNetwork net = new NeuralNetwork(layers);
            net.Load("Assets/Training Set/Save.txt");//on start load the network save
            networks.Add(net);
        }
    }

    public void CreateBots()
    {
        Brains = new List<Brain>();
        for (int i = 0; i < populationSize; i++)
        {
            Vector3 position = new Vector3(Random.Range(-mapSize.x / 2, mapSize.x / 2), 0, Random.Range(-mapSize.z / 2, mapSize.z / 2));
            Brain creature = (Instantiate(creaturePrefab, position, Quaternion.identity).GetComponent<Brain>());//create botes
            creature.network = networks[i];//deploys network to each learner
            Brains.Add(creature);
        }

    }
    public void SortNetworks()
    {
        for (int i = 0; i < Brains.Count; i++)
        {
            Brains[i].NNUpdateFitness();//gets bots to set their corrosponding networks fitness
        }
        networks.Sort();
        networks[Brains.Count - 1].Save("Assets/Training Set/Save.txt");//saves networks weights and biases to file, to preserve network performance
        /*for (int i = 0; i < populationSize / 2; i++)
        {
            networks[i] = networks[i + populationSize / 2].copy(new NeuralNetwork(layers));
            networks[i].Mutate((int)(1 / MutationChance), MutationStrength);
        }*/
    }
    void Update()
    {
        if(Input.GetKeyDown("f"))
        {
            entered++;
            MeshRenderer[] temp = GameObject.FindObjectsOfType<MeshRenderer>();
            SkinnedMeshRenderer[] temp2 =GameObject.FindObjectsOfType<SkinnedMeshRenderer>();
            if(entered == 1)
            {
                for (int i = 0; i < temp.Length; i++)
                {
                    if (temp[i].enabled != false)
                    {
                        temp[i].enabled = false;
                    }
                }
                for (int i = 0; i < temp2.Length; i++)
                {
                    temp2[i].enabled = false;
                }
            }
            else
            {
                for (int i = 0; i < temp.Length; i++)
                {
                    temp[i].enabled = true;
                   
                }
                for (int i = 0; i < temp2.Length; i++)
                {
                    temp2[i].enabled = true;
                }
                entered = 0;
            }
        }
        // slowly lerp through time, as to not destroy the game.
        tick -= Time.deltaTime;
        temp -= 0.1f;
        if (temp > 0)
        {
            Gamespeed += 0.1f;
        }

        //set variables        

        Time.timeScale = Gamespeed;

        //check to see if Neural Network is On
        NN = UI.NN.isOn;

        // Every so often update the UI and averages of the data. 
        if (tick < 0)
        {
            float[] temp1 = new float[18];
            float[] temp2= new float[18];
            float[] temp3 = new float[18];
            plants = GameObject.FindObjectsOfType<PlantBehavior>();
            creatures = GameObject.FindObjectsOfType<Brain>();
            prey = GameObject.FindGameObjectsWithTag("prey");
            predator = GameObject.FindGameObjectsWithTag("predator");
            for (int i = 0; i < creatures.Length; i++)
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
                    temp2[k] = prey[i].GetComponent<Brain>().stats.genes[k] + temp2[k];
                }
            }
            for (int i = 0; i < predator.Length; i++)
            {
                for (int k = 0; k < 18; k++)
                {
                    temp3[k] = predator[i].GetComponent<Brain>().stats.genes[k] + temp3[k];
                }
            }
            if (creatures.Length > 0)
            {
                for (int i = 0; i < 18; i++)
                {
                    averages1[i] = temp1[i] / creatures.Length;
                    averages2[i] = temp2[i] / prey.Length;
                    averages3[i] = temp3[i] / predator.Length;
                }
            }
            if(creatures.Length <= 0)
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
            UI.avgData.text = "Speed: " + averages1[3] + ", Look Radius: " + averages1[4]  + ", Gestation: " + averages1[5] + ", Creatures: " + creatures.Length + ", Plants: " + plants.Length;
            tick = -1;
        }
        // Set values instantly.
        MutationChance = UI.MutationChance.value;
        MutationStrength = UI.MutationStrength.value;
        MutationChance2 = UI.MutationChance2.value;
        MutationStrength2 = UI.MutationStrength2.value;
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
    }
    public void CreateCreatures(Vector3 pos)
    {
        for (int i = 0; i < 18; i++)
        {
            details[i] = CreatureDetails[UI.selected, i];
        }

        //Debug.Log(details[15]);
        //Debug.Log(CreatureDetails[0, 15]);
        //Debug.Log(CreatureDetails[1, 15]);
        creaturePrefab.GetComponent<CreatureBehavior>().torsoBody.GetComponent<CreatureDetails>().genes = details;
        GameObject creature = (GameObject)Instantiate(creaturePrefab, pos, Quaternion.identity);
    }
    public void SpawnTrees()
    {
        CreatePlants(new Vector3(Random.Range(-mapSize.x/2, mapSize.x/2), Random.Range(-mapSize.y/2, mapSize.y/2), Random.Range(-mapSize.z/2, mapSize.z/2)));
    }
    public void SpawnAnimals()
    {
        Vector3 random = new Vector3(Random.Range(-mapSize.x / 2, mapSize.x / 2), 500, Random.Range(-mapSize.z / 2, mapSize.z / 2));
        if (Physics.Raycast(new Vector3(random.x,500,random.z),-Vector3.up,out RaycastHit hit,Mathf.Infinity,rayLayer))
        {
            CreateCreatures(hit.point);
            Debug.DrawLine(new Vector3(random.x, 500, random.z), hit.point, Color.cyan);
            Debug.Log(hit.point);
        }
    }
    public void CreatePlants(Vector3 pos)
    {
        for (int i = 0; i < 14; i++)
        {
            details2[i] = PlantDetails[UI.selected2, i];
        }
        //Debug.Log(details2.Length);
        for (int i = 0; i < 100; i++)
        {
            int num = 0;
            for (int k = 0; k < 14; k++)
            {
                if (database.genes[i, k] == details2[k])
                {
                    num++;
                    //Debug.Log(num);
                }
            }
            if (num >= 14)
            {
                GameObject tree = Instantiate(database.TreeGameObjectDataBase[i], pos, new Quaternion(0, 0, 0, 0));
                PlantDetails detail = tree.AddComponent<PlantDetails>();

                detail.genes = details2;
                tree.AddComponent<PlantBehavior>();
                tree.AddComponent<MeshCollider>();
                tree.tag = "food";
                tree.layer = 3;
                match = true;
            }

        }
        if (match == false)
        {
            for (int i = 0; i < database.TreeGameObjectDataBase.Length; i++)
            {
                if (database.TreeGameObjectDataBase[i] == null && placed == false)
                {
                    generator.seed = details2[4];
                    generator._recursionLevel = (int)details2[5];
                    generator._trunkThickness = details2[6];
                    generator._floorHeight = details2[7];
                    generator._firstBranchHeight = details2[8];
                    generator._twistiness = details2[9];
                    generator._branchDensity = details2[10];
                    generator._leavesSize = details2[11];
                    generator.gen2();
                    for (int k = 0; k < details2.Length; k++)
                    {
                        database.genes[i, k] = details2[k];
                    }
                    //database.TreeGameObjectDataBase[i] = generator.tree;
                    database.GenerateTree(generator.tree, i);
                    //database.TreeGameObjectDataBase = new GameObject[database.TreeGameObjectDataBase.Length + 1];


                    PlantDetails detail = generator.tree.AddComponent<PlantDetails>();
                    detail.genes = details2;
                    generator.tree.AddComponent<PlantBehavior>();
                    generator.tree.AddComponent<MeshCollider>();
                    generator.tree.tag = "food";
                    generator.tree.layer = 3;
                    generator.tree.transform.position = pos;
                    placed = true;
                }
            }
            /*
            for (int i = 0; i < 14; i++)
            {
                details2[i] = PlantDetails[UI.selected2, i];
            }
            plantPrefab.GetComponent<PlantDetails>().genes = details2;

            generator.seed = details2[4];
            generator._recursionLevel = (int)details2[5];
            generator._trunkThickness = details2[6];
            generator._floorHeight = details2[7];
            generator._firstBranchHeight = details2[8];
            generator._twistiness = details2[9];
            generator._branchDensity = details2[10];
            generator._leavesSize = details2[11];
            generator.gen2();
            PlantDetails detail = generator.tree.AddComponent<PlantDetails>();
            detail.genes = details2;
            generator.tree.AddComponent<PlantBehavior>();
            generator.tree.AddComponent<MeshCollider>();
            generator.tree.tag = "food";
            generator.tree.layer = 3;
            */
        }
    }

}
