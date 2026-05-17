using UnityEngine;
using UnityEngine.SceneManagement;

public class RedButtonInteraction : MonoBehaviour
{
    public GameObject pressEUI;
    public GameObject youHaveToBeDrunkUI;
    public KeyCode keyboardInteractKey = KeyCode.E;
    public KeyCode leftBlueButton = KeyCode.Joystick1Button0;
    public KeyCode rightBlueButton = KeyCode.Joystick2Button0;

    private MouseLook mouseLook;
    private bool playerInRange = false;

    void Start()
    {
        mouseLook = FindFirstObjectByType<MouseLook>();

        if (pressEUI != null)
            pressEUI.SetActive(false);

        if (youHaveToBeDrunkUI != null)
            youHaveToBeDrunkUI.SetActive(false);
    }

    void Update()
    {
        if (!playerInRange) return;

        if (IsInteractPressed())
        {
            if (mouseLook != null && mouseLook.isDrunk)
            {
                SceneManager.LoadScene("Level1");
            }
            else
            {
                if (pressEUI != null)
                    pressEUI.SetActive(false);

                if (youHaveToBeDrunkUI != null)
                    youHaveToBeDrunkUI.SetActive(true);
            }
        }
    }

    bool IsInteractPressed()
    {
        return Input.GetKeyDown(keyboardInteractKey)
            || Input.GetKeyDown(leftBlueButton)
            || Input.GetKeyDown(rightBlueButton)
            || Input.GetButtonDown("Submit");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (pressEUI != null)
                pressEUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (pressEUI != null)
                pressEUI.SetActive(false);

            if (youHaveToBeDrunkUI != null)
                youHaveToBeDrunkUI.SetActive(false);
        }
    }
}
