using System.Collections;
using UnityEngine;

public class EnemyChaser : MonoBehaviour
{
    public static void BoostAll(float multiplier, float duration)
    {
        foreach (EnemyChaser enemy in FindObjectsByType<EnemyChaser>(FindObjectsSortMode.None))
        {
            enemy.BoostSpeed(multiplier, duration);
        }
    }

    public void BoostSpeed(float multiplier, float duration)
    {
        if (boostRoutine != null)
        {
            StopCoroutine(boostRoutine);
        }

        boostRoutine = StartCoroutine(BoostRoutine(multiplier, duration));
    }

    IEnumerator BoostRoutine(float multiplier, float duration)
    {
        speedMultiplier = multiplier;
        yield return new WaitForSeconds(duration);
        speedMultiplier = 1f;
        boostRoutine = null;
    }

    public Transform target;
    public float moveSpeed = 3f;
    public float detectionRange = 25f;
    public float stopDistance = 0.8f;
    public float turnSpeed = 8f;
    public float hitRadius = 0.7f;
    public float bodyHeight = 2f;
    public LayerMask obstacleMask = ~0;
    public bool lockYPosition = true;
    public Transform leftArm;
    public Transform rightArm;
    public float armSwingSpeed = 6f;
    public float armSwingAmount = 35f;
    public float armRaiseAngle = -90f;
    public bool keepArmsRaised = true;
    public bool useSimpleArmRotation = true;

    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    private Quaternion leftArmStartRotation;
    private Quaternion rightArmStartRotation;
    private Rigidbody rb;
    private bool isChasing;
    private float speedMultiplier = 1f;
    private Coroutine boostRoutine;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.useGravity = !lockYPosition;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        if (lockYPosition)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        }

        EnsureCollider();
        AutoFindArms();
        DisableConflictingAnimators();

        spawnPosition = transform.position;
        spawnRotation = transform.rotation;

        if (leftArm != null)
        {
            leftArmStartRotation = leftArm.localRotation;
        }

        if (rightArm != null)
        {
            rightArmStartRotation = rightArm.localRotation;
        }
    }

    void OnEnable()
    {
        PlayerRespawn.Respawned += ResetToSpawn;
    }

    void OnDisable()
    {
        PlayerRespawn.Respawned -= ResetToSpawn;
    }

    void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    void FixedUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0f;

        if (toTarget.magnitude > detectionRange)
        {
            isChasing = keepArmsRaised;
            return;
        }

        if (toTarget.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);
        }

        if (toTarget.magnitude > stopDistance)
        {
            Vector3 direction = toTarget.normalized;
            float moveDistance = moveSpeed * speedMultiplier * Time.fixedDeltaTime;

            if (CanMove(direction, moveDistance))
            {
                Vector3 nextPosition = rb.position + direction * moveDistance;
                if (lockYPosition)
                {
                    nextPosition.y = spawnPosition.y;
                }

                rb.MovePosition(nextPosition);
            }

            isChasing = true;
        }
        else
        {
            isChasing = true;
        }
    }

    void LateUpdate()
    {
        AnimateArms(isChasing);
    }

    void DisableConflictingAnimators()
    {
        foreach (Animator animator in GetComponentsInChildren<Animator>(true))
        {
            animator.enabled = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        TryHitPlayer(collision.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        TryHitPlayer(other.gameObject);
    }

    void TryHitPlayer(GameObject hitObject)
    {
        if (!hitObject.CompareTag("Player"))
        {
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
    }

    void ResetToSpawn()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.position = spawnPosition;
            rb.rotation = spawnRotation;
        }
        else
        {
            transform.SetPositionAndRotation(spawnPosition, spawnRotation);
        }

        ResetArms();
    }

    void EnsureCollider()
    {
        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        if (capsule == null)
        {
            capsule = gameObject.AddComponent<CapsuleCollider>();
        }

        capsule.radius = hitRadius;
        capsule.height = bodyHeight;
        capsule.center = new Vector3(0f, bodyHeight * 0.5f, 0f);
        capsule.isTrigger = false;
    }

    bool CanMove(Vector3 direction, float moveDistance)
    {
        Vector3 bottom = transform.position + Vector3.up * hitRadius;
        Vector3 top = transform.position + Vector3.up * (bodyHeight - hitRadius);
        float castDistance = moveDistance + 0.08f;

        if (Physics.CapsuleCast(bottom, top, hitRadius * 0.9f, direction, out RaycastHit hit, castDistance, obstacleMask, QueryTriggerInteraction.Ignore))
        {
            return hit.collider.CompareTag("Player") || hit.collider.transform.IsChildOf(transform);
        }

        return true;
    }

    void AutoFindArms()
    {
        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            string childName = child.name.ToLowerInvariant();

            if (leftArm == null && IsLeftArmName(childName))
            {
                leftArm = child;
            }

            if (rightArm == null && IsRightArmName(childName))
            {
                rightArm = child;
            }
        }
    }

    bool IsLeftArmName(string childName)
    {
        return childName.Contains("arm-left") || childName.Contains("leftarm") || childName.Contains("left-arm") || childName.Contains("arm_l") || childName.Contains("l_arm") || childName.Contains("left arm");
    }

    bool IsRightArmName(string childName)
    {
        return childName.Contains("arm-right") || childName.Contains("rightarm") || childName.Contains("right-arm") || childName.Contains("arm_r") || childName.Contains("r_arm") || childName.Contains("right arm");
    }

    void AnimateArms(bool chasing)
    {
        if (!chasing)
        {
            ResetArms();
            return;
        }

        float swing = Mathf.Sin(Time.time * armSwingSpeed) * armSwingAmount;
        float raise = keepArmsRaised ? armRaiseAngle : 0f;

        if (leftArm != null)
        {
            leftArm.localRotation = useSimpleArmRotation ? Quaternion.Euler(raise + swing, 0f, 0f) : leftArmStartRotation * Quaternion.Euler(raise + swing, 0f, 0f);
        }

        if (rightArm != null)
        {
            rightArm.localRotation = useSimpleArmRotation ? Quaternion.Euler(raise - swing, 0f, 0f) : rightArmStartRotation * Quaternion.Euler(raise - swing, 0f, 0f);
        }
    }

    void ResetArms()
    {
        if (leftArm != null)
        {
            leftArm.localRotation = leftArmStartRotation;
        }

        if (rightArm != null)
        {
            rightArm.localRotation = rightArmStartRotation;
        }
    }
}
