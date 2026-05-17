using UnityEngine;

public class FemaleRobotFlee : MonoBehaviour
{
    public Transform player;
    public Transform goal;
    public Transform[] escapeWaypoints;
    public float keepAwayDistance = 10f;
    public float keepAwaySpeed = 12f;
    public float emergencyDistance = 5f;
    public float emergencySpeedMultiplier = 2.6f;
    public float contactEscapeMultiplier = 3.5f;
    public float escapeDelay = 8f;
    public float escapeSpeed = 24f;
    public float goalStopDistance = 0.4f;
    public float turnSpeed = 10f;
    public float bodyRadius = 0.55f;
    public float bodyHeight = 2f;
    public LayerMask obstacleMask = ~0;
    public bool lockYPosition = true;
    public Transform leftLeg;
    public Transform rightLeg;
    public Transform leftArm;
    public Transform rightArm;
    public float walkSwingSpeed = 10f;
    public float legSwingAmount = 28f;
    public float armSwingAmount = 28f;

    private float startY;
    private float timer;
    private int waypointIndex;
    private bool escaping;
    private bool arrived;
    private bool isMoving;

    public bool HasArrived => arrived;
    private Quaternion leftLegStartRotation;
    private Quaternion rightLegStartRotation;
    private Quaternion leftArmStartRotation;
    private Quaternion rightArmStartRotation;
    private Rigidbody rb;

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
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.mass = 50f;

        if (lockYPosition)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        }

        startY = transform.position.y;
        EnsureCollider();
        AutoFindLimbs();
        StoreLimbRotations();
    }

    void Start()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }
    }

    void FixedUpdate()
    {
        isMoving = false;

        if (arrived)
        {
            AnimateWalk(false);
            return;
        }

        timer += Time.fixedDeltaTime;

        if (!escaping && timer >= escapeDelay)
        {
            escaping = true;
        }

        if (escaping)
        {
            MoveToGoal();
            AnimateWalk(isMoving);
            return;
        }

        KeepAwayFromPlayer();
        AnimateWalk(isMoving);
    }

    void OnCollisionStay(Collision collision)
    {
        ForceAwayFromPlayer(collision.gameObject);
    }

    void OnTriggerStay(Collider other)
    {
        ForceAwayFromPlayer(other.gameObject);
    }

    void ForceAwayFromPlayer(GameObject hitObject)
    {
        if (player == null || !hitObject.CompareTag("Player") || arrived)
        {
            return;
        }

        Vector3 away = transform.position - player.position;
        away.y = 0f;
        if (away.sqrMagnitude < 0.001f)
        {
            away = -transform.forward;
        }

        Move(away.normalized, keepAwaySpeed * contactEscapeMultiplier, true);
    }

    void KeepAwayFromPlayer()
    {
        if (player == null)
        {
            return;
        }

        Vector3 away = transform.position - player.position;
        away.y = 0f;

        float distance = away.magnitude;
        if (distance >= keepAwayDistance || away.sqrMagnitude < 0.001f)
        {
            return;
        }

        Vector3 direction = away.normalized;
        Transform target = GetEscapeTarget();
        if (target != null)
        {
            Vector3 toTarget = target.position - transform.position;
            toTarget.y = 0f;
            if (toTarget.sqrMagnitude > 0.001f)
            {
                direction = (direction * 1.35f + toTarget.normalized * 0.75f).normalized;
            }
        }

        float closeness = 1f - Mathf.Clamp01(distance / keepAwayDistance);
        float speed = keepAwaySpeed + keepAwaySpeed * emergencySpeedMultiplier * closeness;
        Move(direction, speed, true);
    }

    void MoveToGoal()
    {
        Transform target = GetEscapeTarget();
        if (target == null)
        {
            return;
        }

        Vector3 toGoal = target.position - transform.position;
        toGoal.y = 0f;

        if (toGoal.magnitude <= goalStopDistance)
        {
            if (escapeWaypoints != null && waypointIndex < escapeWaypoints.Length)
            {
                waypointIndex++;
                return;
            }

            arrived = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            Vector3 finalPosition = target.position;
            if (lockYPosition)
            {
                finalPosition.y = startY;
            }

            rb.position = finalPosition;
            return;
        }

        Move(toGoal.normalized, escapeSpeed, false);
    }

    Transform GetEscapeTarget()
    {
        if (escapeWaypoints == null)
        {
            return goal;
        }

        while (waypointIndex < escapeWaypoints.Length && escapeWaypoints[waypointIndex] == null)
        {
            waypointIndex++;
        }

        if (waypointIndex < escapeWaypoints.Length)
        {
            return escapeWaypoints[waypointIndex];
        }

        return goal;
    }

    void Move(Vector3 direction, float speed, bool keepPlayerAway)
    {
        if (direction.sqrMagnitude < 0.001f)
        {
            return;
        }

        Vector3 moveDirection = FindOpenDirection(direction, speed * Time.fixedDeltaTime, keepPlayerAway);
        if (moveDirection.sqrMagnitude < 0.001f)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);

        Vector3 nextPosition = rb.position + moveDirection * speed * Time.fixedDeltaTime;
        if (lockYPosition)
        {
            nextPosition.y = startY;
        }

        rb.MovePosition(nextPosition);
        isMoving = true;
    }

    Vector3 FindOpenDirection(Vector3 preferredDirection, float moveDistance, bool keepPlayerAway)
    {
        Vector3[] directions =
        {
            preferredDirection,
            Quaternion.Euler(0f, 20f, 0f) * preferredDirection,
            Quaternion.Euler(0f, -20f, 0f) * preferredDirection,
            Quaternion.Euler(0f, 40f, 0f) * preferredDirection,
            Quaternion.Euler(0f, -40f, 0f) * preferredDirection,
            Quaternion.Euler(0f, 70f, 0f) * preferredDirection,
            Quaternion.Euler(0f, -70f, 0f) * preferredDirection,
            Quaternion.Euler(0f, 110f, 0f) * preferredDirection,
            Quaternion.Euler(0f, -110f, 0f) * preferredDirection,
            Quaternion.Euler(0f, 150f, 0f) * preferredDirection,
            Quaternion.Euler(0f, -150f, 0f) * preferredDirection
        };

        float bestScore = float.NegativeInfinity;
        Vector3 bestDirection = Vector3.zero;

        for (int i = 0; i < directions.Length; i++)
        {
            if (directions[i].sqrMagnitude <= 0.001f)
            {
                continue;
            }

            Vector3 candidate = directions[i].normalized;
            if (!CanMove(candidate, moveDistance))
            {
                continue;
            }

            float score = Vector3.Dot(candidate, preferredDirection.normalized);
            if (keepPlayerAway && player != null)
            {
                Vector3 futurePosition = rb.position + candidate * moveDistance;
                Vector3 futureAway = futurePosition - player.position;
                futureAway.y = 0f;
                score += futureAway.magnitude * 0.4f;
            }

            if (score > bestScore)
            {
                bestScore = score;
                bestDirection = candidate;
            }
        }

        return bestDirection;
    }

    bool CanMove(Vector3 direction, float moveDistance)
    {
        Vector3 bottom = transform.position + Vector3.up * bodyRadius;
        Vector3 top = transform.position + Vector3.up * (bodyHeight - bodyRadius);
        float castDistance = moveDistance + 0.15f;

        if (Physics.CapsuleCast(bottom, top, bodyRadius * 0.9f, direction, out RaycastHit hit, castDistance, obstacleMask, QueryTriggerInteraction.Ignore))
        {
            return hit.collider.transform.IsChildOf(transform);
        }

        return true;
    }

    void EnsureCollider()
    {
        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        if (capsule == null)
        {
            capsule = gameObject.AddComponent<CapsuleCollider>();
        }

        capsule.radius = bodyRadius;
        capsule.height = bodyHeight;
        capsule.center = new Vector3(0f, bodyHeight * 0.5f, 0f);
        capsule.isTrigger = false;
    }

    void AutoFindLimbs()
    {
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            string childName = child.name.ToLowerInvariant();

            if (leftLeg == null && IsLeftLegName(childName))
            {
                leftLeg = child;
            }

            if (rightLeg == null && IsRightLegName(childName))
            {
                rightLeg = child;
            }

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

    bool IsLeftLegName(string childName)
    {
        return childName.Contains("leg-left") || childName.Contains("leftleg") || childName.Contains("left-leg") || childName.Contains("leg_l") || childName.Contains("l_leg");
    }

    bool IsRightLegName(string childName)
    {
        return childName.Contains("leg-right") || childName.Contains("rightleg") || childName.Contains("right-leg") || childName.Contains("leg_r") || childName.Contains("r_leg");
    }

    bool IsLeftArmName(string childName)
    {
        return childName.Contains("arm-left") || childName.Contains("leftarm") || childName.Contains("left-arm") || childName.Contains("arm_l") || childName.Contains("l_arm");
    }

    bool IsRightArmName(string childName)
    {
        return childName.Contains("arm-right") || childName.Contains("rightarm") || childName.Contains("right-arm") || childName.Contains("arm_r") || childName.Contains("r_arm");
    }

    void StoreLimbRotations()
    {
        if (leftLeg != null)
        {
            leftLegStartRotation = leftLeg.localRotation;
        }

        if (rightLeg != null)
        {
            rightLegStartRotation = rightLeg.localRotation;
        }

        if (leftArm != null)
        {
            leftArmStartRotation = leftArm.localRotation;
        }

        if (rightArm != null)
        {
            rightArmStartRotation = rightArm.localRotation;
        }
    }

    void AnimateWalk(bool moving)
    {
        if (!moving)
        {
            ResetLimbs();
            return;
        }

        float swing = Mathf.Sin(Time.time * walkSwingSpeed);

        if (leftLeg != null)
        {
            leftLeg.localRotation = leftLegStartRotation * Quaternion.Euler(swing * legSwingAmount, 0f, 0f);
        }

        if (rightLeg != null)
        {
            rightLeg.localRotation = rightLegStartRotation * Quaternion.Euler(-swing * legSwingAmount, 0f, 0f);
        }

        if (leftArm != null)
        {
            leftArm.localRotation = leftArmStartRotation * Quaternion.Euler(-swing * armSwingAmount, 0f, 0f);
        }

        if (rightArm != null)
        {
            rightArm.localRotation = rightArmStartRotation * Quaternion.Euler(swing * armSwingAmount, 0f, 0f);
        }
    }

    void ResetLimbs()
    {
        if (leftLeg != null)
        {
            leftLeg.localRotation = leftLegStartRotation;
        }

        if (rightLeg != null)
        {
            rightLeg.localRotation = rightLegStartRotation;
        }

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
