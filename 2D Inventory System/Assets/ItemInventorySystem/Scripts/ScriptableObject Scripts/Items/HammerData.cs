using UnityEngine;

namespace DIVH_InventorySystem
{
    [CreateAssetMenu(fileName = "New Hammer Object", menuName = "ScriptableObject/Item/Tools/Hammer", order = 51)]
    public class HammerData : UpgradeableItemData
    {
        [Header("Hammer Properties")]
        public byte hammerPower;
        public int attackDamage;
    }
}
