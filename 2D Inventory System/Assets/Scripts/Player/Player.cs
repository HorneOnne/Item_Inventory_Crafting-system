using MyGame.Ultilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(PlayerInventory))]
[RequireComponent(typeof(ItemInHand))]  
public class Player : MonoBehaviour
{
    [Header("References")]
    PlayerInventory playerInventory;
    UIInventory ui_inventory;
    ItemInHand itemInHand;
    UIItemInHand ui_itemInHand;
    ItemDictionary itemDictionary;
    UI_DisplayItemDict ui_displayItemDict;





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
