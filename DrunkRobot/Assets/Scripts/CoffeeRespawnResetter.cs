using UnityEngine;

public class CoffeeRespawnResetter : MonoBehaviour
{
    private Transform coffee;
    private Transform originalParent;
    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;
    private Vector3 originalLocalScale;
    private DrinkInteraction interaction;

    public void Configure(Transform coffeeTransform, DrinkInteraction drinkInteraction)
    {
        coffee = coffeeTransform;
        interaction = drinkInteraction;
        originalParent = coffee.parent;
        originalLocalPosition = coffee.localPosition;
        originalLocalRotation = coffee.localRotation;
        originalLocalScale = coffee.localScale;
    }

    void OnEnable()
    {
        PlayerRespawn.Respawned += ResetCoffee;
    }

    void OnDisable()
    {
        PlayerRespawn.Respawned -= ResetCoffee;
    }

    void ResetCoffee()
    {
        if (coffee == null)
        {
            return;
        }

        coffee.SetParent(originalParent, false);
        coffee.localPosition = originalLocalPosition;
        coffee.localRotation = originalLocalRotation;
        coffee.localScale = originalLocalScale;
        coffee.gameObject.SetActive(true);

        if (interaction != null)
        {
            interaction.ResetInteraction();
        }
    }
}
