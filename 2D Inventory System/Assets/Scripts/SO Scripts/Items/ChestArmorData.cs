using UnityEngine;

[CreateAssetMenu(fileName = "New Sword Object", menuName = "ScriptableObject/Item/Equipment/ChestArmor", order = 51)]
public class ChestArmorData : UpgradeableItemData
{
    [Header("ChestArmor Data")]
    public int armor;
}
