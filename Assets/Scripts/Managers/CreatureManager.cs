using UnityEngine;

public class CreatureManager : MonoBehaviour
{
    public CreatureData creatureData = new CreatureData();

    private void Start()
    {
        InvokeRepeating("UpdateCreatureCount", 1.0f, 1.0f);
    }

    void UpdateCreatureCount()
    {
        // Assuming you have a way to count the creatures
        int currentCount = CountAliveCreatures();
        currentCount = Random.Range(0, 100);
        creatureData.creatureCountPerSecond.Add(currentCount);
    }

    int CountAliveCreatures()
    {
        // Your logic to count creatures goes here
        return 0; // Example
    }
}

