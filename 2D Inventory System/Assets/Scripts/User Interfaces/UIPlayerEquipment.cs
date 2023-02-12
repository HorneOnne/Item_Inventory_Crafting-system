using MyGame.Ultilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPlayerEquipment : Singleton<UIPlayerEquipment>
{
    [Header("References")]
    public Player player;
    private ItemInHand itemInHand;
    private UIItemInHand uiItemInHand;
    private PlayerEquipment playerEquipment;


    public UIEquipSlot uiHelmSlot;
    public UIEquipSlot uiChestSlot;
    public UIEquipSlot uiShieldSlot;



    private void Start()
    {
        playerEquipment = player.PlayerEquipment;
        itemInHand = player.ItemInHand;
        uiItemInHand = UIItemInHand.Instance;


        AddUIItemSLotEvent(uiHelmSlot);
        AddUIItemSLotEvent(uiChestSlot);
        AddUIItemSLotEvent(uiShieldSlot);

        UpdateEquipmentUI();
    }


    private void AddUIItemSLotEvent(UIEquipSlot slot)
    {
        GameObject slotObject = slot.gameObject;

        slot.Set(null);
        Utilities.AddEvent(slotObject, EventTriggerType.PointerDown, (baseEvent) => OnClick(baseEvent, slotObject));  
    }

    #region UPDATE CRAFTINGTABLE DISPLAY UI REGION.
    private void UpdateEquipmentUI()
    {
        UpdateHelmEquipmentUI();
        UpdateChestEquipmentUI();
        UpdateShieldEquipmentUI();
    }

    private void UpdateHelmEquipmentUI()
    {
        if (player.PlayerEquipment.helmSlot.HasItem())
            uiHelmSlot.Set(player.PlayerEquipment.helmSlot.ItemData.icon);
        else
            uiHelmSlot.SetDefault();
    }

    private void UpdateChestEquipmentUI()
    {
        if (player.PlayerEquipment.chestSlot.HasItem())
            uiChestSlot.Set(player.PlayerEquipment.chestSlot.ItemData.icon);
        else
            uiChestSlot.SetDefault();
    }

    private void UpdateShieldEquipmentUI()
    {
        if (player.PlayerEquipment.shieldSlot.HasItem())
            uiShieldSlot.Set(player.PlayerEquipment.shieldSlot.ItemData.icon);
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
                //Debug.Log("HAND: EMPTY \t SLOT: HAS ITEM");
                GetItemSlot(equipmentSlot);
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
                itemInHand.Set(copyEquipmentSlot, StoredType.Another);
                uiItemInHand.DisplayItemInHand();
                
            }                
        }
        else
        {
            bool canEquip = playerEquipment.Equip(equipmentType, itemInHand.GetSlot());

            if (canEquip)
                itemInHand.ClearSlot();
        }
    }




    /// <summary>
    /// Method remove itemSlot in itemSlotList and put it in "itemInHand".
    /// </summary>
    /// <param name="index">Index used in itemSlotList at specific itemSlot you want to get.</param>
    private void GetItemSlot(ItemSlot equipmentSlot)
    {
        ItemSlot itemSlot = new ItemSlot(equipmentSlot);
        equipmentSlot.ClearSlot();
        itemInHand.Set(itemSlot, StoredType.Another);

        uiItemInHand.DisplayItemInHand();
    }

    #endregion LOCGIC HANDLER
}
