using UnityEngine;

namespace DIVH_InventorySystem
{
    public abstract class Item : MonoBehaviour, IDroppable, ICollectible, IUseable
    {
        #region Properties
        [field: SerializeField]
        public ItemData ItemData { get; private set; }
        [field: SerializeField]
        public ItemSlot ItemSlot { get; private set; }
        protected GameObject Model { get; private set; }
        #endregion


        [Header("References")]
        public SpriteRenderer spriteRenderer;

        [Header("Item Properties")]
        [Tooltip("Indicate this item can be shown when held in hand.")]
        public bool showIconWhenHoldByHand;


        protected virtual void Start()
        {
            LoadComponents();         
        }

 

        private void LoadComponents()
        {
            Model = GetComponentInChildren<SpriteRenderer>().gameObject;
            spriteRenderer = Model.GetComponent<SpriteRenderer>();
        }


        public void SetData(ItemSlot itemSlot)
        {
            this.ItemSlot = itemSlot;
            this.ItemData = itemSlot.ItemData;
            UpdateData();
        }


        public void UpdateData()
        {
            if (spriteRenderer == null)
                LoadComponents();


            spriteRenderer.sprite = ItemData.icon;
           
        }


        public virtual void Collect(Player player)
        {

        }

    
        public virtual void Drop(Player player, Vector2 position, Vector3 rotation, bool forceDestroyItemObject = false)
        {
            var itemSlotDrop = new ItemSlot(ItemSlot);
            var itemPrefab = GameDataManager.Instance.GetItemPrefab($"IP_DropItem");
            if (itemPrefab == null) 
            {
                throw new System.Exception($"Not found prefab name P_DropItem in GameDataManager.cs");
            }
            var itemObject = Instantiate(itemPrefab, position, Quaternion.Euler(rotation), SaveManager.Instance.itemContainerParent);
            itemObject.GetComponent<DropItem>().Set(itemSlotDrop);
            itemObject.transform.localScale = new Vector3(2, 2, 1);

            if (forceDestroyItemObject)
            {
                Destroy(this.gameObject);
            }
        }



        public virtual bool Use(Player player)
        {
            return true;
        }
    }
}