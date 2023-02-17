using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [field: SerializeField] public GameObject PlayerInventoryCanvas { get; private set; }
    [field: SerializeField] public GameObject CreativeInventoryCanvas { get; private set; }
    [field: SerializeField] public GameObject CraftingTableCanvas { get; private set; }
    [field: SerializeField] public GameObject ItemInHandCanvas { get; private set; }
    [field: SerializeField] public GameObject PlayerEquipmentCanvas { get; private set; }
    [field: SerializeField] public GameObject ChestInventoryCanvas { get; private set; }
    [field: SerializeField] public GameObject AnvilCanvas { get; private set; }


    private void Awake()
    {
        PlayerInventoryCanvas.SetActive(true);
        CreativeInventoryCanvas.SetActive(true);
        CraftingTableCanvas.SetActive(true);
        ItemInHandCanvas.SetActive(true);
        PlayerEquipmentCanvas.SetActive(true);
        ChestInventoryCanvas.SetActive(true);
        AnvilCanvas.SetActive(true);
    }


}
