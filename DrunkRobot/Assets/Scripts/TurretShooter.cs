using UnityEngine;

public class TurretShooter : MonoBehaviour
{
    public Transform target;
    public Transform firePoint;
    public GameObject projectilePrefab;
    public float detectionRange = 20f;
    public float fireInterval = 1f;
    public float projectileSpeed = 14f;
    public float turnSpeed = 8f;
    public float projectileScale = 0.45f;
    public float aimYawOffset = 0f;
    public Color projectileColor = new Color(1f, 0.15f, 0f, 1f);
    public AudioClip shootSound;
    public float shootVolume = 1f;
    public LayerMask lineOfSightMask = ~0;

    private float fireTimer;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;

        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    void Update()
    {
        if (target == null)
        {
            return;
        }

        Vector3 targetCenter = target.position + Vector3.up;
        Vector3 origin = firePoint != null ? firePoint.position : transform.position + Vector3.up * 0.7f;
        Vector3 toTarget = targetCenter - origin;

        if (toTarget.magnitude > detectionRange || !CanSeeTarget(origin, toTarget))
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(toTarget.normalized, Vector3.up) * Quaternion.Euler(0f, aimYawOffset, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f)
        {
            Shoot(origin, toTarget.normalized);
            fireTimer = fireInterval;
        }
    }

    bool CanSeeTarget(Vector3 origin, Vector3 toTarget)
    {
        if (Physics.Raycast(origin, toTarget.normalized, out RaycastHit hit, detectionRange, lineOfSightMask, QueryTriggerInteraction.Ignore))
        {
            return hit.collider.CompareTag("Player") || hit.collider.GetComponentInParent<PlayerMovement>() != null;
        }

        return false;
    }

    void Shoot(Vector3 origin, Vector3 direction)
    {
        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound, shootVolume);
        }

        GameObject projectile = projectilePrefab != null ? Instantiate(projectilePrefab, origin, Quaternion.LookRotation(direction)) : GameObject.CreatePrimitive(PrimitiveType.Sphere);

        if (projectilePrefab == null)
        {
            projectile.transform.position = origin;
            projectile.transform.rotation = Quaternion.LookRotation(direction);
            projectile.transform.localScale = Vector3.one * projectileScale;
            Renderer renderer = projectile.GetComponent<Renderer>();
            if (renderer != null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit");
                if (shader == null)
                {
                    shader = Shader.Find("Standard");
                }

                Material material = new Material(shader);
                material.color = projectileColor;
                renderer.material = material;
            }
        }

        TurretProjectile turretProjectile = projectile.GetComponent<TurretProjectile>();
        if (turretProjectile == null)
        {
            turretProjectile = projectile.AddComponent<TurretProjectile>();
        }

        Collider projectileCollider = projectile.GetComponent<Collider>();
        if (projectileCollider != null)
        {
            projectileCollider.isTrigger = true;
        }

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = projectile.AddComponent<Rigidbody>();
        }

        rb.useGravity = false;
        rb.linearVelocity = direction * projectileSpeed;
    }
}
