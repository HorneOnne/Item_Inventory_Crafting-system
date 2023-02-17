using UnityEngine;
using UnityEngine.UIElements;

public abstract class Item : MonoBehaviour, IDroppable, ICollectible, IUseable
{
    #region Properties
    [field: SerializeField]
    public ItemData ItemData { get; private set; }
    [field: SerializeField]
    public ItemSlot ItemSlot { get; private set; }
    [field: SerializeField]
    public Vector2 OffsetPosition { get; private set; }
    [field: SerializeField]
    public float OffsetZAngle { get; private set; }
    protected GameObject Model { get; private set; }
    #endregion


    [Header("References")] 
    private SpriteRenderer spriteRenderer;

    [Header("Item Properties")]
    [Tooltip("Indicate this item can be shown when held in hand.")]
    public bool canShow;


    protected virtual void Start()
    {
        LoadComponents();
    }


    /*public void Copy(Item other)
    {
        this.ItemData = other.ItemData;
        this.ItemSlot = other.ItemSlot;
        this.OffsetPosition = other.OffsetPosition;
        this.OffsetZAngle= other.OffsetZAngle;
        this.Model = other.Model;   
        this.spriteRenderer = other.spriteRenderer;
        this.canShow = other.canShow;
    }*/

    private void LoadComponents()
    {
        Model = GetComponentInChildren<SpriteRenderer>().gameObject;
        spriteRenderer = Model.GetComponent<SpriteRenderer>();
    }


    public void SetData(ItemSlot itemSlot)
    {
        this.ItemSlot = itemSlot;
        this.ItemData = itemSlot.ItemData;
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
        Model.transform.localPosition += (Vector3)OffsetPosition;
    }

    public virtual void SetOffsetPosition(Vector3 offsetPosition)
    {
        this.OffsetPosition = offsetPosition;
        Model.transform.localPosition += (Vector3)OffsetPosition;
    }

    public virtual void SetOffsetAngle()
    {
        transform.localRotation = Quaternion.Euler(0, 0, OffsetZAngle);
    }

    public virtual void Collect(Player player)
    {
        
    }

    /*public virtual void Drop(Player player, Vector2 position, Vector3 rotation)
    {
        if (ItemData == null) return;

        var itemPrefab = ItemContainerManager.Instance.GetItemPrefab($"DropItem");
        var itemObject = Instantiate(itemPrefab, position, Quaternion.Euler(rotation), SaveManager.Instance.itemContainerParent);
        itemObject.GetComponent<DropItem>().Set(ItemSlot);
        itemObject.transform.localScale = new Vector3(2, 2, 1);

        player.ItemInHand.RemoveItem();
        UIItemInHand.Instance.DisplayItemInHand();
        Debug.Log(ItemData.itemType);
    }*/

    public virtual void Drop(Player player, Vector2 position, Vector3 rotation)
    {
        var itemInHand = player.ItemInHand;
        if (itemInHand.HasItemData() == false) return;
        var itemSlotDrop = new ItemSlot(itemInHand.GetSlot());
        var itemPrefab = ItemContainerManager.Instance.GetItemPrefab($"DropItem");
        var itemObject = Instantiate(itemPrefab, position, Quaternion.Euler(rotation), SaveManager.Instance.itemContainerParent);

        
        itemObject.GetComponent<DropItem>().Set(itemSlotDrop);
        itemObject.transform.localScale = new Vector3(2, 2, 1);

        itemInHand.ClearSlot();
        UIItemInHand.Instance.UpdateItemInHandUI();
    }



    public virtual bool Use(Player player)
    {
        return true;
    }  
}
