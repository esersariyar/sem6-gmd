using UnityEngine;

public class TurretProjectile : MonoBehaviour
{
    public float lifeTime = 5f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        TryRespawn(other.gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        TryRespawn(collision.gameObject);
    }

    void TryRespawn(GameObject hitObject)
    {
        if (!hitObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            return;
        }

        PlayerRespawn respawn = hitObject.GetComponent<PlayerRespawn>();
        if (respawn == null)
        {
            respawn = hitObject.GetComponentInParent<PlayerRespawn>();
        }

        if (respawn != null)
        {
            respawn.Respawn();
        }

        Destroy(gameObject);
    }
}
