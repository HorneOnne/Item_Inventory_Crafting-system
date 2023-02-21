using UnityEngine;

namespace DIVH_InventorySystem
{
    [CreateAssetMenu(fileName = "New Sword Object", menuName = "ScriptableObject/Item/Equipment/Helm", order = 51)]
    public class HelmData : UpgradeableItemData
    {
        [Header("Helm Properties")]
        public int armor;
    }
}
