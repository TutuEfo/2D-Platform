using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
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
        if (gameManager.fruitsHaveRandomLook == false)
        {
            return;
        }

        // We write 8 even though we have only 7 variables, read the description of the "Range"
        int randomIndex = Random.Range(0, 8);
        anim.SetFloat("fruitIndex", randomIndex);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player != null)
        {
            gameManager.AddFruit();
            Destroy(gameObject);
        }

    }
}
