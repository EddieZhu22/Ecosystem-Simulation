using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlantManager : MonoBehaviour
{
    [SerializeField] private GameUI canvas;
    [SerializeField] private TreeDB database;
    [SerializeField] private GameManager manager;
    public void AddSeed(PlantDetails p1, PlantDetails p2, Vector3 pos, int gen)
    {
        float[] genes = new float[13];
        // set genes of the plant
        for (int i = 0; i < 13; i++)
        {
            genes[i] = (p1.genes[i] + p2.genes[i]) / 2;

            float rand = Random.Range(0, 1);
            if (i <= 10 && i >= 4)
            {
                if (rand < canvas.MutationChance2.value)
                {
                    //mutate
                    genes[i] += Random.Range((int)(-canvas.MutationStrength2.value * genes[i] * 100) / 100, (int)(canvas.MutationStrength2.value * genes[i] * 100) / 100);
                }
            }

        }
        CreateTree(genes.ToList(),pos,gen);
    }
    public void AddSeedFruit(PlantDetails p1, PlantDetails p2, Transform pos, float digestionDuration, int gen)
    {
        float[] genes = new float[13];
        // set genes of the plant
        for (int i = 0; i < 13; i++)
        {
            genes[i] = (p1.genes[i] + p2.genes[i]) / 2;

            float rand = Random.Range(0, 1);
            if (i <= 10 && i >= 4)
            {
                if (rand < canvas.MutationChance2.value)
                {
                    //mutate
                    genes[i] += Random.Range((int)(-canvas.MutationStrength2.value * genes[i] * 100) / 100, (int)(canvas.MutationStrength2.value * genes[i] * 100) / 100);
                }
            }
        }
        StartCoroutine(Digestion(genes, pos, digestionDuration,gen));
    }
    IEnumerator Digestion(float[] genes, Transform pos, float digestionDuration, int gen)
    {
        float minRange = 0.5f; // -10%
        float maxRange = 1.5f; // +10%
        float randomMultiplier = Random.Range(minRange, maxRange);

        // Adjust the digestionDuration
        float adjustedDigestionDuration = digestionDuration * randomMultiplier;
        Vector3 lastKnownPosition = Vector3.zero;

        float xOffset = Random.Range(-5f, 5f);  // Random offset between -5 and 5
        float zOffset = Random.Range(-5f, 5f);
        while (true)
        {
            if (pos != null)
            {
                lastKnownPosition = pos.position + new Vector3(xOffset, 0, zOffset);  // Save the last known position
                //print(pos.position);
            }
            else
            {
                // pos is null, use last known position to create the seed
                //print("End");
                CreateTree(genes.ToList(), lastKnownPosition + new Vector3(xOffset, 0, zOffset),gen);
                break;  // Exit the loop and coroutine
            }

            yield return null;  // Wait for the next frame
        }
        // Wait for some time (digestionDuration/10)
        yield return new WaitForSeconds(adjustedDigestionDuration / 50);
        //print("End");
        CreateTree(genes.ToList(), lastKnownPosition + new Vector3(xOffset, 0, zOffset),gen);
        // Continuously check for pos validity

    }

    private void CreateTree(List<float> genes, Vector3 pos, int generation)
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
            if (num >= 13)
            {
                GameObject tree = Instantiate(database.GetComponent<TreeDB>().TreeGameObjectDataBase[i], pos, new Quaternion(0, 0, 0, 0));
                PlantDetails detail = tree.AddComponent<PlantDetails>();

                detail.genes = genes.ToArray();
                tree.AddComponent<BoxCollider>();
                Plant plant = tree.AddComponent<Plant>();
                plant.pManager = GetComponent<PlantManager>();

                generation++;

                plant.generation = generation;

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
            generation++;
            manager.CreatePlants(genes, pos, true,generation);
        }
    }
    
}
