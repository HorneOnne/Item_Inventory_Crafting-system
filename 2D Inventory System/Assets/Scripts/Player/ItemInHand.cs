using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace DIVH_InventorySystem
{
    public class ItemInHand : MonoBehaviour
    {
        [SerializeField] private ItemSlot itemSlot;

        [Header("References")]
        private Player player;
        private UIItemInHand uiItemInHand;
        private Item itemObject;


        [Header("Settings")]
        [Tooltip("Prevent using a selected item for the first time.")]
        private bool firstGet = true;

        #region Properties
        //public StoredType ItemGetFrom { get; private set; }
        public ItemSlotData ItemGetFrom { get; private set; }

        #endregion

        private void Start()
        {
            player = GetComponent<Player>();
            uiItemInHand = UIItemInHand.Instance;
        }


        public ItemData GetItemData()
        {
            return itemSlot.ItemData;
        }

        public void Set(ItemSlot takenSlot, int slotIndex = -1, StoredType storageType = StoredType.Another, bool forceUpdateUI = false, Transform parent = null)
        {
            itemSlot = new ItemSlot(takenSlot);
            ItemGetFrom = new ItemSlotData
            {
                slotIndex = slotIndex,
                slotStoredType = storageType
            };

            firstGet = true;
            EventManager.ItemInHandChanged();

            if (forceUpdateUI)
            {
                uiItemInHand.UpdateItemInHandUI(parent);
            }
        }


        public void Swap(ref List<ItemSlot> inventory, int slotIndex, StoredType storageType = StoredType.Another, bool forceUpdateUI = false, Transform parent = null)
        {
            var inHandSlotTemp = new ItemSlot(this.itemSlot);
            var invSlotChoosen = inventory[slotIndex];

            this.itemSlot = new ItemSlot(invSlotChoosen);
            inventory[slotIndex] = new ItemSlot(inHandSlotTemp);

            ItemGetFrom = new ItemSlotData
            {
                slotIndex = slotIndex,
                slotStoredType = storageType
            };


            firstGet = true;
            EventManager.ItemInHandChanged();

            if (forceUpdateUI)
            {
                uiItemInHand.UpdateItemInHandUI(parent);
            }
        }

        public void Swap(ref ItemSlot[] inventory, int slotIndex, StoredType storageType = StoredType.Another, bool forceUpdateUI = false, Transform parent = null)
        {
            var inHandSlotTemp = new ItemSlot(this.itemSlot);
            var invSlotChoosen = inventory[slotIndex];

            this.itemSlot = new ItemSlot(invSlotChoosen);
            inventory[slotIndex] = new ItemSlot(inHandSlotTemp);

            ItemGetFrom = new ItemSlotData
            {
                slotIndex = slotIndex,
                slotStoredType = storageType
            };


            firstGet = true;
            EventManager.ItemInHandChanged();

            if (forceUpdateUI)
            {
                uiItemInHand.UpdateItemInHandUI(parent);
            }
        }




        public void SplitItemSlotQuantityInInventoryAt(ref List<ItemSlot> inventory, int slotIndex)
        {
            int itemQuantity = inventory[slotIndex].ItemQuantity;
            if (itemQuantity > 1)
            {
                int splitItemQuantity = itemQuantity / 2;
                inventory[slotIndex].SetItemQuantity(itemQuantity - splitItemQuantity);

                var chosenSlot = new ItemSlot(inventory[slotIndex]);
                chosenSlot.SetItemQuantity(splitItemQuantity);
                Set(chosenSlot, slotIndex, StoredType.PlayerInventory, true);
            }
            else
            {
                Swap(ref inventory, slotIndex, StoredType.PlayerInventory, true);
            }
        }

        public void SplitItemSlotQuantityInInventoryAt(ref ItemSlot[] inventory, int slotIndex)
        {
            int itemQuantity = inventory[slotIndex].ItemQuantity;
            if (itemQuantity > 1)
            {
                int splitItemQuantity = itemQuantity / 2;
                inventory[slotIndex].SetItemQuantity(itemQuantity - splitItemQuantity);

                var chosenSlot = new ItemSlot(inventory[slotIndex]);
                chosenSlot.SetItemQuantity(splitItemQuantity);
                Set(chosenSlot, slotIndex, StoredType.PlayerInventory, true);
            }
            else
            {
                Swap(ref inventory, slotIndex, StoredType.PlayerInventory, true);
            }
        }


        public void SetItemObject(Item item)
        {
            itemObject = item;
        }

        public Item GetItemObject()
        {
            return itemObject;
        }


        public bool PickupItem(ref ItemSlot itemContainerSlot)
        {
            bool canPickupItem;


            if (HasItemData() == false)
            {
                itemSlot = new ItemSlot(itemContainerSlot);
                itemContainerSlot.ClearSlot();
                uiItemInHand.UpdateItemInHandUI();
                canPickupItem = true;
            }
            else
            {
                if (itemSlot.ItemData.Equals(itemContainerSlot.ItemData))
                {
                    if (itemSlot.TryAddItem(itemContainerSlot) == true)
                    {
                        itemContainerSlot = itemSlot.AddItemsFromAnotherSlot(itemContainerSlot);
                        uiItemInHand.UpdateItemInHandUI();
                        canPickupItem = true;
                    }
                    else
                    {
                        //Debug.Log("The item quantity > item maxquantity");
                        canPickupItem = false;
                    }
                }
                else
                {
                    //Debug.Log("Not same");
                    canPickupItem = false;
                }
            }

            EventManager.ItemInHandChanged();
            return canPickupItem;
        }

        public bool HasItemData()
        {
            if (itemSlot == null)
                return false;

            return itemSlot.HasItem();
        }


        public bool HasItemObject()
        {
            return itemObject != null;
        }


        public ItemSlot GetSlot() => itemSlot;

        public void ClearSlot()
        {
            itemSlot.ClearSlot();
            ItemGetFrom = new ItemSlotData
            {
                slotStoredType = StoredType.Another,
                slotIndex = -1
            };

            EventManager.ItemInHandChanged();
        }


        public bool UseItem()
        {
            if (HasItemData() == false || HasItemObject() == false) return false;
            if (IsMouseOverUI() == true) return false;

            if (firstGet)
            {
                firstGet = false;
                return false;
            }

            return itemObject.Use(player);
        }


        /// <summary>
        /// substract 1 quantity of item in hand.
        /// </summary>
        public void RemoveItem()
        {
            itemSlot.RemoveItem();
            uiItemInHand.UpdateItemInHandUI();

            EventManager.ItemInHandChanged();
        }


        private bool IsMouseOverUI()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // mouse click is on a UI element.
                return true;
            }
            else
            {
                // mouse click is not on a UI element
                return false;
            }
        }
    }
}

