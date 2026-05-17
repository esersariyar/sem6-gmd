using UnityEngine;
using System.Collections;

public class DrinkAnimation : MonoBehaviour
{
    public enum DrinkEffectMode
    {
        ActivateDrunk,
        SuppressDrunk
    }

    public Transform rightArm;
    public Transform bottle;
    public DrinkInteraction interaction;
    public MouseLook mouseLook;
    public PlayerMovement playerMovement;
    public DrinkEffectMode effectMode = DrinkEffectMode.ActivateDrunk;
    public float soberDuration = 5f;
    public float speedBoostMultiplier = 1.5f;
    public bool destroyAfterDrink = true;

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

        if (mouseLook != null && effectMode == DrinkEffectMode.ActivateDrunk)
        {
            mouseLook.ActivateDrunk();
        }
        else if (mouseLook != null && effectMode == DrinkEffectMode.SuppressDrunk)
        {
            mouseLook.SuppressDrunk(soberDuration);

            if (playerMovement != null)
            {
                playerMovement.BoostSpeed(speedBoostMultiplier, soberDuration);
            }

            EnemyChaser.BoostAll(speedBoostMultiplier, soberDuration);
        }

        if (destroyAfterDrink)
        {
            Destroy(bottle.gameObject);
        }
        else
        {
            bottle.gameObject.SetActive(false);
        }

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
