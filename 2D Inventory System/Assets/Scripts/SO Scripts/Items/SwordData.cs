using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sword Object", menuName = "ScriptableObject/Item/Weapons/Sword", order = 51)]
public class SwordData : UpgradeableItemData
{
    [Header("Sword Properties")]
    public int damage;
    public int numOfUseRemaining;
}
