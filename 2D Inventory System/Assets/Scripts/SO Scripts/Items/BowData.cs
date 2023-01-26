using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Bow Object", menuName = "ScriptableObject/Item/Weapons/Bow Object", order = 51)]
public class BowData : ItemData
{
    public float attackSpeed;
    public int baseDamage;
    public int numOfUseRemaining;

}