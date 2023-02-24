using UnityEngine;

namespace DIVH_InventorySystem
{
    [CreateAssetMenu(fileName = "Boomerang", menuName = "ScriptableObject/Item/Weapons/Boomerang", order = 51)]
    public class BoomerangData : UpgradeableItemData
    {
        [Header("BOOMERANG SETTINGS")]
        public int damage;
        public float releaseSpeed;
        public float timeToReturn;  // Time the boomerang back to player (seconds).
        public float rotateSpeed;  // Only visualization effect.

        [Header("UPGRADE SETTINGS")]
        public int attackType;
    }
}
