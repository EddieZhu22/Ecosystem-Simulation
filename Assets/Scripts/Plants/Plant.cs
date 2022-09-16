using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlantBehavior : Entity
{
    public PlantDetails details;
    public PlantEditor editor;
    public TreeGenerator generator;

    public GameObject seed, mate;
    public float range, tick, tick2;
    public float[] distanceColl;
    public Collider[] colliders;
    public Transform target;


    public float energy;
    public int type, seeds;

    public bool readyToMate;
    private RaycastHit hit;


    void Start()
    {
        init();
    }

    void Update()
    {
        tick -= Time.deltaTime;
        if(tick < 0)
        {
            Collider[] hitColliders1 = Physics.OverlapSphere(transform.position, 5, 8);

            //visual represetation/debugging
            colliders = hitColliders1;
            tick = 10;
        }
        tick -= 1 * Time.timeScale;
        if(tick2 <= 0)
        {
            energy += (((details.genes[0] + details.genes[1]) / (15 * (colliders.Length * colliders.Length + 1))) - ((details.genes[0] + details.genes[1]) / 100));            
            age += Time.timeScale / 100;
            tick = 10;
            
            transform.localScale += new Vector3(0.0015f * Time.timeScale, 0.0015f * Time.timeScale, 0.0015f * Time.timeScale);
            transform.localScale = new Vector3(Mathf.Clamp(transform.localScale.x, -1, 1), Mathf.Clamp(transform.localScale.y, -1, 1), Mathf.Clamp(transform.localScale.z, -1, 1));

        }
        if (transform.localScale.x < 0.1f) // check if object is small
        {
            Destroy(gameObject);
        }
        if (energy > 100 && age > 5) // arbitrary mating age
        {
            readyToMate = true;
        }
        else
        {
            readyToMate = false;
        }
        if (energy < 0)
        {
            Destroy(gameObject);
        }
        if (details.genes[12] == 1)//male or female
        {
            if (readyToMate == true)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    if(colliders[i] != null)
                    {
                        if (colliders[i].GetComponent<PlantBehavior>().details.gender != details.gender && colliders[i].GetComponent<PlantBehavior>().readyToMate == true && details.gender == 1 && colliders[i].GetComponent<PlantBehavior>().details.genes[13] == details.genes[13])
                        {
                            type = Convert.ToInt32(details.genes[13]) - 1;
                            energy -= 100;
                            mate = colliders[i].gameObject;
                            CreateSeeds();
                            seeds++;
                        }
                    }
                    

                }

            }       
        }
        if (details.genes[12] == 2)//both
        {
            type = Convert.ToInt32(details.genes[13]) - 1;
            if (readyToMate == true)
            {
                mate = gameObject;
                energy -= 100;
                CreateSeeds();
                seeds++;
            }
        }
    }
    public void CreateSeeds()
    {
        if (seeds > 0)
        {
            if (type == 2) // flying seeds
            {
                range = 100;
            }
            if (type == 1) // hard seeds
            {
                range = 0;
            }
            Vector3 random = new Vector3(UnityEngine.Random.Range(transform.position.x - range, transform.position.x + range), 0, UnityEngine.Random.Range(transform.position.z - range, transform.position.z + range));
            GameObject seedObj = Instantiate(seed, random, Quaternion.identity);
            Seed seedScript = seedObj.GetComponent<Seed>();
            seedScript.p2 = this.gameObject;
            seedScript.p1 = mate;
            seedScript.type = type;
            seedScript.dormantTime = 1000;
            seeds--;
        }
    }
    public void Fruit(GameObject creature, float digestionDuration)
    {
        if (seeds > 0)
        {
            if (type == 4)
            {
                int rand = UnityEngine.Random.Range(0, 100);
                if (rand >= 50)
                {
                    Destroy(creature);
                }
            }
            if(creature != null)
            {
                GameObject seedObj = Instantiate(seed, creature.transform.position, Quaternion.identity);
                Seed seedScript = seedObj.GetComponent<Seed>();
                seedObj.transform.parent = creature.transform;
                seedObj.GetComponent<SphereCollider>().enabled = false;
                //seedObj.GetComponent<Rigidbody>().isKinematic = true;
                seedScript.p2 = this.gameObject;
                seedScript.p1 = mate;
                seedScript.type = type;
                seedScript.dd = digestionDuration;
                seedScript.dormantTime = 100;
                seeds--;
            }

        }
    }
    private void init()
    {
        if (editor == null)
            editor = GameObject.Find("Plant Editor").GetComponent<PlantEditor>();
        seed = editor.seed;
        details = GetComponent<PlantDetails>();
        if (generator == null)
            generator = GameObject.Find("GameManager").GetComponent<TreeGenerator>();
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        details.gender = UnityEngine.Random.Range(0, 2);
        if (Physics.Raycast(transform.position + new Vector3(0, 1000, 0), -Vector3.up, out hit, Mathf.Infinity, 128))
        {
            transform.position = hit.point - new Vector3(0, 0.1f, 0);
        }
        if (transform.position.y < 3)
        {
            Destroy(gameObject);
        }
    }
}
