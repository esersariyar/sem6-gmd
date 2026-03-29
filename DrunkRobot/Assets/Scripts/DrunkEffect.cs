using UnityEngine;

public class DrunkEffect : MonoBehaviour
{
    public Transform cameraTarget;
    public float tiltAmount = 8f;
    public float tiltSpeed = 2f;
    public float swayAmount = 2f;
    public float swaySpeed = 1.5f;

    private bool isDrunk = false;
    private float baseX;
    private float baseY;

    void Start()
    {
        Vector3 startRot = cameraTarget.localEulerAngles;
        baseX = NormalizeAngle(startRot.x);
        baseY = NormalizeAngle(startRot.y);
    }

    void Update()
    {
        if (!isDrunk) return;

        float swayX = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
        float swayZ = Mathf.Sin(Time.time * tiltSpeed) * tiltAmount;

        cameraTarget.localRotation = Quaternion.Euler(baseX + swayX, baseY, swayZ);
    }

    public void ActivateDrunk()
    {
        isDrunk = true;
    }

    float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}