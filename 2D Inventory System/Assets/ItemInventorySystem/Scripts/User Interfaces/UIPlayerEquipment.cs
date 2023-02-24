using UnityEngine;
using UnityEngine.EventSystems;

namespace DIVH_InventorySystem
{
    public class UIPlayerEquipment : Singleton<UIPlayerEquipment>
    {
        [Header("References")]
        public Player player;
        private ItemInHand itemInHand;
        private PlayerEquipment playerEquipment;


        public UIEquipSlot uiHelmSlot;
        public UIEquipSlot uiChestSlot;
        public UIEquipSlot uiShieldSlot;



        private void Start()
        {
            playerEquipment = player.PlayerEquipment;
            itemInHand = player.ItemInHand;

            AddUIItemSLotEvent(uiHelmSlot);
            AddUIItemSLotEvent(uiChestSlot);
            AddUIItemSLotEvent(uiShieldSlot);
        }


        private void AddUIItemSLotEvent(UIEquipSlot slot)
        {
            GameObject slotObject = slot.gameObject;

            slot.Set(null);
            Utilities.AddEvent(slotObject, EventTriggerType.PointerDown, (baseEvent) => OnClick(baseEvent, slotObject));
        }

        #region UPDATE CRAFTINGTABLE DISPLAY UI REGION.
        public void UpdateEquipmentUI()
        {
            UpdateHelmEquipmentUI();
            UpdateChestEquipmentUI();
            UpdateShieldEquipmentUI();
        }

        private void UpdateHelmEquipmentUI()
        {
            if (player.PlayerEquipment.Helm.HasItem())
                uiHelmSlot.Set(player.PlayerEquipment.Helm.GetItemIcon());
            else
                uiHelmSlot.SetDefault();
        }

        private void UpdateChestEquipmentUI()
        {
            if (player.PlayerEquipment.Chest.HasItem())
                uiChestSlot.Set(player.PlayerEquipment.Chest.GetItemIcon());
            else
                uiChestSlot.SetDefault();
        }

        private void UpdateShieldEquipmentUI()
        {
            if (player.PlayerEquipment.Shield.HasItem())
                uiShieldSlot.Set(player.PlayerEquipment.Shield.GetItemIcon());
            else
                uiShieldSlot.SetDefault();
        }

        #endregion UPDATE CRAFTINGTABLE DISPLAY UI REGION.



        #region Interactive Events
        public void OnClick(BaseEventData baseEvent, GameObject clickedObject)
        {
            PointerEventData pointerEventData = (PointerEventData)baseEvent;

            ItemType equipmentType = GetEquipmentType(clickedObject.GetComponent<UIEquipSlot>());
            ItemSlot equipmentSlot = player.PlayerEquipment.GetEquipmentSlot(equipmentType);

            if (equipmentType == ItemType.Null)
                return;


            if (itemInHand.HasItemData() == false)
            {
                if (equipmentSlot.HasItem() == false)
                {
                    //Debug.Log("HAND: EMPTY \t SLOT: EMPTY");
                }
                else
                {
                    Debug.Log("HAND: EMPTY \t SLOT: HAS ITEM");
                    itemInHand.SetItem(equipmentSlot, slotIndex: -1, storageType: StoredType.Another, true);
                    equipmentSlot.ClearSlot();
                }
            }
            else
            {
                if (equipmentSlot.HasItem() == false)
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");           
                    
                }
                else
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");
                }
                Equip(equipmentSlot, equipmentType);
            }
            UpdateEquipmentUI();
            EventManager.TriggerPlayerEquipmentChangedEvent();
        }



        #endregion

        #region LOGIC HANDLER
        // LOGIC 
        // ===================================================================
        private ItemType GetEquipmentType(UIEquipSlot equipmentSlot)
        {
            if (equipmentSlot == null)
                return ItemType.Null;

            return equipmentSlot.equipmentType;
        }


        /// <summary>
        /// This Method Swap InHandSlot and Slot in inventory at index
        /// </summary>
        /// <param name="index">index for itemSlotList</param>
        private void Equip(ItemSlot equipmentSlot, ItemType equipmentType)
        {
            if (equipmentSlot.HasItem())
            {
                var copyEquipmentSlot = new ItemSlot(equipmentSlot);

                bool canEquip = playerEquipment.Equip(equipmentType, itemInHand.GetSlot());

                if (canEquip)
                {
                    itemInHand.SetItem(copyEquipmentSlot, slotIndex: -1, storageType: StoredType.Another, true);
                }
            }
            else
            {
                bool canEquip = playerEquipment.Equip(equipmentType, itemInHand.GetSlot());

                if (canEquip)
                    itemInHand.ClearSlot();
            }
        }



        #endregion LOCGIC HANDLER
    }
}
