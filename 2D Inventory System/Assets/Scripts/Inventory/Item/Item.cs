using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class Item : MonoBehaviour, IDroppable, ICollectible, IUseable
{

    #region Properties
    [field: SerializeField]
    public ItemData ItemData { get; private set; }
    [field: SerializeField]
    public Vector2 OffsetPosition { get; private set; }
    [field: SerializeField]
    public float OffsetZAngle { get; private set; }
    #endregion


    [Header("Referecnes")]
    [SerializeField] private GameObject model;
    private SpriteRenderer spriteRenderer;


    public virtual void Start()
    {
        LoadComponents();       
    }

    private void LoadComponents()
    {
        spriteRenderer = model.GetComponent<SpriteRenderer>();
    }


    public void SetData(ItemData itemData)
    {
        this.ItemData = itemData;
        UpdateData();
    }


    public void UpdateData()
    {
        if (spriteRenderer == null)
            LoadComponents();

        spriteRenderer.sprite = ItemData.icon;
    }

    
    public virtual void SetOffsetPosition()
    {
        transform.localPosition = OffsetPosition;
    }

    public virtual void SetOffsetAngle()
    {
        transform.localRotation = Quaternion.Euler(0, 0, OffsetZAngle);
    }

    public virtual void Collect(Player player)
    {
        Vector2 displacment = player.transform.position - this.transform.position;
        float distance = displacment.magnitude;
    }

    public virtual void Drop(Player player)
    {
        
    }

    public virtual void Use(Player player)
    {
       
    }
}
