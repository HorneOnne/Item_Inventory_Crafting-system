using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Pickaxe Object", menuName = "ScriptableObject/Item/Tools/Axe", order = 51)]
public class AxeData : UpgradeableItemData
{
    [Header("Pickaxe Properties")]
    public byte axePower;
    public int attackDamage;
}
