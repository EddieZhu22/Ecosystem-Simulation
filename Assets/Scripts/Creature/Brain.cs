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
    /*
    
    ███╗░░██╗███████╗██╗░░░██╗██████╗░░█████╗░██╗░░░░░  ███╗░░██╗███████╗████████╗
    ████╗░██║██╔════╝██║░░░██║██╔══██╗██╔══██╗██║░░░░░  ████╗░██║██╔════╝╚══██╔══╝
    ██╔██╗██║█████╗░░██║░░░██║██████╔╝███████║██║░░░░░  ██╔██╗██║█████╗░░░░░██║░░░
    ██║╚████║██╔══╝░░██║░░░██║██╔══██╗██╔══██║██║░░░░░  ██║╚████║██╔══╝░░░░░██║░░░
    ██║░╚███║███████╗╚██████╔╝██║░░██║██║░░██║███████╗  ██║░╚███║███████╗░░░██║░░░
    ╚═╝░░╚══╝╚══════╝░╚═════╝░╚═╝░░╚═╝╚═╝░░╚═╝╚══════╝  ╚═╝░░╚══╝╚══════╝░░░╚═╝░░░
    

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
