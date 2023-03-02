using UnityEngine;


namespace UltimateItemSystem
{
    [CreateAssetMenu(fileName = "New Pickaxe Object", menuName = "UltimateItemSystem/Item/Tools/Axe", order = 51)]
    public class AxeData : UpgradeableItemData
    {
        [Header("Pickaxe Properties")]
        public byte axePower;
        public int attackDamage;
    }
}
