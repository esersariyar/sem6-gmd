using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    public FemaleRobotFlee femaleRobot;
    public GameObject winPanel;
    public bool pauseGameOnWin = true;
    public bool requireArrived = true;

    private bool won;

    void Awake()
    {
        if (femaleRobot == null)
        {
            femaleRobot = GetComponent<FemaleRobotFlee>();
        }

        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        TryWin(collision.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        TryWin(other.gameObject);
    }

    void OnCollisionStay(Collision collision)
    {
        TryWin(collision.gameObject);
    }

    void OnTriggerStay(Collider other)
    {
        TryWin(other.gameObject);
    }

    void TryWin(GameObject hitObject)
    {
        if (won || !hitObject.CompareTag("Player"))
        {
            return;
        }

        if (requireArrived && (femaleRobot == null || !femaleRobot.HasArrived))
        {
            return;
        }

        won = true;

        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }

        if (pauseGameOnWin)
        {
            Time.timeScale = 0f;
        }
    }
}
