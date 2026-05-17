using UnityEngine;

public class CoffeeDrinkInteraction : MonoBehaviour
{
    public GameObject promptUI;
    public Transform rightArm;
    public MouseLook mouseLook;
    public PlayerMovement playerMovement;
    public float interactionRadius = 1.2f;
    public float soberDuration = 5f;
    public float speedBoostMultiplier = 1.5f;

    void Awake()
    {
        EnsureReferences();
        EnsureTrigger();
        EnsureDrinkInteraction();
    }

    void EnsureReferences()
    {
        if (playerMovement == null && mouseLook != null && mouseLook.playerBody != null)
        {
            playerMovement = mouseLook.playerBody.GetComponent<PlayerMovement>();
        }

        if (playerMovement == null)
        {
            playerMovement = FindFirstObjectByType<PlayerMovement>();
        }
    }

    void EnsureTrigger()
    {
        SphereCollider trigger = GetComponent<SphereCollider>();
        if (trigger == null)
        {
            trigger = gameObject.AddComponent<SphereCollider>();
        }

        trigger.isTrigger = true;
        trigger.radius = interactionRadius;
    }

    void EnsureDrinkInteraction()
    {
        DrinkInteraction interaction = GetComponent<DrinkInteraction>();
        if (interaction == null)
        {
            interaction = gameObject.AddComponent<DrinkInteraction>();
        }

        DrinkAnimation animationRunner = CreateAnimationRunner(interaction);

        interaction.promptUI = promptUI;
        interaction.drinkAnimation = animationRunner;
    }

    DrinkAnimation CreateAnimationRunner(DrinkInteraction interaction)
    {
        GameObject runnerObject = new GameObject($"{name}_DrinkAnimationRunner");
        runnerObject.transform.SetParent(transform.root, false);

        DrinkAnimation animationRunner = runnerObject.AddComponent<DrinkAnimation>();
        animationRunner.rightArm = rightArm;
        animationRunner.bottle = transform;
        animationRunner.interaction = interaction;
        animationRunner.mouseLook = mouseLook;
        animationRunner.playerMovement = playerMovement;
        animationRunner.effectMode = DrinkAnimation.DrinkEffectMode.SuppressDrunk;
        animationRunner.soberDuration = soberDuration;
        animationRunner.speedBoostMultiplier = speedBoostMultiplier;
        animationRunner.destroyAfterDrink = false;

        CoffeeRespawnResetter resetter = runnerObject.AddComponent<CoffeeRespawnResetter>();
        resetter.Configure(transform, interaction);

        return animationRunner;
    }
}
