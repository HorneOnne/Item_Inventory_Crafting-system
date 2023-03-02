using UnityEngine;

namespace UltimateItemSystem
{
    public class MagicStaff : Item
    {
        private GameObject magicStaffProjectileObject;
        private MagicStaffData magicStaffData;


        [field: SerializeField]
        public bool UseGravity { get; set; }


        protected override void Start()
        {
            base.Start();
            magicStaffData = (MagicStaffData)ItemData;
        }


        public override bool Use(Player player)
        {
            magicStaffProjectileObject = MagicStaffProjectileSpawner.Instance.Pool.Get();
            magicStaffProjectileObject.transform.position = transform.position;
            magicStaffProjectileObject.transform.rotation = transform.rotation;

            magicStaffProjectileObject.transform.localScale = new Vector3(1, 1, 1);
            magicStaffProjectileObject.GetComponent<MagicStaffProjectile_001>().SetData(this.ItemSlot.ItemData, magicStaffData.projectile, UseGravity);
            magicStaffProjectileObject.GetComponent<MagicStaffProjectile_001>().Shoot();


            return true;
        }
    }
}