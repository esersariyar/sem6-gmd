using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public Transform spawnPoint;
    public static event System.Action Respawned;

    private Rigidbody rb;
    private Vector3 startPosition;
    private Quaternion startRotation;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    public void Respawn()
    {
        Vector3 targetPosition = spawnPoint != null ? spawnPoint.position : startPosition;
        Quaternion targetRotation = spawnPoint != null ? spawnPoint.rotation : startRotation;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.position = targetPosition;
            rb.rotation = targetRotation;
        }
        else
        {
            transform.SetPositionAndRotation(targetPosition, targetRotation);
        }

        Respawned?.Invoke();
    }
}
