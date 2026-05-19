using UnityEngine;
using UnityEngine.SceneManagement;

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
    public int sideObstacleGap = 0;
    public int sideObstaclesBeforeSwitch = 2;

    private bool stopSpawning = false;
    private float currentSpawnTime;
    private float totalDistance;
    private int gapCount = 0;
    private int currentSide = -1;
    private int sideRunCount = 0;

    void Start()
    {
        currentSpawnTime = spawnTimeStart;
        currentSide = Random.Range(0, 2);
        if (endZone != null)
            totalDistance = endZone.position.z - player.position.z;
        InvokeRepeating(nameof(Spawn), startDelay, currentSpawnTime);
    }

    void Update()
    {
        if (stopSpawning)
        {
            if (endZone != null)
            {
                float distZ = Mathf.Abs(player.position.z - endZone.position.z);
                if (distZ <= 2f)
                {
                    SceneManager.LoadScene("Level2");
                }
            }
            return;
        }

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
        
        if (progress < 0.3f)
        {
            bottomChance = 0.1f;
        }
        else if (progress < 0.5f)
        {
            bottomChance = 0.18f;
        }
        else if (progress < 0.7f)
        {
            bottomChance = 0.25f;
        }
        else
        {
            bottomChance = 0.32f;
        }

        GameObject selectedPrefab;
        int sideGap = Mathf.Max(0, sideObstacleGap);
        int maxSideRun = Mathf.Max(1, sideObstaclesBeforeSwitch);

        if (gapCount > 0)
        {
            selectedPrefab = bottomPrefab;
            gapCount--;

            if (gapCount == 0)
            {
                currentSide = 1 - currentSide;
                sideRunCount = 0;
            }
        }
        else
        {
            float rand = Random.Range(0f, 1f);

            if (sideRunCount >= maxSideRun)
            {
                selectedPrefab = bottomPrefab;
                currentSide = 1 - currentSide;
                sideRunCount = 0;
            }
            else if (sideRunCount > 0 && rand < bottomChance)
            {
                selectedPrefab = bottomPrefab;
                currentSide = 1 - currentSide;
                sideRunCount = 0;
            }
            else
            {
                if (currentSide == 0)
                {
                    selectedPrefab = leftPrefab;
                }
                else
                {
                    selectedPrefab = rightPrefab;
                }

                sideRunCount++;
                gapCount = sideGap;
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
