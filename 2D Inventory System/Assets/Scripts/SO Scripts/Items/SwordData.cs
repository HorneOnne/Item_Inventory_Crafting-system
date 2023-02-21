using UnityEngine;

namespace DIVH_InventorySystem
{
    [CreateAssetMenu(fileName = "New Sword Object", menuName = "ScriptableObject/Item/Weapons/Sword", order = 51)]
    public class SwordData : UpgradeableItemData
    {
        [Header("Sword Properties")]
        public int damage;
        public float swingSwordIncreaseSize;    // Used to increase swing sword size
    }
}
