using UnityEngine;

public class DrinkInteraction : MonoBehaviour
{
    public GameObject promptUI;
    public DrinkAnimation drinkAnimation;
    public KeyCode keyboardInteractKey = KeyCode.E;
    public KeyCode leftBlueButton = KeyCode.Joystick1Button0;
    public KeyCode rightBlueButton = KeyCode.Joystick2Button0;

    private bool playerInRange = false;
    private bool hasDrunk = false;

    void Start()
    {
        if (promptUI != null)
        {
            promptUI.SetActive(false);
        }
    }

    void Update()
    {
        if (hasDrunk) return;

        if (playerInRange && IsInteractPressed())
        {
            if (drinkAnimation != null)
            {
                drinkAnimation.PlayDrink();
            }

            hasDrunk = true;
            playerInRange = false;

            if (promptUI != null)
            {
                promptUI.SetActive(false);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasDrunk) return;

        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (promptUI != null)
            {
                promptUI.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (promptUI != null)
            {
                promptUI.SetActive(false);
            }
        }
    }

    public void DisableInteraction()
    {
        playerInRange = false;
        hasDrunk = true;

        if (promptUI != null)
        {
            promptUI.SetActive(false);
        }
    }

    public void ResetInteraction()
    {
        playerInRange = false;
        hasDrunk = false;

        if (promptUI != null)
        {
            promptUI.SetActive(false);
        }
    }

    bool IsInteractPressed()
    {
        return Input.GetKeyDown(keyboardInteractKey)
            || Input.GetKeyDown(leftBlueButton)
            || Input.GetKeyDown(rightBlueButton)
            || Input.GetButtonDown("Submit");
    }
}
