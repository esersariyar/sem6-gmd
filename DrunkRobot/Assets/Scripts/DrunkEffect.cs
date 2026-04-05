using UnityEngine;
using UnityEngine.UI;

public class DrunkEffect : MonoBehaviour
{
    public Transform cameraTarget;
    public float tiltAmount = 8f;
    public float tiltSpeed = 2f;
    public float swayAmount = 2f;
    public float swaySpeed = 1.5f;

    public float boostTiltMultiplier = 3f;
    public float boostSpeedMultiplier = 2.5f;
    public float boostDuration = 2f;

    public Image blurOverlay;

    public bool isDrunk = false;
    private float baseX;
    private float baseY;

    private float boostTimer = 0f;
    private bool isBoosted = false;

    void Start()
    {
        Vector3 startRot = cameraTarget.localEulerAngles;
        baseX = NormalizeAngle(startRot.x);
        baseY = NormalizeAngle(startRot.y);

        if (blurOverlay != null)
            blurOverlay.color = new Color(1f, 0f, 0f, 0f);
    }

    void Update()
    {
        if (!isDrunk) return;

        if (isBoosted)
        {
            boostTimer -= Time.deltaTime;
            if (boostTimer <= 0f)
            {
                isBoosted = false;
            }
        }

        float currentTilt = isBoosted ? tiltAmount * boostTiltMultiplier : tiltAmount;
        float currentTiltSpeed = isBoosted ? tiltSpeed * boostSpeedMultiplier : tiltSpeed;
        float currentSway = isBoosted ? swayAmount * boostTiltMultiplier : swayAmount;
        float currentSwaySpeed = isBoosted ? swaySpeed * boostSpeedMultiplier : swaySpeed;

        float swayX = Mathf.Sin(Time.time * currentSwaySpeed) * currentSway;
        float swayZ = Mathf.Sin(Time.time * currentTiltSpeed) * currentTilt;

        cameraTarget.localRotation = Quaternion.Euler(baseX + swayX, baseY, swayZ);

        if (blurOverlay != null)
        {
            float targetAlpha = isBoosted ? 0.3f : 0f;
            Color c = blurOverlay.color;
            c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * 5f);
            blurOverlay.color = c;
        }
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

    float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}