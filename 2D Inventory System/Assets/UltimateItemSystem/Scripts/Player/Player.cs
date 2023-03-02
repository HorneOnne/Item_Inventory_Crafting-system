using UnityEngine;


namespace UltimateItemSystem
{
    /// <summary>
    /// Class that represents a player character.
    /// </summary>
    [RequireComponent(typeof(PlayerInventory))]
    [RequireComponent(typeof(ItemInHand))]
    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(PlayerInputHandler))]
    [RequireComponent(typeof(PlayerEquipment))]
    public class Player : MonoBehaviour
    {
        [Header("CHARACTER REFERENCES")]
        [SerializeField] Transform handHoldItem;


        [Header("CHARACTER DATA")]
        public PlayerData playerData;


        #region Properties
        [HideInInspector] public PlayerInventory PlayerInventory { get; private set; }
        [HideInInspector] public ItemInHand ItemInHand { get; private set; }
        [HideInInspector] public PlayerMovement PlayerMovement { get; private set; }
        [HideInInspector] public PlayerInputHandler PlayerInputHandler { get; private set; }
        [HideInInspector] public PlayerEquipment PlayerEquipment { get; private set; }
        [HideInInspector] public Transform HandHoldItem { get => handHoldItem; }
        #endregion

        /// <summary>
        /// The chest currently open by the player.
        /// </summary>
        public Chest currentOpenChest;



        private void Awake()
        {
            PlayerInventory = GetComponent<PlayerInventory>();
            ItemInHand = GetComponent<ItemInHand>();
            PlayerMovement = GetComponent<PlayerMovement>();
            PlayerInputHandler = GetComponent<PlayerInputHandler>();
            PlayerEquipment = GetComponent<PlayerEquipment>();
        }


       
    }
}