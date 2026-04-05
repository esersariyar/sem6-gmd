using UnityEngine;
using UnityEngine.UI;

public class MouseLook : MonoBehaviour
{
    public Transform playerBody;
    public float sensitivity = 120f;
    public float smoothTime = 0.05f;

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

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        xRotation = 0f;
        transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        if (blurOverlay != null)
            blurOverlay.color = new Color(1f, 0f, 0f, 0f);
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

        float targetMouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float targetMouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

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
}