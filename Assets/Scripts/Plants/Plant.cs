using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Plant : Entity
{
    
    [HideInInspector] public PlantEditor editor;
    [HideInInspector] public TreeGenerator generator;
    [HideInInspector] public GameManager manager;
    private PlantDetails details;
    public PlantManager pManager;
    private GameObject Seed, mate;
    private float range, tick, tick2;
    private float[] distanceColl;
    private Collider[] colliders;
    private Transform target;


    [SerializeField] private float energy;
    [SerializeField] private int type, seeds;

    [SerializeField] private bool readyToMate;
    private RaycastHit hit;
    void Start()
    {
        init();
    }

    void Update()
    {
        if (transform.position.y < manager.settings.waterHeight)
        {
            Destroy(gameObject);
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
            energy += (((details.genes[0] + details.genes[1]) / (15 * (colliders.Length * colliders.Length + 1))) - ((details.genes[0] + details.genes[1]) / 100))*Time.deltaTime*20;            
            age += Time.deltaTime;            
            transform.localScale += new Vector3(0.15f * Time.deltaTime, 0.15f * Time.deltaTime, 0.15f * Time.deltaTime);
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
                        if (colliders[i].GetComponent<Plant>().details.gender != details.gender && colliders[i].GetComponent<Plant>().readyToMate == true && details.gender == 1 && colliders[i].GetComponent<Plant>().details.genes[13] == details.genes[13])
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
                range = 25;
            }
            if (type == 1) // hard seeds
            {
                range = 0;
            }
            Vector3 random = new Vector3(UnityEngine.Random.Range(transform.position.x - range, transform.position.x + range), 0, UnityEngine.Random.Range(transform.position.z - range, transform.position.z + range));

            pManager.AddSeed(GetComponent<PlantDetails>(),mate.GetComponent<PlantDetails>(),random);

            seeds--;
        }
    }
    public void Fruit(GameObject creature, float digestionDuration)
    {
        /*if (seeds > 0)
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
                GameObject seedObj = Instantiate(Seed, creature.transform.position, Quaternion.identity);
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

        }*/
    }
    private void init()
    {
        Seed = editor.seed;
        details = GetComponent<PlantDetails>();
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        details.gender = UnityEngine.Random.Range(0, 2);
        if (Physics.Raycast(transform.position + new Vector3(0, 50, 0), -Vector3.up, out hit, Mathf.Infinity, 128))
        {
            transform.position = hit.point - new Vector3(0, 0.1f, 0);
        }

    }
}
