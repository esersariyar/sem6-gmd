using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public Transform playerBody;
    public float sensitivity = 120f;
    public float smoothTime = 0.05f;

    public bool isDrunk = false;
    public float drunkAmount = 2f;
    public float drunkSpeed = 1.5f;

    public bool canLook = false;

    float xRotation = 0f;

    float currentMouseX;
    float currentMouseY;

    float mouseXVelocity;
    float mouseYVelocity;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!canLook) return;

        float targetMouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float targetMouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        currentMouseX = Mathf.SmoothDamp(currentMouseX, targetMouseX, ref mouseXVelocity, smoothTime);
        currentMouseY = Mathf.SmoothDamp(currentMouseY, targetMouseY, ref mouseYVelocity, smoothTime);

        xRotation -= currentMouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        float drunkOffsetX = 0f;
        float drunkOffsetZ = 0f;

        if (isDrunk)
        {
            drunkOffsetX = Mathf.Sin(Time.time * drunkSpeed) * drunkAmount;
            drunkOffsetZ = Mathf.Sin(Time.time * drunkSpeed * 0.7f) * drunkAmount * 2f;
        }

        transform.localRotation = Quaternion.Euler(xRotation + drunkOffsetX, 0f, drunkOffsetZ);

        playerBody.Rotate(Vector3.up * currentMouseX);
    }

    public void ActivateDrunk()
    {
        isDrunk = true;
    }
}