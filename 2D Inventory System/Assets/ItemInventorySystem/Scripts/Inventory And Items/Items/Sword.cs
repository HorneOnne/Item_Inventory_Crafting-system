using UnityEngine;

namespace DIVH_InventorySystem
{
    public class Sword : Item
    {
        private GameObject swordProjectilePrefab;
        private GameObject swordProjectileObject;
        private SwordData swordData;


        protected override void Start()
        {
            base.Start();
            swordProjectilePrefab = GameDataManager.Instance.GetProjectilePrefab("PP_SwordProjectile_001");
            swordData = ((SwordData)ItemData);
        }


        public override bool Use(Player player)
        {           
            switch(swordData.useType)
            {
                case 1:
                    UseType01(player);
                    break;
                case 2:
                    UseType02(player);
                    break;
                default:
                    Debug.LogWarning($"Not found useType {swordData.useType} in SwordData.");
                    break;
            }

            return true;
        }

        private void UseType01(Player player)
        {
            swordProjectilePrefab = GameDataManager.Instance.GetProjectilePrefab("PP_SwordProjectile_001");
            swordProjectileObject = Instantiate(swordProjectilePrefab, transform.position, transform.rotation, player.transform);
            swordProjectileObject.transform.localScale = new Vector3(4, 4, 1);
            swordProjectileObject.SetActive(true);
            swordProjectileObject.GetComponent<Projectile>().SetData(this.ItemSlot.ItemData);

        }

        private void UseType02(Player player)
        {
            swordProjectilePrefab = GameDataManager.Instance.GetProjectilePrefab("PP_SwordProjectile_001");
            swordProjectileObject = Instantiate(swordProjectilePrefab, transform.position, transform.rotation, player.transform);
            swordProjectileObject.transform.localScale = new Vector3(4, 4, 1);
            swordProjectileObject.SetActive(true);
            swordProjectileObject.GetComponent<Projectile>().SetData(this.ItemSlot.ItemData);

            var swordProjectile002 = SworldProjectile002Spawner.Instance.Pool.Get().GetComponent<SwordProjectile_002>();
            swordProjectile002.SetData(swordData);
            swordProjectile002.Shoot(this.transform.position);
            Utilities.RotateObjectTowardMouse2D(swordProjectile002.transform, -45f);
        }

    }
}