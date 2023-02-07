using UnityEngine;

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
    [field: SerializeField]
    public GameObject Model { get; private set; }
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
        spriteRenderer = Model.GetComponent<SpriteRenderer>();
    }


    public void SetData(ItemSlot itemSlot)
    {
        this.ItemSlot = itemSlot;
        this.ItemData = itemSlot.ItemObject;
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
        Vector2 displacment = player.transform.position - this.transform.position;
        float distance = displacment.magnitude;
    }

    public virtual void Drop(Player player)
    {
        
    }

    public virtual bool Use(Player player)
    {
        return true;
    }  
}
