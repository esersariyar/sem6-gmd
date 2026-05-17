using UnityEngine;

public class MenuIconIntroGate : MonoBehaviour
{
    public IntroManager introManager;
    public GameObject menuIcon;
    public PauseMenu pauseMenu;
    public bool disablePauseMenuDuringIntro = true;

    void Awake()
    {
        if (introManager == null)
        {
            introManager = FindFirstObjectByType<IntroManager>();
        }

        if (menuIcon != null)
        {
            menuIcon.SetActive(false);
        }

        if (disablePauseMenuDuringIntro && pauseMenu != null)
        {
            pauseMenu.enabled = false;
        }
    }

    void Update()
    {
        if (introManager == null || introManager.playerCamActivated)
        {
            Reveal();
        }
    }

    void Reveal()
    {
        if (menuIcon != null && !menuIcon.activeSelf)
        {
            menuIcon.SetActive(true);
        }

        if (disablePauseMenuDuringIntro && pauseMenu != null && !pauseMenu.enabled)
        {
            pauseMenu.enabled = true;
        }

        enabled = false;
    }
}
