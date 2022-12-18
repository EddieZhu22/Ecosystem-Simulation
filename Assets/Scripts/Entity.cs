using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public bool alive;
    public float age;
    public int gender(int min, int max) => Random.Range(min,max);
    public void Die(GameObject gameObject) => Destroy(gameObject);
}
