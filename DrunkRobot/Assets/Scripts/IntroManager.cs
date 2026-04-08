using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.Cinemachine;

public class IntroManager : MonoBehaviour
{
    public Image fadeImage;
    public GameObject loadingImage;
    public PlayerMovement playerMovement;
    public MouseLook mouseLook;

    public AudioSource audioSource;

    public CinemachineCamera introCam;
    public CinemachineCamera introWhiskeyCam;
    public CinemachineCamera playerCam;
    public Transform characterHead;

    public float fadeSpeed = 1f;
    public float waitDuration = 5f;
    public float whiskeyCamDuration = 2.5f;
    public float gameplayVolume = 0.3f;
    public float volumeFadeDuration = 2f;

    private float alpha = 1f;
    public bool playerCamActivated = false;
    private Coroutine introSequenceRoutine;
    private float sequenceStartRealtime;

    void Start()
    {
        if (playerMovement != null)
        {
            playerMovement.canMove = false;
        }
         if (mouseLook != null)
            {
                mouseLook.canLook = false;
            }

        if (audioSource != null)
        {
            audioSource.volume = 1f;
            audioSource.Play();
        }

        sequenceStartRealtime = Time.realtimeSinceStartup;
        introSequenceRoutine = StartCoroutine(RunIntroSequence());
    }

        void Update()
        {
            if (playerCamActivated)
            {
                return;
            }

            float maxIntroTime = Mathf.Max(6f, waitDuration + whiskeyCamDuration + 6f);

            if (Time.realtimeSinceStartup - sequenceStartRealtime >= maxIntroTime)
            {
                ForceFinishIntro();
            }
        }

    void LateUpdate()
    {
        if (playerCamActivated && playerCam != null)
        {
            ActivateOnlyCamera(playerCam);
        }
    }

void ForceFinishIntro()
{
    playerCamActivated = true;

    StopAllCoroutines();

    if (introCam != null)
    {
        introCam.gameObject.SetActive(false);
    }

    if (introWhiskeyCam != null)
    {
        introWhiskeyCam.gameObject.SetActive(false);
    }

    if (playerCam != null)
    {
        ActivateOnlyCamera(playerCam);
    }

    if (fadeImage != null)
    {
        fadeImage.gameObject.SetActive(false);
    }

    if (loadingImage != null)
    {
        loadingImage.SetActive(false);
    }

    if (playerMovement != null)
    {
        playerMovement.canMove = true;
    }

    if (mouseLook != null)
    {
        mouseLook.canLook = true;
    }

    if (audioSource != null)
    {
        StartCoroutine(LowerVolume());
    }
}

    IEnumerator RunIntroSequence()
    {
        if (introCam != null)
        {
            ActivateOnlyCamera(introCam);
        }
        else if (introWhiskeyCam != null)
        {
            ActivateOnlyCamera(introWhiskeyCam);
        }
        else if (playerCam != null)
        {
            ActivateOnlyCamera(playerCam);
        }

        yield return FadeTo(0f);
        yield return new WaitForSecondsRealtime(Mathf.Max(0f, waitDuration));

        yield return FadeTo(1f);

        if (loadingImage != null)
        {
            loadingImage.SetActive(false);
        }

        if (introWhiskeyCam != null)
        {
            ActivateOnlyCamera(introWhiskeyCam);
            yield return FadeTo(0f);
            if (characterHead != null)
            {
                yield return ShakeHead();
            }
            yield return new WaitForSecondsRealtime(Mathf.Max(0f, whiskeyCamDuration));
            yield return FadeTo(1f);
        }

        ActivatePlayerCam();

        yield return FadeTo(0f);

        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(false);
        }

        introSequenceRoutine = null;
    }

    void SetAlpha(float a)
    {
        if (fadeImage == null) return;

        Color c = fadeImage.color;
        c.a = a;
        fadeImage.color = c;
    }

    IEnumerator LowerVolume()
    {
        float startVolume = audioSource.volume;
        float t = 0f;

        while (t < volumeFadeDuration)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, gameplayVolume, t / volumeFadeDuration);
            yield return null;
        }

        audioSource.volume = gameplayVolume;
    }

    IEnumerator FadeTo(float targetAlpha)
    {
        if (fadeImage == null)
        {
            yield break;
        }

        float startAlpha = alpha;
        float duration = Mathf.Approximately(fadeSpeed, 0f) ? 0f : 1f / Mathf.Max(0.0001f, fadeSpeed);
        float t = 0f;

        if (duration <= 0f)
        {
            alpha = targetAlpha;
            SetAlpha(alpha);
            yield break;
        }

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            alpha = Mathf.Lerp(startAlpha, targetAlpha, t / duration);
            SetAlpha(alpha);
            yield return null;
        }

        alpha = targetAlpha;
        SetAlpha(alpha);
    }

    void ActivatePlayerCam()
    {
        if (playerCamActivated)
        {
            return;
        }

        playerCamActivated = true;

        if (introWhiskeyCam != null)
        {
            introWhiskeyCam.Priority = 0;
            introWhiskeyCam.gameObject.SetActive(false);
        }

        if (playerCam != null)
        {
            ActivateOnlyCamera(playerCam);
        }

        if (playerMovement != null)
        {
            playerMovement.canMove = true;
        }

        if (mouseLook != null)
        {
            mouseLook.canLook = true;
        }

        if (audioSource != null)
        {
            StartCoroutine(LowerVolume());
        }
    }

    void ActivateOnlyCamera(CinemachineCamera targetCam)
    {
        SetCameraState(introCam, targetCam == introCam);
        SetCameraState(introWhiskeyCam, targetCam == introWhiskeyCam);
        SetCameraState(playerCam, targetCam == playerCam);
    }

    void SetCameraState(CinemachineCamera cam, bool active)
    {
        if (cam == null)
        {
            return;
        }

        cam.enabled = active;
        cam.gameObject.SetActive(active);
        cam.Priority = active ? 20 : 0;
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && !playerCamActivated)
        {
            sequenceStartRealtime = Time.realtimeSinceStartup - 100f;
        }
    }


    IEnumerator ShakeHead()
    {
        Quaternion originalRotation = characterHead.localRotation;
        float shakeAmount = 20f;
        float shakeDuration = 0.4f;
        int shakeCount = 2;

        for (int i = 0; i < shakeCount; i++)
        {
            float elapsed = 0f;
            while (elapsed < shakeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / shakeDuration;
                float curve = Mathf.Sin(t * Mathf.PI);
                float rotX = Mathf.Lerp(0f, shakeAmount, curve);
                characterHead.localRotation = originalRotation * Quaternion.Euler(rotX, 0, 0);
                yield return null;
            }
        }

        characterHead.localRotation = originalRotation;
    }
}