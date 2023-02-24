using UnityEngine;

namespace DIVH_InventorySystem
{
    [RequireComponent(typeof(ChestInventory))]
    public class Chest : Item, IPlaceable
    {
        [Header("References")]
        private GameObject uiChestInventoryCanvas;
        private Animator anim;
        private ChestInventory chestInventory;


        public enum ChestStateEnum
        {
            Placed,
            OpenClose,
        }


        [Header("Chest Properties")]
        private bool isOpen;


        #region Properties
        [field: SerializeField]
        public bool ShowRay { get; set; }
        [field: SerializeField]
        public LayerMask PlacedLayer { get; set; }
        public ChestStateEnum ChestState { get; private set; }
        public ChestInventory Inventory { get => chestInventory; }

        #endregion




        protected override void Start()
        {
            base.Start();
            anim = base.Model.GetComponent<Animator>();
            isOpen = false;
            chestInventory = GetComponent<ChestInventory>();
            uiChestInventoryCanvas = UIManager.Instance.ChestInventoryCanvas;
        }


        public bool IsAboveGround(Player player, bool showRay = false)
        {
            bool canBePlaced = false;
            RaycastHit2D hit = Physics2D.Raycast(UIItemInHand.Instance.uiSlotDisplay.transform.position, Vector2.down, 2.0f, PlacedLayer);

            if (showRay)
                Debug.DrawRay(UIItemInHand.Instance.uiSlotDisplay.transform.position, Vector2.down * 2.0f, Color.blue, 1);

            if (hit.collider != null)
            {
                canBePlaced = true;
            }

            return canBePlaced;
        }

        public void Placed(Vector3 placedPosition, Player player = null, Transform parent = null)
        {
            ChestState = ChestStateEnum.Placed;
            base.spriteRenderer.enabled = true;

            Vector3 cachedLocalScale = transform.localScale;

            if (parent != null)
                transform.parent = parent.transform;

            gameObject.SetActive(true);
            transform.position = placedPosition;
            transform.localScale = cachedLocalScale;
            transform.localRotation = Quaternion.Euler(0, 0, 0);

            player.ItemInHand.RemoveItem();
            UIItemInHand.Instance.UpdateItemInHandUI();
        }




        private void ShowChestInventoryUI()
        {
            uiChestInventoryCanvas.SetActive(true);
        }

        private void HideChestInventoryUI()
        {
            uiChestInventoryCanvas.SetActive(false);
        }



        /// <summary>
        /// Toggle open and close this chest.
        /// IF this chest is already open -> close.
        /// IF this chest is already close -> open.
        /// </summary>
        /// <param name="player"></param>
        public void Toggle(Player player)
        {
            if (ChestState == ChestStateEnum.Placed)
            {
                ChestState = ChestStateEnum.OpenClose;
                return;
            }

            isOpen = !isOpen;


            if (isOpen)
            {
                Open(player, forceOpenUI: true);

            }
            else
            {
                Close(player, forceCloseUI: true);
            }

            anim.SetBool("isOpen", isOpen);
        }

        /// <summary>
        /// Open chest.
        /// </summary>
        /// <param name="player">Player who opens this chest.</param>
        /// <param name="forceOpenUI"></param>
        public void Open(Player player, bool forceOpenUI = false)
        {
            if (forceOpenUI)
                ShowChestInventoryUI();

            if (ChestState == ChestStateEnum.Placed)
            {
                ChestState = ChestStateEnum.OpenClose;
                return;
            }

            isOpen = true;

            player.currentOpenChest = this;
            this.chestInventory.Set(player);
            EventManager.TriggerChestOpenedEvent();
            UIChestInventory.Instance.SetChestInventoryData(chestInventory);

            anim.SetBool("isOpen", isOpen);
        }


        /// <summary>
        /// Close chest.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="forceCloseUI">Player who closes this chest.</param>
        public void Close(Player player, bool forceCloseUI = false)
        {
            if (forceCloseUI)
                HideChestInventoryUI();

            if (ChestState == ChestStateEnum.Placed)
            {
                ChestState = ChestStateEnum.OpenClose;
                return;
            }

            isOpen = false;

            player.currentOpenChest = null;
            this.chestInventory.Set(null);
            EventManager.TriggerChestClosedEvent();
            UIChestInventory.Instance.RemoveChestInventoryData();

            anim.SetBool("isOpen", isOpen);
        }
    }
}


