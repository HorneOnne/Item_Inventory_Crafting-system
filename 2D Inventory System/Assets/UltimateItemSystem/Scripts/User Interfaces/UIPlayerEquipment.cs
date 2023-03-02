using UnityEngine;
using UnityEngine.EventSystems;

namespace UltimateItemSystem
{
    /// <summary>
    /// Represents the UI for displaying the player's equipment.
    /// </summary>
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


        /// <summary>
        /// Adds the UI item slot event.
        /// </summary>
        /// <param name="slot">The slot to add the event to.</param>
        private void AddUIItemSLotEvent(UIEquipSlot slot)
        {
            GameObject slotObject = slot.gameObject;

            slot.Set(null);
            Utilities.AddEvent(slotObject, EventTriggerType.PointerDown, (baseEvent) => OnClick(baseEvent, slotObject));
        }

        #region UPDATE DISPLAY UI REGION.
        /// <summary>
        /// Updates the equipment UI.
        /// </summary>
        public void UpdateEquipmentUI()
        {
            UpdateHelmEquipmentUI();
            UpdateChestEquipmentUI();
            UpdateShieldEquipmentUI();
        }


        /// <summary>
        /// Updates the helm equipment UI.
        /// </summary>
        private void UpdateHelmEquipmentUI()
        {
            if (player.PlayerEquipment.Helm.HasItem())
                uiHelmSlot.Set(player.PlayerEquipment.Helm.GetItemIcon());
            else
                uiHelmSlot.SetDefault();
        }

        /// <summary>
        /// Updates the chest equipment UI.
        /// </summary>
        private void UpdateChestEquipmentUI()
        {
            if (player.PlayerEquipment.Chest.HasItem())
                uiChestSlot.Set(player.PlayerEquipment.Chest.GetItemIcon());
            else
                uiChestSlot.SetDefault();
        }

        /// <summary>
        /// Updates the shield equipment UI.
        /// </summary>
        private void UpdateShieldEquipmentUI()
        {
            if (player.PlayerEquipment.Shield.HasItem())
                uiShieldSlot.Set(player.PlayerEquipment.Shield.GetItemIcon());
            else
                uiShieldSlot.SetDefault();
        }

        #endregion UPDATE DISPLAY UI REGION.



        #region Interactive Events

        /// <summary>
        /// Handles the pointer down event.
        /// </summary>
        /// <param name="baseEvent">The base event.</param>
        /// <param name="clickedObject">The clicked object.</param>
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
                    //Debug.Log("HAND: EMPTY \t SLOT: HAS ITEM");
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
        
        /// <summary>
        /// Returns the equipment type of the given UI equipment slot.
        /// </summary>
        /// <param name="equipmentSlot">The UI equipment slot to get the equipment type from.</param>
        /// <returns>The equipment type of the given UI equipment slot.</returns>
        private ItemType GetEquipmentType(UIEquipSlot equipmentSlot)
        {
            if (equipmentSlot == null)
                return ItemType.Null;

            return equipmentSlot.equipmentType;
        }


        /// <summary>
        /// Swaps the item in the in-hand slot with the item in the given equipment slot.
        /// </summary>
        /// <param name="equipmentSlot">The equipment slot to swap with.</param>
        /// <param name="equipmentType">The equipment type of the given equipment slot.</param>
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
