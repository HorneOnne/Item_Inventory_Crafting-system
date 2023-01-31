using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : Item, IUpgradeable
{
    public int CurrentLevel { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public int MaxLevel { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }


    public override void Start()
    {
        base.Start();
        base.SetOffsetPosition();
    }

   

    public bool CanUpgrade()
    {
        //throw new System.NotImplementedException();
        return false;
    }

    public bool IsMaxLevel()
    {
        //throw new System.NotImplementedException();
        return false;
    }


    public override void Use(Player player)
    {
        PlayerInventory playerInventory = player.PlayerInventory;
        var arrowSlotIndex = playerInventory.FindArrowSlotIndex();

        if(arrowSlotIndex != null) 
        {
            /*Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 releaseDirection = mousePosition - (Vector2)player.transform.position;

            var arrowObject = Instantiate(prefab, player.transform.position, Quaternion.identity);
            arrowObject.AddComponent<Rigidbody2D>().velocity = releaseDirection * 5;
            Destroy(arrowObject, 2f);*/

            GameObject arrowPrefab = ItemContainerManager.Instance.GetItemPrefab("Arrow");
            if(arrowPrefab != null)
            {
                GameObject arrowObject = Instantiate(arrowPrefab, player.transform.position, player.HandPart.rotation);
                arrowObject.GetComponent<Item>().SetData(playerInventory.inventory[(int)arrowSlotIndex].itemObject);   
                arrowObject.GetComponent<Item>().Use(player);   
            }

        }
    }


    public void Upgrade()
    {
        //throw new System.NotImplementedException();
    }

    
}
