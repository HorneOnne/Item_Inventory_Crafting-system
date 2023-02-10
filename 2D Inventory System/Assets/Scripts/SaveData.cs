using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public PlayerInventory PlayerInventoryData { get; set; }    
    

    public SaveData()
    {
        PlayerInventoryData = new PlayerInventory();
    }

    public SaveData CreateSave(PlayerInventory pInventory)
    {
        SaveData saveData = new SaveData();

        for(int i = 0; i < pInventory.capacity; i++)
        {
            PlayerInventoryData.inventory[i] = pInventory.inventory[i]; 
        }

        return saveData;
    }


    public void SaveByJson(PlayerInventory pInventory)
    {
        SaveData saveData = CreateSave(pInventory);
        string jsonString = JsonUtility.ToJson(saveData);

        using(StreamWriter sw = new StreamWriter(Application.dataPath + "/JSONData.txt"))   
        {
            sw.Write(jsonString);
        }

        Debug.Log("Saved");
    }


    public void LoadByJson()
    {
        if(File.Exists(Application.dataPath + "/JSONData.txt"))
        {
            string jsonString = "";
            // LOAD DATA
            using(StreamReader sr = new StreamReader(Application.dataPath + "/JSONData.txt"))
            {
                jsonString = sr.ReadToEnd();

                Debug.Log("Loaded.");
            }
        }
        else
        {
            Debug.Log("NOT FOUND FILE.");
        }
    }
}

