using UnityEngine;

public class RedObstacle : MonoBehaviour
{
    private MouseLook mouseLook;

    void Start()
    {
        mouseLook = FindFirstObjectByType<MouseLook>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            TriggerEffect();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TriggerEffect();
        }
    }

    private void TriggerEffect()
    {
        if (mouseLook != null)
        {
            mouseLook.BoostDrunk();
        }
    }
}
