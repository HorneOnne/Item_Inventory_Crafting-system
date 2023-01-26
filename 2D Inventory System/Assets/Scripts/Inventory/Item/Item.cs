using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))] 
[RequireComponent(typeof(SpriteRenderer))] 
public abstract class Item : MonoBehaviour, IDroppable, ICollectible, IUseable
{
    public ItemData itemData;


    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;

    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public virtual void Start()
    {
        
    }


    public void UpdateData()
    {
        spriteRenderer.sprite = itemData.icon;
    }

    public void AddItemData(ItemData itemData)
    {
        this.itemData = itemData;
    }

    public virtual void Collect(Player player)
    {
        Vector2 displacment = player.transform.position - this.transform.position;
        float distance = displacment.magnitude;

        rb.velocity = displacment * 20;
    }

    public virtual void Drop(Player player)
    {
        
    }

    public virtual void Use(Player player)
    {
        
    }
}
