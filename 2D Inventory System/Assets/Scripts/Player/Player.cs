using MyGame.Ultilities;
using UnityEngine;


[RequireComponent(typeof(PlayerInventory))]
[RequireComponent(typeof(ItemInHand))]  
[RequireComponent(typeof(PlayerMovement))]  
[RequireComponent(typeof(PlayerInputHandler))]  
[RequireComponent(typeof(PlayerEquipment))]  
[RequireComponent(typeof(PlayerBattle))]  
public class Player : MonoBehaviour
{
    [Header("Character Body")]
    [SerializeField] Transform handHoldItem;


    [Header("Character Data")]
    public PlayerData playerData;


    #region Properties
    [HideInInspector] public PlayerInventory PlayerInventory { get; private set; }
    [HideInInspector] public ItemInHand ItemInHand { get; private set; }
    [HideInInspector] public PlayerMovement PlayerMovement { get; private set; }
    [HideInInspector] public PlayerInputHandler PlayerInputHandler { get; private set; }
    [HideInInspector] public PlayerEquipment PlayerEquipment { get; private set; }
    [HideInInspector] public PlayerBattle PlayerBattle { get; private set; }
    [HideInInspector] public Transform HandHoldItem { get => handHoldItem; }
    #endregion

    public ChestInventory currentOpenChest;
    


    private void Awake()
    {
        PlayerInventory = GetComponent<PlayerInventory>();
        ItemInHand= GetComponent<ItemInHand>();
        PlayerMovement = GetComponent<PlayerMovement>();
        PlayerInputHandler = GetComponent<PlayerInputHandler>();
        PlayerEquipment = GetComponent<PlayerEquipment>();
        PlayerBattle = GetComponent<PlayerBattle>();
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

        itemObject.name = $"{slot.ItemObject.name}";
        itemObject.AddComponent<BoxCollider2D>();
        itemObject.AddComponent<SpriteRenderer>();
        itemObject.AddComponent<Rigidbody2D>();
        itemObject.AddComponent<Item>();

        itemObject.layer = LayerMask.NameToLayer("Item");
        itemObject.tag = "Item";
        itemObject.GetComponent<SpriteRenderer>().sprite = slot.ItemObject.icon;

        Debug.Log("Fix here");
        //itemObject.GetComponent<Item>().AddItemData(slot.itemObject);
    }

    

    public void SetDefaultAttackSpeed()
    {
        playerData.currentAttackSpeed = playerData.baseAttackSpeed;
    }

    public void IncreaseAttackSpeed(float value)
    {
        playerData.currentAttackSpeed += value;
    }



}
