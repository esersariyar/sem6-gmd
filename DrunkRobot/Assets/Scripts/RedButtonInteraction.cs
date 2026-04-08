using UnityEngine;
using UnityEngine.SceneManagement;

public class RedButtonInteraction : MonoBehaviour
{
    public GameObject pressEUI;
    public GameObject youHaveToBeDrunkUI;

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

        if (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Submit"))
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
