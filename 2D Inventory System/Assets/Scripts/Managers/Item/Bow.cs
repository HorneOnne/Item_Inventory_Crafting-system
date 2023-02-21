using UnityEngine;

namespace DIVH_InventorySystem
{
    public class Bow : Item
    {
        [Header("References")]
        private PlayerInventory playerInventory;

        [Header("Bow Properties")]
        private BowData bowData;
        [SerializeField] private bool consumeArrow;


        private ArrowData arrowData;
        private int? arrowSlotIndex;
        private ItemSlot arrowSlotInPlayerInventory;
        private GameObject arrowProjectilePrefab;
        private ArrowProjectile_001 arrowProjectileObject;

        // Cached

        protected override void Start()
        {
            base.Start();
            base.SetOffsetPosition();

            bowData = (BowData)this.ItemData;
            arrowProjectilePrefab = ItemDataManager.Instance.GetItemPrefab("ArrowProjectile_001");

        }





        public override bool Use(Player player)
        {
            playerInventory = player.PlayerInventory;
            arrowSlotIndex = playerInventory.FindArrowSlotIndex();

            if (arrowSlotIndex == null) return false;
            if (arrowProjectilePrefab == null) return false;

            UseType02();


            ConsumeArrow();
            return true;
        }

        private void UseType01()
        {
            arrowProjectileObject = ArrowSpawner.Instance.Pool.Get().GetComponent<ArrowProjectile_001>();
            arrowProjectileObject.transform.position = transform.position;
            arrowProjectileObject.transform.rotation = transform.rotation;

            arrowSlotInPlayerInventory = playerInventory.inventory[(int)arrowSlotIndex];
            arrowData = (ArrowData)arrowSlotInPlayerInventory.ItemData;
            arrowProjectileObject.SetData(arrowData);
            arrowProjectileObject.Shoot(bowData, arrowData);
        }

        private void UseType02()
        {
            var arrowProjectileObject01 = ArrowSpawner.Instance.Pool.Get().GetComponent<ArrowProjectile_001>();
            var arrowProjectileObject02 = ArrowSpawner.Instance.Pool.Get().GetComponent<ArrowProjectile_001>();
            arrowProjectileObject01.transform.position = transform.position;
            arrowProjectileObject01.transform.rotation = transform.rotation;
            arrowProjectileObject02.transform.position = transform.position;
            arrowProjectileObject02.transform.rotation = transform.rotation;

            Debug.Log("Fix here.");


            arrowSlotInPlayerInventory = playerInventory.inventory[(int)arrowSlotIndex];
            arrowData = (ArrowData)arrowSlotInPlayerInventory.ItemData;
            arrowProjectileObject01.SetData(arrowData);
            arrowProjectileObject01.Shoot(bowData, arrowData);
        }


        private void ConsumeArrow()
        {
            if (consumeArrow)
            {
                arrowSlotInPlayerInventory.RemoveItem();
                UIPlayerInventory.Instance.UpdateInventoryUIAt((int)arrowSlotIndex);
            }
        }
    }
}