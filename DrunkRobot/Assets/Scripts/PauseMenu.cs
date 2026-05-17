using TMPro;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject menuPanel;

    [Header("Toggle / Confirm buttons")]
    public KeyCode toggleKey = KeyCode.Escape;
    public KeyCode altToggleKey = KeyCode.P;
    public KeyCode leftMenuButton = KeyCode.Joystick1Button2;
    public KeyCode rightMenuButton = KeyCode.Joystick2Button2;

    [Header("Quit shortcut (only when menu open)")]
    public KeyCode quitKey = KeyCode.Q;
    public KeyCode leftQuitButton = KeyCode.Joystick1Button3;
    public KeyCode rightQuitButton = KeyCode.Joystick2Button3;

    [Header("Menu items (0 = Continue, 1 = Quit)")]
    public TextMeshProUGUI[] menuItemTexts;
    public Color selectedColor = new Color(1f, 0.85f, 0.2f, 1f);
    public Color unselectedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    public float selectedScale = 1.15f;
    public float navInputCooldown = 0.18f;

    public bool pauseTimeWhenOpen = true;

    private bool isOpen;
    private int selectedIndex;
    private float lastNavTime;

    void Awake()
    {
        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
        }

        isOpen = false;
        Time.timeScale = 1f;
        selectedIndex = 0;
        ApplyHighlight();
    }

    void Update()
    {
        if (TogglePressed())
        {
            if (isOpen)
            {
                ConfirmSelection();
            }
            else
            {
                Open();
            }
            return;
        }

        if (!isOpen)
        {
            return;
        }

        if (QuitPressed())
        {
            Quit();
            return;
        }

        HandleNavigation();
    }

    bool TogglePressed()
    {
        return Input.GetKeyDown(toggleKey)
            || Input.GetKeyDown(altToggleKey)
            || Input.GetKeyDown(leftMenuButton)
            || Input.GetKeyDown(rightMenuButton);
    }

    bool QuitPressed()
    {
        return Input.GetKeyDown(quitKey)
            || Input.GetKeyDown(leftQuitButton)
            || Input.GetKeyDown(rightQuitButton);
    }

    void HandleNavigation()
    {
        if (menuItemTexts == null || menuItemTexts.Length == 0)
        {
            return;
        }

        float vertical = 0f;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            vertical += 1f;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            vertical -= 1f;
        }

        float joystickAxis = Input.GetAxisRaw("Vertical");
        if (Mathf.Abs(joystickAxis) > Mathf.Abs(vertical))
        {
            vertical = joystickAxis;
        }

        if (Mathf.Abs(vertical) < 0.5f)
        {
            return;
        }

        if (Time.unscaledTime - lastNavTime < navInputCooldown)
        {
            return;
        }

        lastNavTime = Time.unscaledTime;
        int direction = vertical > 0f ? -1 : 1;
        selectedIndex = (selectedIndex + direction + menuItemTexts.Length) % menuItemTexts.Length;
        ApplyHighlight();
    }

    void ApplyHighlight()
    {
        if (menuItemTexts == null)
        {
            return;
        }

        for (int i = 0; i < menuItemTexts.Length; i++)
        {
            if (menuItemTexts[i] == null)
            {
                continue;
            }

            bool isSelected = i == selectedIndex;
            menuItemTexts[i].color = isSelected ? selectedColor : unselectedColor;
            menuItemTexts[i].transform.localScale = Vector3.one * (isSelected ? selectedScale : 1f);
        }
    }

    void ConfirmSelection()
    {
        if (menuItemTexts == null || menuItemTexts.Length == 0)
        {
            Continue();
            return;
        }

        if (selectedIndex == 1)
        {
            Quit();
        }
        else
        {
            Continue();
        }
    }

    public void Open()
    {
        isOpen = true;
        if (menuPanel != null)
        {
            menuPanel.SetActive(true);
        }

        if (pauseTimeWhenOpen)
        {
            Time.timeScale = 0f;
        }

        selectedIndex = 0;
        ApplyHighlight();
    }

    public void Continue()
    {
        isOpen = false;
        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
        }

        Time.timeScale = 1f;
    }

    public void Quit()
    {
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
