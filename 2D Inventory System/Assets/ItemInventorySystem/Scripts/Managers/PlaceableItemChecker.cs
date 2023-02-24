using UnityEngine;

namespace DIVH_InventorySystem
{

    /// <summary>
    /// Checks whether an item in the player's hand can be placed in the game world.
    /// </summary>
    public class PlaceableItemChecker : Singleton<PlaceableItemChecker>
    {
        [Header("References")]
        public Player player;
        private BoxCollider2D checkerCollider2D;
        private ItemInHand itemInHand;
        private UIItemInHand uiItemInHand;
        private Transform placeableItemContainerParent;

        [Header("Cached")]
        private Vector2 mousePosition;
        private bool isAboveGround;



        #region Properties
        /// <summary>
        /// Gets a value indicating whether the item being placed is colliding with another object.
        /// </summary>
        [field: SerializeField] public bool IsCollideWithOtherObject { get; private set; }

        #endregion




        private void OnEnable()
        {
            EventManager.OnItemInHandChanged += SetBoxCollider2D;
        }

        private void OnDisable()
        {
            EventManager.OnItemInHandChanged -= SetBoxCollider2D;
        }


        private void Start()
        {
            checkerCollider2D = GetComponent<BoxCollider2D>();
            itemInHand = player.ItemInHand;
            uiItemInHand = UIItemInHand.Instance;
            IsCollideWithOtherObject = false;

            placeableItemContainerParent = GameDataManager.Instance.itemContainerParent;
        }




        private void Update()
        {
            if (itemInHand.HasItem() == false) return;
            if (itemInHand.GetICurrenttem() is IPlaceable == false) return;

            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mousePosition;

            isAboveGround = itemInHand.GetICurrenttem().GetComponent<IPlaceable>().IsAboveGround(player);

            if (isAboveGround && IsCollideWithOtherObject == false)
            {
                //Debug.Log("Can be placed");
                uiItemInHand.UISlotImage.color = new Color(0, 1, 0, 1);

                if (Input.GetMouseButtonDown(0))
                {
                    itemInHand.GetICurrenttem().GetComponent<IPlaceable>().Placed(mousePosition, player, placeableItemContainerParent);
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


        /// <summary>
        /// Sets the size and offset of the <see cref="BoxCollider2D"/> based on the size of the item being held by the player.
        /// </summary>
        private void SetBoxCollider2D()
        {
            if (itemInHand.HasItemData())
            {
                var itemObject = GameDataManager.Instance.GetItemPrefab("IP_"+ itemInHand.GetItemData().itemType.ToString());
                if (itemObject == null) return;

                if (itemObject.GetComponent<Item>() is IPlaceable)
                {
                    // Set the size and offset of the collider based on the size of the item
                    checkerCollider2D.size = itemObject.GetComponent<BoxCollider2D>().size;
                    checkerCollider2D.offset = itemObject.GetComponent<BoxCollider2D>().offset;

                    // Scale the size of the collider based on the scale of the item
                    checkerCollider2D.size *= (Vector2)itemObject.transform.localScale;
                }
            }
        }
    }
}
