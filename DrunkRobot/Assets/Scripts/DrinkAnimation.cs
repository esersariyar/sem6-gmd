using UnityEngine;
using System.Collections;

public class DrinkAnimation : MonoBehaviour
{
    public Transform rightArm;
    public Transform bottle;
    public DrinkInteraction interaction;
    public MouseLook mouseLook;

    public float raiseSpeed = 2f;
    public float lowerSpeed = 1.5f;
    public float holdTime = 1.5f;

    private bool isDrinking = false;
    private Transform handPoint;

    void EnsureReferences()
    {
        if (rightArm == null || bottle == null)
        {
            return;
        }

        if (handPoint == null)
        {
            Transform existing = rightArm.Find("_AutoHandPoint");
            if (existing != null)
            {
                handPoint = existing;
            }
            else
            {
                GameObject handPointObject = new GameObject("_AutoHandPoint");
                handPoint = handPointObject.transform;
                handPoint.SetParent(rightArm, false);
                handPoint.localPosition = new Vector3(0f, 0.15f, 0.25f);
                handPoint.localRotation = Quaternion.identity;
            }
        }
    }

    public void PlayDrink()
    {
        if (isDrinking) return;

        EnsureReferences();
        if (rightArm == null || bottle == null || handPoint == null) return;

        StartCoroutine(DrinkAnim());
    }

    IEnumerator DrinkAnim()
    {
        isDrinking = true;

        bottle.SetParent(handPoint);
        bottle.localPosition = new Vector3(0f, 0.15f, 0.25f);
        bottle.localRotation = Quaternion.identity;

        Quaternion startRot = rightArm.localRotation;
        Quaternion targetRot = Quaternion.Euler(-110f, -10f, -20f);

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * raiseSpeed;
            rightArm.localRotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        yield return new WaitForSeconds(holdTime);

        if (interaction != null)
        {
            interaction.DisableInteraction();
        }

        if (mouseLook != null)
        {
            mouseLook.ActivateDrunk();
        }

        Destroy(bottle.gameObject);

        t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * lowerSpeed;
            rightArm.localRotation = Quaternion.Slerp(targetRot, startRot, t);
            yield return null;
        }

        isDrinking = false;
    }
}