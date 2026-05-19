using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 0.5f;
    public float jumpForce = 8f;
    public float airControl = 0.3f;
    public KeyCode keyboardJumpKey = KeyCode.Space;
    public KeyCode leftGreenButton = KeyCode.Joystick1Button1;
    public KeyCode rightGreenButton = KeyCode.Joystick2Button1;
    public string moveHorizontalAxis = "Arcade Move Horizontal";
    public string moveVerticalAxis = "Arcade Move Vertical";
    public bool useKeyboardMovement = true;

    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundLayer;
    public float coyoteTime = 0.2f;

    public int maxJumps = 2;

    [Header("Drunk Stumble")]
    public MouseLook mouseLook;
    public float stumbleMinInterval = 1.2f;
    public float stumbleMaxInterval = 2.8f;
    public float stumbleMinDuration = 0.4f;
    public float stumbleMaxDuration = 0.9f;
    [Range(0f, 1f)] public float chanceOfReverse = 0.4f;
    public float baseDriftAmplitude = 25f;
    public float baseDriftSpeed = 1.8f;
    public float stumbleAngleMultiplier = 1f;

    private float nextStumbleTime;
    private float stumbleEndTime;
    private float stumbleAngle;
    private float driftSeed;

    private Rigidbody rb;
    private bool isGrounded;
    private Vector3 moveInput;
    public bool canMove = false;
    private int jumpsRemaining;
    private Coroutine speedBoostCoroutine;
    private float baseSpeed;
    private float groundTimer;
    private Vector3 externalVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        jumpsRemaining = maxJumps;
        baseSpeed = speed;

        if (mouseLook == null)
        {
            mouseLook = GetComponentInChildren<MouseLook>();
        }

        ApplySceneDrunkTuning();
        ScheduleNextStumble();
        driftSeed = Random.Range(0f, 100f);
    }

    void ApplySceneDrunkTuning()
    {
        if (SceneManager.GetActiveScene().name != "Level1")
        {
            return;
        }

        chanceOfReverse = Mathf.Min(chanceOfReverse, 0.08f);
        baseDriftAmplitude = Mathf.Min(baseDriftAmplitude, 8f);
        stumbleAngleMultiplier = Mathf.Min(stumbleAngleMultiplier, 0.35f);
        stumbleMinInterval = Mathf.Max(stumbleMinInterval, 2.2f);
        stumbleMaxInterval = Mathf.Max(stumbleMaxInterval, 4f);
        stumbleMinDuration = Mathf.Min(stumbleMinDuration, 0.25f);
        stumbleMaxDuration = Mathf.Min(stumbleMaxDuration, 0.45f);
    }

    void ScheduleNextStumble()
    {
        nextStumbleTime = Time.time + Random.Range(stumbleMinInterval, stumbleMaxInterval);
    }

    Vector2 ApplyDrunkStumble(float h, float v)
    {
        if (mouseLook == null || !mouseLook.isDrunk)
        {
            return new Vector2(h, v);
        }

        if (Time.time >= stumbleEndTime && Time.time >= nextStumbleTime)
        {
            stumbleEndTime = Time.time + Random.Range(stumbleMinDuration, stumbleMaxDuration);
            bool reverse = Random.value < chanceOfReverse;
            if (reverse)
            {
                stumbleAngle = (180f + Random.Range(-30f, 30f)) * stumbleAngleMultiplier;
            }
            else
            {
                stumbleAngle = ((Random.value < 0.5f ? 90f : -90f) + Random.Range(-25f, 25f)) * stumbleAngleMultiplier;
            }
            ScheduleNextStumble();
        }

        if (Mathf.Abs(h) < 0.05f && Mathf.Abs(v) < 0.05f)
        {
            return new Vector2(h, v);
        }

        float drift = (Mathf.PerlinNoise(driftSeed, Time.time * baseDriftSpeed) - 0.5f) * 2f * baseDriftAmplitude;
        float angle = drift;

        if (Time.time < stumbleEndTime)
        {
            angle += stumbleAngle;
        }

        float rad = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        float newH = h * cos - v * sin;
        float newV = h * sin + v * cos;
        return new Vector2(newH, newV);
    }

    void OnEnable()
    {
        PlayerRespawn.Respawned += ClearSpeedBoost;
    }

    void OnDisable()
    {
        PlayerRespawn.Respawned -= ClearSpeedBoost;
    }

    void Update()
    {
        UpdateGrounded();

        if (isGrounded)
        {
            jumpsRemaining = maxJumps;
            groundTimer = coyoteTime;
        }
        else
        {
            groundTimer -= Time.deltaTime;
        }

        if (!canMove)
        {
            moveInput = Vector3.zero;
            return;
        }

        float h = GetMoveHorizontal();
        float v = GetMoveVertical();

        if (Mathf.Abs(h) < 0.1f) h = 0f;
        if (Mathf.Abs(v) < 0.1f) v = 0f;

        Vector2 stumbled = ApplyDrunkStumble(h, v);
        h = stumbled.x;
        v = stumbled.y;

        Transform cam = Camera.main.transform;

        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        moveInput = camForward * v + camRight * h;

        if (v > 0f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(camForward);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        if (IsJumpPressed() && (jumpsRemaining > 0 || groundTimer > 0f))
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpsRemaining = Mathf.Max(0, jumpsRemaining - 1);
            groundTimer = 0f;
        }
    }

    void UpdateGrounded()
    {
        Vector3 checkPosition = groundCheck != null ? groundCheck.position : transform.position + Vector3.down * 0.9f;
        isGrounded = Physics.CheckSphere(checkPosition, groundDistance, groundLayer, QueryTriggerInteraction.Ignore);
    }

    void FixedUpdate()
    {
        bool isSpeedBoosted = speedBoostCoroutine != null;
        float currentSpeed = isGrounded || isSpeedBoosted ? speed : speed * airControl;

        Vector3 move = moveInput.normalized * currentSpeed;

        Vector3 velocity = new Vector3(move.x + externalVelocity.x, rb.linearVelocity.y, move.z + externalVelocity.z);
        rb.linearVelocity = velocity;
        externalVelocity = Vector3.MoveTowards(externalVelocity, Vector3.zero, 45f * Time.fixedDeltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.normal.y > 0.3f)
            {
                jumpsRemaining = maxJumps;
                groundTimer = coyoteTime;
                return;
            }
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (isGrounded) return;
        
        foreach (ContactPoint contact in collision.contacts)
        {
            if (Mathf.Abs(contact.normal.x) > 0.5f || Mathf.Abs(contact.normal.z) > 0.5f)
            {
                if (jumpsRemaining < 1)
                    jumpsRemaining = 1;
                return;
            }
        }
    }

    public void BoostSpeed(float multiplier, float duration)
    {
        if (speedBoostCoroutine != null)
        {
            StopCoroutine(speedBoostCoroutine);
        }

        speedBoostCoroutine = StartCoroutine(BoostSpeedRoutine(multiplier, duration));
    }

    IEnumerator BoostSpeedRoutine(float multiplier, float duration)
    {
        speed = baseSpeed * multiplier;

        yield return new WaitForSeconds(duration);

        speed = baseSpeed;
        speedBoostCoroutine = null;
    }

    public void ClearSpeedBoost()
    {
        if (speedBoostCoroutine != null)
        {
            StopCoroutine(speedBoostCoroutine);
        }

        speedBoostCoroutine = null;
        speed = baseSpeed;
    }

    public void AddExternalVelocity(Vector3 velocity)
    {
        externalVelocity += velocity;
    }

    public void SetExternalVelocity(Vector3 velocity)
    {
        if (velocity.sqrMagnitude > externalVelocity.sqrMagnitude)
        {
            externalVelocity = velocity;
        }
    }

    bool IsJumpPressed()
    {
        return Input.GetKeyDown(keyboardJumpKey)
            || Input.GetKeyDown(leftGreenButton)
            || Input.GetKeyDown(rightGreenButton);
    }

    float GetMoveHorizontal()
    {
        float value = GetAxisRawSafe(moveHorizontalAxis);

        if (useKeyboardMovement)
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                value -= 1f;
            }

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                value += 1f;
            }
        }

        return Mathf.Clamp(value, -1f, 1f);
    }

    float GetMoveVertical()
    {
        float value = GetAxisRawSafe(moveVerticalAxis);

        if (useKeyboardMovement)
        {
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                value -= 1f;
            }

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                value += 1f;
            }
        }

        return Mathf.Clamp(value, -1f, 1f);
    }

    float GetAxisRawSafe(string axisName)
    {
        if (string.IsNullOrEmpty(axisName))
        {
            return 0f;
        }

        try
        {
            return Input.GetAxisRaw(axisName);
        }
        catch
        {
            return 0f;
        }
    }
}
