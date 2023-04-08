using UnityEngine;

namespace UltimateItemSystem
{
    /// <summary>
    /// The UIManager class manages all the UI canvas in the game and provides easy access to them through properties.
    /// </summary>
    public class UIManager : Singleton<UIManager>
    {
        [field: SerializeField] public GameObject PlayerInventoryCanvas { get; private set; }
        [field: SerializeField] public GameObject CreativeInventoryCanvas { get; private set; }
        [field: SerializeField] public GameObject CraftingTableCanvas { get; private set; }
        [field: SerializeField] public GameObject PlayerInformationCanvas { get; private set; }
        [field: SerializeField] public GameObject PlayerEquipmentCanvas { get; private set; }
        [field: SerializeField] public GameObject ChestInventoryCanvas { get; private set; }
        [field: SerializeField] public GameObject AnvilCanvas { get; private set; }
        [field: SerializeField] public GameObject MenuCanvas { get; private set; }


        private void Awake()
        {
            // Activate all UI canvases on awake.
            if(PlayerInventoryCanvas != null)
                PlayerInventoryCanvas.SetActive(true);

            if (CreativeInventoryCanvas != null)
                CreativeInventoryCanvas.SetActive(true);

            if (CraftingTableCanvas != null)
                CraftingTableCanvas.SetActive(true);

            if (PlayerInformationCanvas != null)
                PlayerInformationCanvas.SetActive(true);

            if (PlayerEquipmentCanvas != null)
                PlayerEquipmentCanvas.SetActive(true);

            if (ChestInventoryCanvas != null)
                ChestInventoryCanvas.SetActive(true);

            if (AnvilCanvas != null)
                AnvilCanvas.SetActive(true);

            if (MenuCanvas != null)
                MenuCanvas.SetActive(true);
        }
    }
}