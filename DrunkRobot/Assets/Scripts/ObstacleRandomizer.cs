using UnityEngine;

public class ObstacleRandomizer : MonoBehaviour
{
    public GameObject blockLeft;
    public GameObject blockRight;
    public GameObject blockBottom;

    void Awake()
    {
        if (blockLeft == null || blockRight == null || blockBottom == null)
            return;

        blockLeft.SetActive(false);
        blockRight.SetActive(false);
        blockBottom.SetActive(false);

        int random = Random.Range(0, 3);

        if (random == 0)
            blockLeft.SetActive(true);
        else if (random == 1)
            blockRight.SetActive(true);
        else
            blockBottom.SetActive(true);
    }
}