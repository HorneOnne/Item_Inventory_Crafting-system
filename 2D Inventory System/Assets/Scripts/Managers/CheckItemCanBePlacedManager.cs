using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class CheckItemCanBePlacedManager : Singleton<CheckItemCanBePlacedManager>
{
    [Header("References")]
    public Player player;
    private BoxCollider2D checkerCollider2D;
    private ItemInHand itemInHand;
    private UIItemInHand uiItemInHand;
    private Transform itemContainerParent;

    [Header("Cached")]
    private Vector2 mousePosition;
    private bool isAboveGround;



    #region Properties
    [field: SerializeField]
    public bool IsCollideWithOtherObject { get; private set; }

    #endregion




    private void OnEnable()
    {
        ItemInHand.OnItemInHandChanged += SetBoxCollider2D;
    }

    private void OnDisable()
    {
        ItemInHand.OnItemInHandChanged -= SetBoxCollider2D;
    }


    private void Start()
    {
        checkerCollider2D = GetComponent<BoxCollider2D>();
        itemInHand = player.ItemInHand;
        uiItemInHand = UIItemInHand.Instance;
        IsCollideWithOtherObject = false;

        itemContainerParent = ItemContainerManager.Instance.itemContainerParent;
    }




    private void Update()
    {
        if (itemInHand.HasItemObject() == false) return;
        if (itemInHand.GetItemObject() is IPlaceable == false) return;

        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mousePosition;

        isAboveGround = itemInHand.GetItemObject().GetComponent<IPlaceable>().IsAboveGround(player);
        
        if (isAboveGround && IsCollideWithOtherObject == false)
        {
            //Debug.Log("Can be placed");
            uiItemInHand.UISlotImage.color = new Color(0, 1, 0, 1);

            if(Input.GetMouseButtonDown(0))
            {
                itemInHand.GetItemObject().GetComponent<IPlaceable>().Placed(mousePosition, player, itemContainerParent);
            }     
        }
        else
        {
            //Debug.Log("Cannot be placed");
            uiItemInHand.UISlotImage.color = new Color(1, 0, 0, 1);



        }

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        IsCollideWithOtherObject = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IsCollideWithOtherObject = false;
    }


    private void SetBoxCollider2D()
    {
        if (itemInHand.HasItemData())
        {
            var itemObject = ItemContainerManager.Instance.GetItemPrefab(itemInHand.GetItemData().itemType.ToString());
            if (itemObject == null) return;
            
            if (itemObject.GetComponent<Item>() is IPlaceable)
            {
                checkerCollider2D.size = itemObject.GetComponent<BoxCollider2D>().size;
                checkerCollider2D.offset = itemObject.GetComponent<BoxCollider2D>().offset;

                checkerCollider2D.size *= (Vector2)itemObject.transform.localScale;
            }
        }
    }
}
