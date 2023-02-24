using UnityEngine;
using System;

namespace DIVH_InventorySystem
{
    /// <summary>
    /// A static class that manages events for different parts of the game.
    /// </summary>
    public static class EventManager
    {
        // Player events
        // ===============================================================
        /// <summary>
        /// Event that gets invoked when the player's inventory is updated.
        /// </summary>
        public static event Action OnPlayerInventoryUpdated;

        /// <summary>
        /// Event that gets invoked when the item in the player's hand is changed.
        /// </summary>
        public static event Action OnItemInHandChanged;

        /// <summary>
        /// Event that gets invoked when the player's equipment is changed.
        /// </summary>
        public static event Action OnPlayerEquipmentChanged;

        // Chest events
        // ===============================================================
        /// <summary>
        /// Event that gets invoked when a chest's inventory is updated.
        /// </summary>
        public static event Action OnChestInventoryUpdated;

        /// <summary>
        /// Event that gets invoked when a chest is opened.
        /// </summary>
        public static event Action OnChestOpened;

        /// <summary>
        /// Event that gets invoked when a chest is closed.
        /// </summary>
        public static event Action OnChestClosed;



        // Crafting table events
        // ===============================================================
        /// <summary>
        /// Event that gets invoked when the crafting table's grid is changed.
        /// </summary>
        public static event Action OnGridChanged;

        /// <summary>
        /// Event that gets invoked when the output item of the crafting table is obtained.
        /// </summary>
        public static event Action OnOutputItemReceived;


        // Anvil (Upgrade items) events
        // ===============================================================
        /// <summary>
        /// Event that gets invoked when the input upgrade item for the anvil is changed.
        /// </summary>
        public static event Action OnInputUpgradeItemChanged;

        /// <summary>
        /// Event that gets invoked when the material input for the anvil upgrade item is changed.
        /// </summary>
        public static event Action OnMaterialInputUpgradeItemChanged;




        public static event Action OnBoomerangReturned;


        public static void TriggerInventoryUpdatedEvent() => OnPlayerInventoryUpdated?.Invoke();

        public static void TriggerChestInventoryUpdatedEvent() => OnChestInventoryUpdated?.Invoke();

        public static void TriggerChestOpenedEvent() => OnChestOpened?.Invoke();

        public static void TriggerChestClosedEvent() => OnChestClosed?.Invoke();

        public static void TriggerItemInHandChangedEvent() => OnItemInHandChanged?.Invoke();

        public static void TriggerPlayerEquipmentChangedEvent() => OnPlayerEquipmentChanged?.Invoke();

        public static void TriggerGridChangedEvent() => OnGridChanged?.Invoke();
        public static void TriggerOutputItemReceivedEvent() => OnOutputItemReceived?.Invoke();

        public static void TriggerInputUpgradeItemChangedEvent() => OnInputUpgradeItemChanged?.Invoke();
        
        public static void TriggerMaterialInputUpgradeItemChangedEvent() => OnMaterialInputUpgradeItemChanged?.Invoke();




        public static void TriggerBoomerangReturnedEvent() => OnBoomerangReturned?.Invoke();


    }
}
