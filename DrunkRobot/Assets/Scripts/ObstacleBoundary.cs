using UnityEngine;

public class ObstacleBoundary : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger: {other.transform.root.gameObject.name}");
        TryDestroy(other.transform.root.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Collision: {collision.transform.root.gameObject.name}");
        TryDestroy(collision.transform.root.gameObject);
    }

    private void TryDestroy(GameObject target)
    {
        string name = target.name;
        
        if (name.Contains("RightPrefab") ||
            name.Contains("LeftPrefab") ||
            name.Contains("BottomPrefab"))
        {
            Debug.Log($"Destroying: {name}");
            Destroy(target);
        }
    }
}
