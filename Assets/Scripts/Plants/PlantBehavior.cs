using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantBehavior : MonoBehaviour
{
    public PlantDetails details;
    public PlantEditor editor;
    public TreeGenerator generator;

    public GameObject seed, mate;
    public float age, range, tick, tick2;
    public float[] distanceColl;
    public Collider[] colliders;
    public Transform target;


    public float energy;
    public int type, seeds, init;

    public bool readyToMate;
    private RaycastHit hit;


    void Start()
    {
        if(editor == null)
        editor = GameObject.Find("Editor (1)").GetComponent<PlantEditor>();
        seed = editor.seed;
        details = GetComponent<PlantDetails>();
        if (generator == null)
            generator = GameObject.Find("GameManager").GetComponent<TreeGenerator>();
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        details.gender = Random.Range(0, 2);
        if (Physics.Raycast(transform.position + new Vector3(0, 1000, 0), -Vector3.up, out hit, Mathf.Infinity, 128))
        {
            transform.position = hit.point - new Vector3(0,0.1f,0);
            //Debug.Log(hit.transform.gameObject);
        }
        //SetDetails();
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
            energy += (((details.genes[0] + details.genes[1])) / ((colliders.Length + 1))) * Time.deltaTime/3f;
            energy -= ((details.genes[0] + details.genes[1]) / 750) * Time.timeScale;
            age += Time.timeScale / 100;
            tick = 10;
            
            transform.localScale += new Vector3(0.0015f * Time.timeScale, 0.0015f * Time.timeScale, 0.0015f * Time.timeScale);
            transform.localScale = new Vector3(Mathf.Clamp(transform.localScale.x, -1, 1), Mathf.Clamp(transform.localScale.y, -1, 1), Mathf.Clamp(transform.localScale.z, -1, 1));

        }
        if (transform.localScale.x < 0.1f)
        {
            Destroy(gameObject);
        }
        if (energy > 100 && age > 5)
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
                // Create sphere to collide using lookRadius
                //Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5, 8);

                //visual represetation/debugging
               // colliders = hitColliders;
                for (int i = 0; i < colliders.Length; i++)
                {
                    if(colliders[i] != null)
                    {
                        if (colliders[i].GetComponent<PlantBehavior>().details.gender != details.gender && colliders[i].GetComponent<PlantBehavior>().readyToMate == true && details.gender == 1)
                        {
                            if (details.genes[13] == 1 && colliders[i].GetComponent<PlantBehavior>().details.genes[13] == 1)//fruit
                            {
                                type = 0;
                            }
                            else if (details.genes[13] == 2 && colliders[i].GetComponent<PlantBehavior>().details.genes[13] == 2)//hard
                            {
                                type = 1;
                            }
                            else if (details.genes[13] == 3 && colliders[i].GetComponent<PlantBehavior>().details.genes[13] == 3)//flying
                            {
                                type = 2;
                            }
                            else if (details.genes[13] == 4 && colliders[i].GetComponent<PlantBehavior>().details.genes[13] == 4)//toxic
                            {
                                type = 3;
                            }
                            else if (details.genes[13] == 4 && colliders[i].GetComponent<PlantBehavior>().details.genes[13] == 1 || details.genes[13] == 1 && colliders[i].GetComponent<PlantBehavior>().details.genes[13] == 4)//toxic fruit
                            {
                                type = 4;
                            }
                            else
                            {
                                if (details.genes[13] == 1)
                                {
                                    type = 0;
                                }
                                if (details.genes[13] == 2)
                                {
                                    type = 1;

                                }
                                if (details.genes[13] == 3)
                                {
                                    type = 2;

                                }
                                if (details.genes[13] == 4)
                                {
                                    type = 3;
                                }
                            }
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
            if (details.genes[13] == 1)
            {
                type = 0;
            }
            if (details.genes[13] == 2)
            {
                type = 1;

            }
            if (details.genes[13] == 3)
            {
                type = 2;

            }
            if (details.genes[13] == 4)
            {
                type = 3;
            }
            if (readyToMate == true)
            {
                mate = gameObject;
                energy -= 100;
                //Debug.Log("repped");
                CreateSeeds();
                seeds++;
            }
        }
        if (details.genes[12] == 3)//neither
        {

        }
    }
    public void CreateSeeds()
    {
        if (seeds > 0)
        {
            if (type == 2)
            {
                range = 20;

                Vector3 random = new Vector3(Random.Range(transform.position.x - range, transform.position.x + range), 0, Random.Range(transform.position.z - range, transform.position.z + range));
                if (Physics.Raycast(random + new Vector3(0, 200, 0), -Vector3.up, out hit, Mathf.Infinity, 8))
                {
                    GameObject seedObj = Instantiate(seed, hit.point, Quaternion.identity);
                    seedObj.transform.position = random;
                    seedObj.GetComponent<Seed>().p2 = this.gameObject;
                    seedObj.GetComponent<Seed>().p1 = mate;
                    seedObj.GetComponent<Seed>().type = type;
                    seed.GetComponent<Seed>().dormantTime = 1000;
                    seeds--;
                }
            }
            if (type == 1)
            {
                range = 0;

                Vector3 random = new Vector3(Random.Range(transform.position.x - range, transform.position.x + range), 0, Random.Range(transform.position.z - range, transform.position.z + range));
                if (Physics.Raycast(random + new Vector3(0, 1000, 0), -Vector3.up, out hit, Mathf.Infinity, 8))
                {
                    GameObject seedObj = Instantiate(seed, hit.point, Quaternion.identity);
                    seedObj.GetComponent<Seed>().p2 = this.gameObject;
                    seedObj.GetComponent<Seed>().p1 = mate;
                    seedObj.GetComponent<Seed>().type = type;
                    seed.GetComponent<Seed>().dormantTime = 1000;
                    seeds--;
                }

            }
        }
    }
    public void Fruit(GameObject creature, float digestionDuration)
    {
        if (seeds > 0)
        {
            if (type == 4)
            {
                int rand = Random.Range(0, 100);
                if (rand >= 50)
                {
                    Destroy(creature);
                }
                else
                {
                    GameObject seedObj = Instantiate(seed, creature.transform.position, Quaternion.identity);
                    seedObj.transform.parent = creature.transform;
                    seedObj.GetComponent<SphereCollider>().enabled = false;
                    seedObj.GetComponent<MeshRenderer>().enabled = false;
                    seedObj.GetComponent<Rigidbody>().isKinematic = true;
                    seedObj.GetComponent<Seed>().p2 = this.gameObject;
                    seedObj.GetComponent<Seed>().p1 = mate;
                    seedObj.GetComponent<Seed>().type = type;
                    seedObj.GetComponent<Seed>().dd = digestionDuration;
                    seed.GetComponent<Seed>().dormantTime = 100;
                    seeds--;
                }
            }
            if (type == 0)
            {
                GameObject seedObj = Instantiate(seed, creature.transform.position, Quaternion.identity);
                seedObj.transform.parent = creature.transform;
                seedObj.GetComponent<SphereCollider>().enabled = false;
                seedObj.GetComponent<Rigidbody>().isKinematic = true;
                //seedObj.GetComponent<MeshRenderer>().enabled = false;
                seedObj.GetComponent<Seed>().p2 = this.gameObject;
                seedObj.GetComponent<Seed>().p1 = mate;
                seedObj.GetComponent<Seed>().type = type;
                seedObj.GetComponent<Seed>().dd = digestionDuration;
                seed.GetComponent<Seed>().dormantTime = 100;
                seeds--;
                Debug.Log("Created Seed");
            }
        }
    }
    void SetDetails()
    {
        generator.seed = details.genes[4];
        generator._recursionLevel = (int)details.genes[5];
        generator._trunkThickness = details.genes[6];
        generator._floorHeight = details.genes[7];
        generator._firstBranchHeight = details.genes[8];
        generator._twistiness = details.genes[9];
        generator._branchDensity = details.genes[10];
        generator._leavesSize = details.genes[11];
        generator.gen();
    }
    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 1 * transform.localScale.z * details.genes[6] * 2);
    }
}
