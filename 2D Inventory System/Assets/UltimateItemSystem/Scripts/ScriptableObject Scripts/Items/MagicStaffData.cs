using UnityEngine;

namespace UltimateItemSystem
{
    [CreateAssetMenu(fileName = "MagicStaff", menuName = "UltimateItemSystem/Item/Weapons/MagicStaff", order = 51)]
    public class MagicStaffData : ItemData
    {
        [Header("Magic Staff Properties")]
        public Sprite projectile;
        public float releaseProjectileSpeed;
        public int damage;
        [Tooltip("Index for getting particles frames in ProjectileParticleDataManager")]
        public int particle;
    }
}
