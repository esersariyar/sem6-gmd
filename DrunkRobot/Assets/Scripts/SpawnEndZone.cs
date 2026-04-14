using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnEndZone : MonoBehaviour
{
    public float triggerDistance = 5f;
    private bool triggered = false;
    private Transform player;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            player = p.transform;
            Debug.Log("ENDZONE: Player found: " + p.name);
        }
        else
        {
            Debug.LogError("ENDZONE: No object with Player tag found!");
        }

        Debug.Log("ENDZONE: Scenes in build = " + SceneManager.sceneCountInBuildSettings);
    }

    void Update()
    {
        if (triggered) return;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
            else return;
        }

        float dist = Mathf.Abs(player.position.z - transform.position.z);
        Debug.Log("ENDZONE dist=" + dist);

        if (dist <= triggerDistance)
        {
            triggered = true;
            Debug.Log("ENDZONE: LOADING LEVEL2 NOW");
            SceneManager.LoadScene(2);
        }
    }
}
