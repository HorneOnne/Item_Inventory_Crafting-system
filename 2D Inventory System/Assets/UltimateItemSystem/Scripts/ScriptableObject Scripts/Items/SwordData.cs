using UnityEngine;

namespace UltimateItemSystem
{
    [CreateAssetMenu(fileName = "New Sword Object", menuName = "UltimateItemSystem/Item/Weapons/Sword", order = 51)]
    public class SwordData : UpgradeableItemData
    {
        [Header("SWORD PROPERTIES")]
        public int damage;
        public float swingSwordIncreaseSize;    // Used to increase swing sword size

        [Header("UPGRADE PROPERTIES")]
        public int useType;
    }
}
