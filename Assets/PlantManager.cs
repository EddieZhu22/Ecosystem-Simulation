using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlantManager : MonoBehaviour
{
    [SerializeField] private GameUI canvas;
    [SerializeField] private TreeDB database;
    [SerializeField] private GameManager manager;
    public void AddSeed(PlantDetails p1, PlantDetails p2, Vector3 pos)
    {
        float[] genes = new float[14];
        // set genes of the plant
        for (int i = 0; i < 14; i++)
        {
            genes[i] = (p1.genes[i] + p2.genes[i]) / 2;

            float rand = Random.Range(0, 1);
            if (i < 12)
            {
                if (rand < canvas.MutationChance2.value)
                {
                    //mutate
                    genes[i] += Random.Range(-canvas.MutationStrength2.value * genes[i], canvas.MutationStrength2.value * genes[i]);
                }
            }

        }
        CreateTree(genes.ToList(),pos);
    }
    private void CreateTree(List<float> genes, Vector3 pos)
    {
        bool match = false;
        for (int i = 0; i < database.genes.Count; i++)
        {
            int num = 0;
            for (int k = 0; k < genes.Count; k++)
            {
                if (database.genes[i][k] == genes[k])
                {
                    num++;
                    //Debug.Log(num);
                }
            }
            if (num >= 14)
            {
                GameObject tree = Instantiate(database.GetComponent<TreeDB>().TreeGameObjectDataBase[i], pos, new Quaternion(0, 0, 0, 0));
                PlantDetails detail = tree.AddComponent<PlantDetails>();

                detail.genes = genes.ToArray();
                tree.AddComponent<BoxCollider>();
                Plant plant = tree.AddComponent<Plant>();
                plant.pManager = GetComponent<PlantManager>();
                plant.manager = manager;
                plant.editor = manager.pEditor;
                plant.generator = manager.generator;
                tree.tag = "food";
                tree.layer = 3;
                match = true;
            }

        }
        if (match == false)
        {
            manager.CreatePlants(genes, pos, true);
        }
    }

}
