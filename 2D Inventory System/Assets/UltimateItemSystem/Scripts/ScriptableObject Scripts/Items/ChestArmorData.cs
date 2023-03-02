using UnityEngine;

namespace UltimateItemSystem
{
    [CreateAssetMenu(fileName = "New Sword Object", menuName = "UltimateItemSystem/Item/Equipment/ChestArmor", order = 51)]
    public class ChestArmorData : UpgradeableItemData
    {
        [Header("ChestArmor Data")]
        public int armor;
    }
}