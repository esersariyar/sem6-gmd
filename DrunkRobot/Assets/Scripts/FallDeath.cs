using UnityEngine;

[RequireComponent(typeof(PlayerRespawn))]
public class FallDeath : MonoBehaviour
{
    public float deathHeight = -10f;

    private PlayerRespawn playerRespawn;

    void Awake()
    {
        playerRespawn = GetComponent<PlayerRespawn>();
    }

    void Update()
    {
        if (transform.position.y < deathHeight)
        {
            playerRespawn.Respawn();
        }
    }
}
