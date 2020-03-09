using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    
    [Serializable]
    public class SaveData
    {
        public PlayerData Player;
        public Dictionary<GameObject, bool> ClearedRooms;
    }

    [Serializable]
    public class PlayerData
    {
        public PlayerData() {}

        public SerializableVector2 Position;
        public int MaxHealth;
        public float CurrentHealth;
        public int MaxFocus;
        public float CurrentFocus;

    }

    [Serializable]
    public class SerializableVector2
    {
        public SerializableVector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        public float x, y;
    }
    
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9))
            SaveGame();
        else if (Input.GetKeyDown(KeyCode.Alpha0))
            LoadGame();
    }

    void SaveGame()
    {
        SaveData save = new SaveData();
        PlayerData player = new PlayerData();

        player.Position.x = Services.Player.transform.position.x;
        player.Position.y = Services.Player.transform.position.y;
        player.CurrentFocus = Services.Player.CurrentFocus;
        player.MaxFocus = Services.Player.AttackCount;
        player.MaxHealth = Services.Player.MaxHealth;
        player.CurrentHealth = Services.Player.GetHealth();
        
        save.Player = player;
        
        WriteSaveData("TestPlayerSave", save);
        
    }
    
    public void WriteSaveData(string fileName, SaveData saveData) {
        var formatter = new BinaryFormatter();  
        var stream = new FileStream(fileName + ".bin", FileMode.Create, FileAccess.Write, FileShare.None);  
        formatter.Serialize(stream, saveData);  
        stream.Close();  
    }

    void LoadGame()
    {
        
    }
}
