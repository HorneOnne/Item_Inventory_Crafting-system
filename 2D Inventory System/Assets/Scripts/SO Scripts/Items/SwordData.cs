using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sword Object", menuName = "ScriptableObject/Item/Weapons/Sword", order = 51)]
public class SwordData : ItemData
{
    public int damage;
    public int numOfUseRemaining;

    [Header("Upgrade References")]
    public SwordData upgradeSwordData;
}
