using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FruitType {Apple, Banana, Cherry, Kiwi, Melon, Orange, Pineapple, Strawberry}

public class Fruit : MonoBehaviour
{
    // This will helps us choose which fruit to be appear (Drop Down Menu)
    [SerializeField] private FruitType fruitType;
    [SerializeField] private GameObject pickupVfx;

    private GameManager gameManager;
    private Animator anim;

    private void Awake()
    {
        // Incase, it didn't get to component on the "Start", and since our animator is in the child section, we use "GetComponentInChildren"
        anim = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        gameManager = GameManager.instance;
        SetRandomLookIfNeeded();
    }

    private void SetRandomLookIfNeeded()
    {
        // Checks whether fruits to have random look or not from game manager.
        if (gameManager.fruitsAreRandom == false)
        {
            /// <summary>
            /// If this part runs, that means we want a single type of fruit to be appear in our game 
            /// Check <see cref="UpdateFruitVisuals"/> and <see cref="FruitType"/> for a further look.
            /// </summary>
            UpdateFruitVisuals();
            return;
        }

        // We write 8 even though we have only 7 variables, read the description of the "Range"
        int randomIndex = Random.Range(0, 8);
        anim.SetFloat("fruitIndex", randomIndex);
    }

    private void UpdateFruitVisuals() => anim.SetFloat("fruitIndex", (int)fruitType);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player != null)
        {
            gameManager.AddFruit();
            Destroy(gameObject);

            // This Fx will be appear on the fruit since we write this code on the Fruits.cs
            // Quaternion.identity: don't change the rotation.
            GameObject newFx = Instantiate(pickupVfx,transform.position,Quaternion.identity);

            // There is no need for writing a time in here because we used animation event, even if we write 5 in here it will destroy itself
            // once the animation is completed.
            // Destroy(newFx, .5f);
        }

    }
}
