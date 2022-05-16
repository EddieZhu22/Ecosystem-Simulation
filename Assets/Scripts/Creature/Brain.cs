using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*


1 ANIMAL - 8 GENES
2 Speed
3 Look Radius
4 Size
5 Attraction 
6 Kill
7 Color
8 TBD


*/

[RequireComponent(typeof(Rigidbody))]
public class Brain : MonoBehaviour
{
    //Overall
    public float mutationStrength, mutationChance;
    public LayerMask raycastMask, raycastMask2;//Mask for the sensors
    private float[] input = new float[5];//input to the neural network
    //By Creature

    public float age;
    public float maxSpeed;

    public float maxStorage, food, water, lookRad;

    public float digestionDuration = 1000, t, refractoryPeriod;

    public bool done, done2, readyToMate;
    private float tick;
    public float action, attack, def, maxDef;
    private int foodCollected;
    private Rigidbody rb;
    private Vector3 randomDir;
    //Objects
    private GameObject Settings, foodObj;
    public GameObject prefab, head;
    public Transform target, parent1, parent2, mate;
    public NeuralNetwork network;
    public Collider[] colliders, colliders2, collider3;
    public float[] distanceColl, allele1, allele2, allele3, allele4, finalAllele, finalChromosome1, finalChromosome2, finalChromosome;

    public GameObject Offset;
    [HideInInspector] public float num;
    public TimeManager time;
    public CreatureDetails stats;


    private Vector3 toTarget;
    private void Start()
    {
        maxStorage = stats.genes[6] * stats.genes[8];
        stats = GetComponent<CreatureDetails>();
        if (stats.genes[16] == 0)
        {
            gameObject.tag = "prey";
        }
        if (stats.genes[16] == 1)
        {
            gameObject.tag = "predator";
        }
        if (time == null)
            time = GameObject.Find("GameManager").GetComponent<TimeManager>();
        if (time.year == 0)
        {
            SetGenes();
        }
        Restart();
        if (parent1 != null)
        {
            finalAllele = new float[parent1.GetComponent<CreatureDetails>().genes.Length / 2];
        }
        digestionDuration = 100;
    }
    public void FindFood()
    {
        if(stats.genes[16] == 0)
        {
            if (action == 0 || action == 1)
            {
                // Create sphere to collide using lookRadius
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, lookRad, 8);

                //visual represetation/debugging
                //colliders = hitColliders;

                // set min distance and distanceCollider
                distanceColl = new float[hitColliders.Length];
                float minDis = Mathf.Infinity;

                //find distance of each element in array
                for (int i = 0; i < hitColliders.Length; i++)
                {
                    distanceColl[i] = Vector3.Distance(transform.position, hitColliders[i].transform.position);
                }

                // find min distance of array
                minDis = Mathf.Min(distanceColl);

                // iterate through new array to see if the min distance is equal to find the index, and since both hitcolliders and distance colliders index are equal, you can just replace.
                for (int i = 0; i < hitColliders.Length; i++)
                {
                    if (minDis == distanceColl[i])
                    {
                        target = hitColliders[i].transform;
                    }
                }
            }
        }
        if(stats.genes[16] == 1)
        {
            if (action == 0 || action == 1)
            {
                // Create sphere to collide using lookRadius
                Collider[] hitColliders2 = Physics.OverlapSphere(transform.position, lookRad, raycastMask2);
                List<Collider> collList = new List<Collider>(hitColliders2);
                collList.RemoveAll(x => x.CompareTag("predator"));
                hitColliders2 = collList.ToArray();
                //visual represetation/debugging
                collider3 = hitColliders2;

                // set min distance and distanceCollider
                distanceColl = new float[hitColliders2.Length];
                float minDis = Mathf.Infinity;

                //find distance of each element in array
                for (int i = 0; i < hitColliders2.Length; i++)
                {
                    distanceColl[i] = Vector3.Distance(transform.position, hitColliders2[i].transform.position);
                }

                // find min distance of array
                minDis = Mathf.Min(distanceColl);

                // iterate through new array to see if the min distance is equal to find the index, and since both hitcolliders and distance colliders index are equal, you can just replace.
                for (int i = 0; i < hitColliders2.Length; i++)
                {
                    if (minDis == distanceColl[i])
                    {
                        target = hitColliders2[i].transform;
                    }
                }
            }
        }
    }

    public void EatFood()
    {
        if(stats.genes[16] == 0)
        {
            if (target != null && action != 2)
            {
                if (Vector3.Distance(transform.position, target.position) > 1.5)
                {
                    toTarget = target.transform.position - transform.position;
                    transform.LookAt(target.transform.position);
                    Vector3 angle = transform.localEulerAngles;
                    transform.localEulerAngles = new Vector3(0, angle.y + 90, 0);
                    //rb.AddRelativeForce(0, 0, stats.genes[3] * 200);
                    transform.Translate(toTarget * stats.genes[3] / 25f * Time.deltaTime, Space.World);
                    food -= stats.genes[0] / 80 * Time.deltaTime;
                    water -= stats.genes[0] / 1000 * Time.deltaTime;
                }
                else if (Vector3.Distance(transform.position, target.position) <= 1.5)
                {
                    if (food < stats.genes[8])
                    {
                        food += 0.045f * Time.timeScale;
                        target.transform.localScale -= new Vector3(0.0035f * Time.timeScale, 0.0035f * Time.timeScale, 0.0035f * Time.timeScale);
                        if (t < 10)
                        {
                            target.GetComponent<PlantBehavior>().Fruit(gameObject, digestionDuration);
                            t++;
                        }
                    }

                }

            }
            else if (target == null && action != 2)
            {
                Wander();
                //Log("Wander_Success");
            }
        }
        if (stats.genes[16] == 1)
        {
            if (target != null && action != 2)
            {
                if (Vector3.Distance(transform.position, target.position) > 0.25f)
                {
                    toTarget = target.transform.position - transform.position;
                    transform.LookAt(target.transform.position);
                    Vector3 angle = transform.localEulerAngles;
                    transform.localEulerAngles = new Vector3(0, angle.y + 90, 0);
                    //rb.AddRelativeForce(0, 0, stats.genes[3] * 200);
                    transform.Translate(toTarget * stats.genes[3] / 10f * Time.deltaTime, Space.World);
                    food -= stats.genes[0] / 80 * Time.deltaTime;
                    water -= stats.genes[0] / 1000 * Time.deltaTime;
                }
                if (Vector3.Distance(transform.position, target.position) <= 1.5)
                {
                    if (food < stats.genes[8])
                    {
                        if(stats.genes[17] == 0)
                        {
                            Debug.Log("fighting");
                            target.GetComponent<Brain>().def -= attack * Time.timeScale;
                            target.localScale -= new Vector3(attack / target.GetComponent<Brain>().maxDef, attack / target.GetComponent<Brain>().maxDef, attack/ target.GetComponent<Brain>().maxDef);
                            if(target.GetComponent<Brain>().def < 0)
                            {
                                Debug.Log("dead!");
                                Destroy(target.root.gameObject);
                                food += stats.genes[8];
                            }
                            food -= stats.genes[0] / 80 * Time.deltaTime;
                            water -= stats.genes[0] / 1000 * Time.deltaTime;
                        }
                    }

                }

            }
            else if (target == null && action != 2)
            {
                Wander();
                //Log("Wander_Success");
            }
        }
    }
    private void Wander()
    {
        food -= stats.genes[0] / 80 * Time.deltaTime;
        water -= stats.genes[0] / 10000 * Time.deltaTime;
        tick -= Time.deltaTime;
        if (tick < 0)
        {
            randomDir = new Vector3(0, Random.Range(60, -60), 0) + transform.eulerAngles;
            tick = 1;
        }
        transform.eulerAngles = new Vector3(0, Mathf.LerpAngle(transform.eulerAngles.y, randomDir.y, (stats.genes[3]) * Time.deltaTime), 0);
        rb.AddRelativeForce(-stats.genes[3] * Time.timeScale * 80, 0, 0);
    }
    public void Movement()
    {
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
        }
        //rb.AddForce(0, -1000, 0);
    }
    public void Run()
    {
        if (stats.genes[16] == 0)
        {
            // Create sphere to collide using lookRadius
            Collider[] hitColliders2 = Physics.OverlapSphere(transform.position, lookRad, raycastMask2);
            List<Collider> collList = new List<Collider>(hitColliders2);
            collList.RemoveAll(x => x.CompareTag("prey"));
            hitColliders2 = collList.ToArray();
            if(hitColliders2.Length > 0)
            {
                action = 3;
                food -= stats.genes[0] / 80 * Time.deltaTime;
                water -= stats.genes[0] / 10000 * Time.deltaTime;
                tick -= Time.deltaTime;
                if (tick < 0)
                {
                    randomDir = -hitColliders2[0].transform.eulerAngles;
                    tick = 1;
                }
                transform.eulerAngles = new Vector3(0, Mathf.LerpAngle(transform.eulerAngles.y, randomDir.y, (stats.genes[3]) * Time.deltaTime), 0);
                rb.AddRelativeForce(-stats.genes[3] * Time.timeScale * 80, 0, 0);
            }
            else
            {
                action = 0;
            }

        }
    }
    public void Reproduce()
    {
        if (stats.gender == 1)
        {
            for (int k = 0; k < stats.genes[5]; k++)
            {
                //food -= 1;
                finalChromosome = new float[stats.genes.Length];
                for (int i = 0; i < finalChromosome.Length; i++)
                {
                    if (i >= 3 && i < 10)
                    {
                        finalChromosome[i] = (stats.genes[i] + mate.GetComponent<Brain>().stats.genes[i]) / 2;
                        float rand = Random.Range(0, 1);
                        if (rand < mutationChance)
                        {
                            float strength = Random.Range(-mutationStrength * stats.genes[i], mutationStrength * stats.genes[i]);
                            finalChromosome[i] = finalChromosome[i] + strength;
                        }
                    }
                    else
                    {
                        if ((int)Random.Range(0, 1) == 0)
                        {
                            finalChromosome[i] = stats.genes[i];
                        }
                        if ((int)Random.Range(0, 1) == 1)
                        {
                            finalChromosome[i] = mate.GetComponent<Brain>().stats.genes[i];
                        }
                    }
                }
            }
            GameObject Creature = Instantiate(prefab, transform.position, Quaternion.identity);
            Creature.GetComponent<CreatureBehavior>().stats.genes = finalChromosome;
            Creature.GetComponent<CreatureBehavior>().editor = transform.parent.GetComponent<CreatureBehavior>().editor;
            Creature.GetComponent<CreatureBehavior>().time = transform.parent.GetComponent<CreatureBehavior>().time;
        }
        refractoryPeriod = 0;
        done2 = true;
        readyToMate = false;
        action = 0;
    }

    public void FindMate()
    {
        if (done2 == false)
        {
            readyToMate = true;
            // Create sphere to collide using lookRadius
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, lookRad, raycastMask2);

            colliders2 = hitColliders;

            //find distance of each element in array
            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (done == false)
                {
                    Communicate(hitColliders[i].gameObject);
                }
            }
            if (mate == null)
            {
                Wander();
            }

            /*if (food < (1 / (stats.genes[9] * 2)) * (maxStorage / 2))
            {
                readyToMate = false;
                Wander();
            }*/
            if (done == true)
            {
                if (mate != null)
                {
                    if (Vector3.Distance(transform.position, mate.position) <= lookRad)
                    {
                        toTarget = mate.transform.position - transform.position;
                        transform.LookAt(mate.transform.position);
                        Vector3 angle = transform.localEulerAngles;
                        transform.localEulerAngles = new Vector3(0, angle.y + 90, 0);
                        //rb.AddRelativeForce(0, 0, stats.genes[3] * 200);
                        transform.Translate(toTarget * stats.genes[3] / 10 * Time.deltaTime, Space.World);
                        food -= stats.genes[0] / 80 * Time.deltaTime;
                        water -= stats.genes[0] / 10000 * Time.deltaTime;
                        if (Vector3.Distance(transform.position, mate.position) < 1)
                        {
                            Reproduce();
                        }
                    }
                    else
                    {
                        mate = null;
                    }
                }
                else
                {
                    mate = null;
                    action = 0;
                    Wander();
                }
            }

        }
        if (done2 == true)
        {
            action = 0;
        }
    }
    public void Communicate(GameObject receiver)
    {
        if (receiver.GetComponent<Brain>().readyToMate == true && receiver.GetComponent<CreatureDetails>().gender != stats.gender && receiver.GetComponent<Brain>().done2 == false && GetComponent<Brain>().done2 == false && receiver.tag == gameObject.tag)
        {
            mate = receiver.transform;
            receiver.GetComponent<Brain>().mate = transform;
            done = true;
        }
    }
    public void Meiosis()
    {
        //set chromosomes equal to parents
        finalChromosome1 = parent1.gameObject.GetComponent<CreatureDetails>().genes;
        finalChromosome2 = parent2.gameObject.GetComponent<CreatureDetails>().genes;
        allele1 = new float[finalChromosome1.Length / 2];
        allele2 = new float[(finalChromosome1.Length) / 2];
        allele3 = new float[finalChromosome1.Length / 2];
        allele4 = new float[(finalChromosome1.Length) / 2];
        for (int i = 3; i < stats.genes.Length; i++)
        {
            //homologous chromosomes cross over
            float gene1 = parent1.gameObject.GetComponent<CreatureDetails>().genes[i];
            float gene2 = parent2.gameObject.GetComponent<CreatureDetails>().genes[i];
            float rand = Random.Range(0, 1);
            if (rand <= mutationChance)
            {
                finalChromosome1[i] = gene2;
                finalChromosome2[i] = gene1;
            }
        }
        //Meiosis II
        for (int i = 0; i < stats.genes.Length; i++)
        {
            if (i < stats.genes.Length / 2)
            {
                allele1[i] = finalChromosome1[i];
                allele3[i] = finalChromosome2[i];
            }
            if (i >= stats.genes.Length / 2)
            {
                allele2[i - stats.genes.Length / 2] = finalChromosome1[i];
                allele4[i - stats.genes.Length / 2] = finalChromosome2[i];
            }
        }
        float alleleTemp = (int)Random.Range(1, 5);
        if (alleleTemp == 1)
        {
            finalAllele = allele1;
            num = 1;
        }
        else if (alleleTemp == 2)
        {
            finalAllele = allele2;
            num = 2;
        }
        else if (alleleTemp == 3)
        {
            finalAllele = allele3;
            num = 3;
        }
        else if (alleleTemp == 4)
        {
            finalAllele = allele4;
            num = 4;
        }
    }
    public void Die()
    {
        Destroy(transform.parent.gameObject);
    }
    public void SetGenes()
    {
        stats.genes[5] = Random.Range(0, 5);
        stats.genes[4] = Random.Range(0, 25);
        stats.genes[3] = Random.Range(0, 200);
    }
    public void Restart()
    {
        //energy = 1;
        age = 0;
        rb = GetComponent<Rigidbody>();
        Settings = GameObject.Find("GameManager");
        mutationStrength = Settings.GetComponent<GameManager>().MutationStrength;
        mutationChance = Settings.GetComponent<GameManager>().MutationChance;
        //finalChromosome = stats.genes;
        /*for (int i = 0; i < finalChromosome.Length; i++)
        {
            if (i >= 3 && i < 10)
            {
                finalChromosome[i] = stats.genes[i];
                float rand = Random.Range(0, 1);
                if (rand < mutationChance)
                {
                    float strength = Random.Range(-mutationStrength * stats.genes[i], mutationStrength * stats.genes[i]);
                    finalChromosome[i] = finalChromosome[i] + strength;
                }
            }
        }
        stats.genes = finalChromosome;
        */
    }

    /*
    ███╗░░██╗███████╗██╗░░░██╗██████╗░░█████╗░██╗░░░░░  ███╗░░██╗███████╗████████╗
    ████╗░██║██╔════╝██║░░░██║██╔══██╗██╔══██╗██║░░░░░  ████╗░██║██╔════╝╚══██╔══╝
    ██╔██╗██║█████╗░░██║░░░██║██████╔╝███████║██║░░░░░  ██╔██╗██║█████╗░░░░░██║░░░
    ██║╚████║██╔══╝░░██║░░░██║██╔══██╗██╔══██║██║░░░░░  ██║╚████║██╔══╝░░░░░██║░░░
    ██║░╚███║███████╗╚██████╔╝██║░░██║██║░░██║███████╗  ██║░╚███║███████╗░░░██║░░░
    ╚═╝░░╚══╝╚══════╝░╚═════╝░╚═╝░░╚═╝╚═╝░░╚═╝╚══════╝  ╚═╝░░╚══╝╚══════╝░░░╚═╝░░░
    */

    public void NNFindFood()
    {

    }
    public void NNEatFood()
    {
        //if (target != null)
        //{
        for (int i = 0; i < 5; i++)//draws five debug rays as inputs
        {
            Vector3 newVector = Quaternion.AngleAxis(i * 45 - 90, new Vector3(0, 1, 0)) * transform.right;//calculating angle of raycast
            RaycastHit hit;
            Ray Ray = new Ray(transform.position, newVector);

            if (Physics.Raycast(Ray, out hit, stats.genes[4], raycastMask))
            {
                input[i] = (stats.genes[4] - hit.distance) / stats.genes[4];//return distance, 1 being close
            }
            else
            {
                input[i] = 0;//if nothing is detected, will return 0 to network
            }
        }
        //energy -= 0.05f * Time.deltaTime * 10;
        float[] output = network.FeedForward(input);

        //transform.Rotate(0, output[0] * stats.rotationSpeed * Time.deltaTime * 100, 0, Space.World);//Call to network to feedforward
        //stats.color = output[0];

        //rb.AddRelativeForce(0, 0, stats.speed * Time.deltaTime * 100);
        rb.AddForce(0, -100, 0);
        //Debug.Log(output[0]);
    }
    public void NNUpdateFitness()
    {
        network.fitness = foodCollected;//updates fitness of network for sorting
    }
    public void NNReproduce()
    {
        for (int i = 0; i < 3; i++)
        {
            //subtract energy
            if (food <= 0 || water <= 0)
                Die();
            food--;
            //normalize
            rb.velocity = Vector3.zero;
            //create baby!
            Vector3 offset = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
            GameObject creature = (Instantiate(prefab, transform.position + offset, Quaternion.identity));//create bots
            creature.GetComponent<Brain>().network = network;
            creature.GetComponent<Brain>().network.Mutate((int)(1 / Settings.GetComponent<GameManager>().MutationChance), Settings.GetComponent<GameManager>().MutationStrength);
        }
        age++;
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.tag == "death")
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
    /*  private void OnCollisionStay(Collision other)
      {
          if (other.collider.tag == "food")
          {
              //foodCollected++;
              if (food < (1 / (stats.genes[9] * 2)) * (maxStorage / 2))
              {
                  food += 0.1f;
                  other.collider.transform.localScale -= new Vector3(0.001f, 0.001f, 0.001f);
              }
          }
      }*/
}
