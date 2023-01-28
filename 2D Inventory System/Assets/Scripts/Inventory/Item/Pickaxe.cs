using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickaxe : Item
{
    public override void Use(PlayerController player)
    {
        Debug.Log("Use pickaxe");
    }
}
