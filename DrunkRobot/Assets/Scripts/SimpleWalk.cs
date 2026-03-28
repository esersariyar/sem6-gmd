using UnityEngine;

public class SimpleWalk : MonoBehaviour
{
    public Transform leftLeg;
    public Transform rightLeg;
    public Transform leftArm;
    public Transform rightArm;

    public float walkSpeed = 8f;
    public float walkAmount = 30f;

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v);

        if (move.magnitude > 0.1f)
        {
            float angle = Mathf.Sin(Time.time * walkSpeed) * walkAmount;

            leftLeg.localRotation = Quaternion.Euler(angle, 0, 0);
            rightLeg.localRotation = Quaternion.Euler(-angle, 0, 0);

            leftArm.localRotation = Quaternion.Euler(-angle, 0, 0);
            rightArm.localRotation = Quaternion.Euler(angle, 0, 0);
        }
    }
}