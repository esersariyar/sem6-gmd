using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform player;
    public Transform endZone;

    public GameObject leftPrefab;
    public GameObject rightPrefab;
    public GameObject bottomPrefab;

    public float spawnDistance = 40f;
    public float spawnTimeStart = 2f;
    public float spawnTimeEnd = 1.2f;
    public float startDelay = 3f;
    public float obstacleLifetime = 60f;

    private bool stopSpawning = false;
    private float currentSpawnTime;
    private float totalDistance;
    private int lastSide = -1;
    private int gapCount = 0;

    void Start()
    {
        currentSpawnTime = spawnTimeStart;
        if (endZone != null)
            totalDistance = endZone.position.z - player.position.z;
        InvokeRepeating(nameof(Spawn), startDelay, currentSpawnTime);
    }

    void Update()
    {
        if (stopSpawning) return;

        if (endZone != null)
        {
            float spawnZ = player.position.z + spawnDistance;
            
            if (spawnZ >= endZone.position.z - 5f)
            {
                StopSpawning();
                return;
            }

            float distanceToEnd = endZone.position.z - player.position.z;
            float progress = 1f - (distanceToEnd / totalDistance);
            progress = Mathf.Clamp01(progress);

            float newSpawnTime = Mathf.Lerp(spawnTimeStart, spawnTimeEnd, progress);

            if (Mathf.Abs(newSpawnTime - currentSpawnTime) > 0.05f)
            {
                currentSpawnTime = newSpawnTime;
                CancelInvoke(nameof(Spawn));
                InvokeRepeating(nameof(Spawn), 0.1f, currentSpawnTime);
            }
        }

        transform.position = new Vector3(0f, 0f, player.position.z + spawnDistance);
    }

    public void StopSpawning()
    {
        if (!stopSpawning)
        {
            stopSpawning = true;
            CancelInvoke(nameof(Spawn));
        }
    }

    void Spawn()
    {
        if (stopSpawning) return;

        float progress = 0f;
        if (endZone != null)
        {
            float distanceToEnd = endZone.position.z - player.position.z;
            progress = 1f - (distanceToEnd / totalDistance);
            progress = Mathf.Clamp01(progress);
        }

        float bottomChance;
        int requiredGap;
        
        if (progress < 0.3f)
        {
            bottomChance = 0.15f;
            requiredGap = 1;
        }
        else if (progress < 0.5f)
        {
            bottomChance = 0.25f;
            requiredGap = 1;
        }
        else if (progress < 0.7f)
        {
            bottomChance = 0.4f;
            requiredGap = 2;
        }
        else
        {
            bottomChance = 0.5f;
            requiredGap = 2;
        }

        GameObject selectedPrefab;

        if (gapCount > 0)
        {
            selectedPrefab = bottomPrefab;
            gapCount--;
        }
        else
        {
            float rand = Random.Range(0f, 1f);

            if (rand < bottomChance)
            {
                selectedPrefab = bottomPrefab;
            }
            else
            {
                if (lastSide == 0)
                {
                    selectedPrefab = rightPrefab;
                    lastSide = 1;
                    gapCount = requiredGap;
                }
                else if (lastSide == 1)
                {
                    selectedPrefab = leftPrefab;
                    lastSide = 0;
                    gapCount = requiredGap;
                }
                else
                {
                    int side = Random.Range(0, 2);
                    selectedPrefab = (side == 0) ? leftPrefab : rightPrefab;
                    lastSide = side;
                }
            }
        }

        if (selectedPrefab == null)
        {
            Debug.LogError("Selected prefab is NULL!");
            return;
        }

        GameObject spawned = Instantiate(selectedPrefab, transform.position, Quaternion.identity);
        Destroy(spawned, obstacleLifetime);
    }
}