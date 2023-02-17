using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : Item
{

    public void Set(ItemSlot itemSlot)
    {
        base.SetData(itemSlot);
    }


    /*public override void Collect(Player player)
    {
        bool canAddItem = player.PlayerInventory.AddItem(ItemData);

        if(canAddItem)
            Destroy(this.gameObject);
    }*/

    public override void Collect(Player player)
    {
        var returnSlot = player.PlayerInventory.AddItem(ItemSlot);


        if (returnSlot.HasItem() == true)
        {
            Set(returnSlot);    
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
