using UnityEngine;

public class SpawnEndZone : MonoBehaviour
{
    public Spawner spawner;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            spawner.StopSpawning();
        }
    }
}
