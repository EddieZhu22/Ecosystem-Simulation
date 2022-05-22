using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed : MonoBehaviour
{
    public float dormantTime;

    public GameObject p1, p2;

    public TreeGenerator generator;

    public GameObject database, canvas;
    public PlantEditor editor;
    public float[] genes;
    public float t, dd;

    public int type;

    public bool match;

    public bool placed;
    void Start()
    {
        editor = p2.GetComponent<PlantBehavior>().editor;
        generator = p2.GetComponent<PlantBehavior>().generator;
        canvas = generator.gameObject.GetComponent<GameManager>().UI.gameObject;
        database = generator.gameObject.GetComponent<GameManager>().database;
        genes = new float[14];
        if (type != 0 || type != 4)
        {
            float rand = Random.Range(0, 1);
            if(rand < 0.3f)
            {
                Debug.Log(rand);
                //Destroy(gameObject);
            }
        }
        for (int i = 0; i < 14; i++)
        {
            genes[i] = (p1.GetComponent<PlantDetails>().genes[i] + p2.GetComponent<PlantDetails>().genes[i]) / 2;

            float rand = Random.Range(0, 1);

            if (rand < canvas.GetComponent<GameUI>().MutationChance2.value)
            {
                //mutate
                genes[i] += Random.Range(-canvas.GetComponent<GameUI>().MutationStrength.value * genes[i], canvas.GetComponent<GameUI>().MutationStrength.value * genes[i]);
            }
        }
        //mutate

    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < -3)
        {
            Destroy(gameObject);
        }
        if (type != 0 || type != 4)
        {
            t += Time.timeScale;
            if (t > dormantTime)
            {
                CreateTree();
            }
        }
        if (type == 0 || type == 4)
        {
            t += Time.timeScale;
            transform.localPosition = Vector3.zero;
            if (t > dd)
            {
                GetComponent<Rigidbody>().isKinematic = false;
                GetComponent<SphereCollider>().enabled = true;
                GetComponent<MeshRenderer>().enabled = true;
                transform.parent = null;
                type = -1;
                t = 0;
            }
        }
    }
    void CreateTree()
    {
        for (int i = 0; i < 100; i++)
        {
            int num = 0;
            for (int k = 0; k < genes.Length; k++)
            {
                if (database.GetComponent<TreeDB>().genes[i, k] == genes[k])
                {
                    num++;
                    //Debug.Log(num);
                }
            }
            if (num >= 14)
            {
                GameObject tree = Instantiate(database.GetComponent<TreeDB>().TreeGameObjectDataBase[i], transform.position, new Quaternion(0, 0, 0, 0));
                PlantDetails detail = tree.AddComponent<PlantDetails>();

                detail.genes = genes;
                tree.AddComponent<PlantBehavior>();
                tree.AddComponent<MeshCollider>();
                tree.GetComponent<PlantBehavior>().editor = editor;
                tree.GetComponent<PlantBehavior>().generator = generator;
                tree.tag = "food";
                tree.layer = 3;
                match = true;
            }

        }
        if (match == false)
        {
            for (int i = 0; i < database.GetComponent<TreeDB>().TreeGameObjectDataBase.Length; i++)
            {
                if (database.GetComponent<TreeDB>().TreeGameObjectDataBase[i] == null && placed == false)
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
                    for (int k = 0; k < genes.Length; k++)
                    {
                        database.GetComponent<TreeDB>().genes[i, k] = genes[k];
                    }
                    //database.TreeGameObjectDataBase[i] = generator.tree;
                    database.GetComponent<TreeDB>().GenerateTree(generator.tree, i);
                    //database.TreeGameObjectDataBase = new GameObject[database.TreeGameObjectDataBase.Length + 1];


                    PlantDetails detail = generator.tree.AddComponent<PlantDetails>();
                    detail.genes = genes;
                    generator.tree.AddComponent<PlantBehavior>();
                    generator.tree.AddComponent<MeshCollider>();
                    generator.tree.tag = "food";
                    generator.tree.layer = 3;
                    generator.tree.transform.position = transform.position;
                    placed = true;
                }
            }
        }
        Destroy(gameObject);
    }
}
