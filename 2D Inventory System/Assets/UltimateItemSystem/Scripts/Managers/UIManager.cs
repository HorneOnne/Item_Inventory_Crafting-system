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
        [field: SerializeField] public GameObject ItemDescCanvas { get; private set; }
        [field: SerializeField] public GameObject MenuCanvas { get; private set; }


        private void Awake()
        {
            // Activate all UI canvases on awake.
            PlayerInventoryCanvas.SetActive(true);
            CreativeInventoryCanvas.SetActive(true);
            CraftingTableCanvas.SetActive(true);
            PlayerInformationCanvas.SetActive(true);
            PlayerEquipmentCanvas.SetActive(true);
            ChestInventoryCanvas.SetActive(true);
            AnvilCanvas.SetActive(true);
            ItemDescCanvas.SetActive(true);
            MenuCanvas.SetActive(true);
        }
    }
}