using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitPickup_VFX : MonoBehaviour
{
    // This method is used for making an animation event, you can see from "Pickup_VFX" animation.
    public void DestroyMe() => Destroy(gameObject);
}
