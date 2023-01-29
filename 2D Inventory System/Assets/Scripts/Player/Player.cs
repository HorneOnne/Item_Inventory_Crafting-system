using MyGame.Ultilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(PlayerInventory))]
[RequireComponent(typeof(ItemInHand))]  
[RequireComponent(typeof(PlayerMovement))]  
[RequireComponent(typeof(PlayerInputHandler))]  
[RequireComponent(typeof(PlayerEquipment))]  
public class Player : MonoBehaviour
{
    #region Properties
    [HideInInspector] public PlayerInventory PlayerInventory { get; private set; }
    [HideInInspector] public ItemInHand ItemInHand { get; private set; }
    [HideInInspector] public PlayerMovement PlayerMovement { get; private set; }
    [HideInInspector] public PlayerInputHandler PlayerInputHandler { get; private set; }
    [HideInInspector] public PlayerEquipment PlayerEquipment { get; private set; }
    #endregion

    public PlayerData playerData;


    private void Awake()
    {
        PlayerInventory = GetComponent<PlayerInventory>();
        ItemInHand= GetComponent<ItemInHand>();
        PlayerMovement = GetComponent<PlayerMovement>();
        PlayerInputHandler = GetComponent<PlayerInputHandler>();
        PlayerEquipment = GetComponent<PlayerEquipment>();
    }


    private void Update()
    {
        /*if (Input.GetMouseButton(0))
        {
            if (itemInHand.HasItem())
            {
                itemInHand.UseItem();
            }
            else
            {
                Debug.Log("Don't have Item");
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (itemInHand.HasItem())
            {
                Drop(itemInHand.itemSlot);
            }

        }*/      
    }

    public void Drop(ItemSlot slot)
    {
        Debug.Log("Drop item called");

        GameObject itemObject = new GameObject();
        itemObject.transform.position = Utilities.GetMousPosition();

        itemObject.name = $"{slot.itemObject.name}";
        itemObject.AddComponent<BoxCollider2D>();
        itemObject.AddComponent<SpriteRenderer>();
        itemObject.AddComponent<Rigidbody2D>();
        itemObject.AddComponent<Item>();

        itemObject.layer = LayerMask.NameToLayer("Item");
        itemObject.tag = "Item";
        itemObject.GetComponent<SpriteRenderer>().sprite = slot.itemObject.icon;

        Debug.Log("Fix here");
        //itemObject.GetComponent<Item>().AddItemData(slot.itemObject);
    }


    
}
