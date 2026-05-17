using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MouseLook : MonoBehaviour
{
    public Transform playerBody;
    public float sensitivity = 120f;
    public float smoothTime = 0.05f;
    public bool useMouseLook = true;
    public bool useArcadeLookAxis = true;
    public string arcadeLookHorizontalAxis = "Arcade Look Horizontal";
    public string alternateArcadeLookHorizontalAxis = "Arcade Look Horizontal 9";
    public float arcadeTurnSensitivity = 140f;

    public bool isDrunk = false;
    public float drunkAmount = 2f;
    public float drunkSpeed = 1.5f;

    public float boostMultiplier = 3f;
    public float boostDuration = 2f;
    public Image blurOverlay;

    public bool canLook = false;

    float xRotation = 0f;

    float currentMouseX;
    float currentMouseY;

    float mouseXVelocity;
    float mouseYVelocity;

    private bool isBoosted = false;
    private float boostTimer = 0f;
    private Coroutine soberCoroutine;
    private float soberTimer = 0f;
    private float soberTimerMax = 0f;
    private bool restoreDrunkAfterSober = false;

    public bool IsSobering => soberCoroutine != null;
    public float SoberRemaining => soberTimer;
    public float SoberDuration => soberTimerMax;
    public float SoberProgress => soberTimerMax > 0f ? soberTimer / soberTimerMax : 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        xRotation = 0f;
        transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        if (blurOverlay != null)
            blurOverlay.color = new Color(1f, 0f, 0f, 0f);

        arcadeLookHorizontalAxis = arcadeLookHorizontalAxis.Trim();
        alternateArcadeLookHorizontalAxis = alternateArcadeLookHorizontalAxis.Trim();

        if (arcadeLookHorizontalAxis == "Debug Horizontal")
        {
            arcadeLookHorizontalAxis = "Arcade Look Horizontal";
        }
    }

    void OnEnable()
    {
        PlayerRespawn.Respawned += ClearCoffeeEffect;
    }

    void OnDisable()
    {
        PlayerRespawn.Respawned -= ClearCoffeeEffect;
    }

    void Update()
    {
        if (isBoosted)
        {
            boostTimer -= Time.deltaTime;
            if (boostTimer <= 0f)
                isBoosted = false;
        }

        if (blurOverlay != null)
        {
            float targetAlpha = isBoosted ? 0.3f : 0f;
            Color c = blurOverlay.color;
            c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * 5f);
            blurOverlay.color = c;
        }

        if (!canLook) return;

        float targetMouseX = 0f;
        float targetMouseY = 0f;

        if (useMouseLook)
        {
            targetMouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            targetMouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        }

        if (useArcadeLookAxis)
        {
            float horizontalLook = GetAxisRawSafe(arcadeLookHorizontalAxis);
            if (Mathf.Abs(horizontalLook) <= 0.1f)
            {
                horizontalLook = GetAxisRawSafe(alternateArcadeLookHorizontalAxis);
            }

            if (Mathf.Abs(horizontalLook) > 0.1f)
            {
                targetMouseX += horizontalLook * arcadeTurnSensitivity * Time.deltaTime;
            }
        }

        currentMouseX = Mathf.SmoothDamp(currentMouseX, targetMouseX, ref mouseXVelocity, smoothTime);
        currentMouseY = Mathf.SmoothDamp(currentMouseY, targetMouseY, ref mouseYVelocity, smoothTime);

        xRotation -= currentMouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        float drunkOffsetX = 0f;
        float drunkOffsetZ = 0f;

        if (isDrunk || isBoosted)
        {
            float currentAmount = isBoosted ? drunkAmount * boostMultiplier : drunkAmount;
            float currentSpeed = isBoosted ? drunkSpeed * boostMultiplier : drunkSpeed;

            drunkOffsetX = Mathf.Sin(Time.time * currentSpeed) * currentAmount;
            drunkOffsetZ = Mathf.Sin(Time.time * currentSpeed * 0.7f) * currentAmount * 2f;
        }

        transform.localRotation = Quaternion.Euler(xRotation + drunkOffsetX, 0f, drunkOffsetZ);

        playerBody.Rotate(Vector3.up * currentMouseX);
    }

    public void ActivateDrunk()
    {
        isDrunk = true;
    }

    public void BoostDrunk()
    {
        isBoosted = true;
        boostTimer = boostDuration;
    }

    public void SuppressDrunk(float duration)
    {
        restoreDrunkAfterSober = restoreDrunkAfterSober || isDrunk;

        if (soberCoroutine != null)
        {
            StopCoroutine(soberCoroutine);
        }

        soberTimerMax = duration;
        soberTimer = duration;
        soberCoroutine = StartCoroutine(SuppressDrunkRoutine(duration));
    }

    public void ClearCoffeeEffect()
    {
        if (soberCoroutine != null)
        {
            StopCoroutine(soberCoroutine);
        }

        soberCoroutine = null;
        soberTimer = 0f;
        soberTimerMax = 0f;
        isDrunk = restoreDrunkAfterSober || isDrunk;
        restoreDrunkAfterSober = false;
    }

    IEnumerator SuppressDrunkRoutine(float duration)
    {
        isDrunk = false;
        isBoosted = false;
        boostTimer = 0f;

        if (blurOverlay != null)
        {
            Color c = blurOverlay.color;
            c.a = 0f;
            blurOverlay.color = c;
        }

        while (soberTimer > 0f)
        {
            soberTimer -= Time.deltaTime;
            yield return null;
        }

        soberTimer = 0f;
        isDrunk = restoreDrunkAfterSober;
        restoreDrunkAfterSober = false;
        soberCoroutine = null;
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
