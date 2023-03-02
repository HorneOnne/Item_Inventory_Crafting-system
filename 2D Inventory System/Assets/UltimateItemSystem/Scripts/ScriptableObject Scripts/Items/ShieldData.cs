using UnityEngine;

namespace UltimateItemSystem
{
    [CreateAssetMenu(fileName = "New Sword Object", menuName = "UltimateItemSystem/Item/Equipment/Shield", order = 51)]
    public class ShieldData : UpgradeableItemData
    {
        [Header("Shield Data")]
        public int armor;
    }
}