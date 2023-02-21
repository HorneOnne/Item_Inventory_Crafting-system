using UnityEngine;

namespace DIVH_InventorySystem
{
    [CreateAssetMenu(fileName = "Arrow", menuName = "ScriptableObject/Item/Projectiles/Arrow", order = 51)]
    public class ArrowData : ItemData
    {
        [Header("Bow Properties")]
        public int damage;
        [Tooltip("Index for getting particles frames in ProjectileParticleDataManager")]
        public int particle;
    }
}