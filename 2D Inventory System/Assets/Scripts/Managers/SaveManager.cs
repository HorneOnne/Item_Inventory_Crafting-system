using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public SaveData saveData;
    public PlayerInventory playerInventory;


    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            saveData = saveData.CreateSave(playerInventory);
            saveData.SaveByJson(playerInventory);
        }

        if(Input.GetKeyDown(KeyCode.L)) 
        {
            saveData.LoadByJson();
        }
    }


}
