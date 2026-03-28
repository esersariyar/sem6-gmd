using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.Cinemachine;

public class IntroManager : MonoBehaviour
{
    public Image fadeImage;
    public GameObject loadingImage;
    public PlayerMovement playerMovement;

    public AudioSource audioSource;

    public CinemachineCamera introCam;
    public CinemachineCamera playerCam;

    public float fadeSpeed = 1f;
    public float waitDuration = 5f;
    public float gameplayVolume = 0.3f;
    public float volumeFadeDuration = 2f;

    private float alpha = 1f;
    private int stage = 0;
    private bool finished = false;
    private bool fadingIn = false;

    void Start()
    {
        playerMovement.canMove = false;

        if (introCam != null && playerCam != null)
        {
            introCam.Priority = 20;
            playerCam.Priority = 0;

            introCam.gameObject.SetActive(true);
            playerCam.gameObject.SetActive(true);
        }

        if (audioSource != null)
        {
            audioSource.volume = 1f;
            audioSource.Play();
        }
    }

    void Update()
    {
        if (stage == 0)
        {
            alpha -= Time.deltaTime * fadeSpeed;

            if (alpha <= 0)
            {
                alpha = 0;
                stage = 1;
                Invoke(nameof(StartFadeOut), waitDuration);
            }

            SetAlpha(alpha);
        }
    }

    void StartFadeOut()
    {
        stage = 2;
    }

    void LateUpdate()
    {
        if (stage == 2 && !finished)
        {
            alpha += Time.deltaTime * fadeSpeed;

            if (alpha >= 1)
            {
                alpha = 1;
                finished = true;

                loadingImage.SetActive(false);
                playerMovement.canMove = true;

                if (introCam != null && playerCam != null)
                {
                    introCam.Priority = 0;
                    playerCam.Priority = 20;
                }

                if (audioSource != null)
                {
                    StartCoroutine(LowerVolume());
                }

                stage = 3;
                fadingIn = true;
            }

            SetAlpha(alpha);
        }

        if (stage == 3 && fadingIn)
        {
            alpha -= Time.deltaTime * fadeSpeed;

            if (alpha <= 0)
            {
                alpha = 0;
                fadingIn = false;
                fadeImage.gameObject.SetActive(false);
            }

            SetAlpha(alpha);
        }
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
}