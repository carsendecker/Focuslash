using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        public PlayerData()
        {
        }
        
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

    
    
    public string FileName;
    private bool loading;
    
    void Awake()
    {
        if (Services.Save != null)
        {
            Destroy(gameObject);
            return;
        }

        Services.Save = this;
        DontDestroyOnLoad(this);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9))
            SaveGame();
        else if (Input.GetKeyDown(KeyCode.Alpha0) && !loading)
            StartCoroutine(LoadGame());
    }

    public void SaveGame()
    {
        SaveData save = new SaveData();
        PlayerData player = new PlayerData();
        
        player.Position = new SerializableVector2(Services.Player.transform.position.x, Services.Player.transform.position.y);
        player.CurrentFocus = Services.Player.CurrentFocus;
        player.MaxFocus = Services.Player.AttackCount;
        player.MaxHealth = Services.Player.MaxHealth;
        player.CurrentHealth = Services.Player.GetHealth();
        
        save.Player = player;
        
        WriteSaveData(FileName, save);
        print("Saved to " + FileName);
    }
    
    private void WriteSaveData(string fileName, SaveData saveData) {
        var formatter = new BinaryFormatter();  
        var stream = new FileStream(fileName + ".bin", FileMode.Create, FileAccess.Write, FileShare.None);  
        formatter.Serialize(stream, saveData);  
        stream.Close();  
    }

    private AsyncOperation loadingLevel;
    public IEnumerator LoadGame()
    {
        loading = true;
        SaveData save = ReadSaveData(FileName);

        loadingLevel = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        while (!loadingLevel.isDone)
            yield return 0;

        Services.Player.MaxHealth = save.Player.MaxHealth;
        Services.Player.SetHealth(save.Player.CurrentHealth);
        
        Services.Player.AttackCount = save.Player.MaxFocus;
        Services.Player.CurrentFocus = save.Player.CurrentFocus;
        
        Services.Player.transform.position = new Vector2(save.Player.Position.x, save.Player.Position.y);

        print("Loaded from " + FileName);
        loading = false;

    }
    
    private SaveData ReadSaveData(string fileName)
    {
        var formatter = new BinaryFormatter();  
        var stream = new FileStream(fileName + ".bin", FileMode.Open, FileAccess.Read, FileShare.Read);  
        SaveData newSave = (SaveData) formatter.Deserialize(stream);  
        stream.Close();

        return newSave;
    }
}
