using UnityEngine;

public class DrinkInteraction : MonoBehaviour
{
    public GameObject promptUI;
    public DrinkAnimation drinkAnimation;

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

        if (playerInRange && (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Submit")))
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
}