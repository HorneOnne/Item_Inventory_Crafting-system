using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Bow Object", menuName = "ScriptableObject/Item/Weapons/Bow", order = 51)]
public class BowData : UpgradeableItemData
{
    [Header("Bow Properties")]
    public int baseAttackDamage;    // used for add damage with arrow
    public float releaseSpeed;  // Affect the arrow speed
}

