using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class SerializableVector
{
    public float x;
    public float y;
    public float z;

    public SerializableVector(Vector3 position)
    {
        x = position.x;
        y = position.y;
        z = position.z;
    }

    public Vector3 GetVector3()
    {
        return new Vector3(x, y, z);
    }

    //public static implicit operator SerealizableVector(Vector3 inVector)
    //{
    //    return new SerealizableVector(inVector);
    //}
}

[Serializable]
public struct PlayerData 
{
    public SerializableVector playerPosition;
}

public class SaveManager : MonoBehaviour
{
    PlayerSpawn playerSpawn;

    void Awake()
    {
        playerSpawn = GetComponent<PlayerSpawn>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Save();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            Load();
        }
    }

    private void Save()
    {
        // Create a platform independent file path
        string saveDirectory = Application.persistentDataPath;
        string fileName = "playerData.sav";
        string filePath = Path.Combine(saveDirectory,fileName);

        // Create amd fill with data object to save
        PlayerData playerData = new();
        Vector3 playerPosition = playerSpawn.spawnedPlayer.transform.position;
        playerData.playerPosition = new SerializableVector(playerPosition);

        // Serialize object into the byte buffer
        BinaryFormatter binaryFormat = new();
        using MemoryStream memoryStream = new();
        binaryFormat.Serialize(memoryStream, playerData);
        memoryStream.Seek(0, SeekOrigin.Begin);
        byte[] saveBuffer = memoryStream.ToArray();

        // Write bytes into the file
        File.WriteAllBytes(filePath, saveBuffer);
    }

    private void Load()
    {
        // Create a platform independent file path
        string saveDirectory = Application.persistentDataPath;
        string fileName = "playerData.sav";
        string filePath = Path.Combine(saveDirectory, fileName);

        // Load byte buffer from file
        // TODO - Handle it if loading buffer fails or file doesn't exist 
        byte[] saveBuffer = File.ReadAllBytes(filePath);

        // Ten co umi serealizovat a desearizovat
        BinaryFormatter binaryFormat = new();

        // Objekt ktery drzi byty
        using MemoryStream memoryStream = new(saveBuffer);

        // Prevedu / deserializuje byte buffer do objektu
        object saveObject = binaryFormat.Deserialize(memoryStream);

        // Prevedu si playerData na objekt
        PlayerData playerData = (PlayerData)saveObject;

        playerSpawn.spawnedPlayer.transform.position = playerData.playerPosition.GetVector3();
    }

    //private void Save()
    //{
    //    string saveDirectory = Application.persistentDataPath;
    //    string playerData = "playerData.txt";
    //    string filePath = Path.Combine(saveDirectory,playerData);

    //    Vector3 playerPosition = playerSpawn.spawnedPlayer.transform.position;

    //    File.WriteAllText(filePath, playerPosition.ToString());
    //}

    //private void Load()
    //{

    //    string saveDirectory = Application.persistentDataPath;
    //    string playerData = "playerData.txt";
    //    string filePath = Path.Combine(saveDirectory, playerData);


    //    string saveData = File.ReadAllText(filePath);

    //    saveData = saveData.Replace(" ", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty);

    //    string[] substrings = saveData.Split(",");
    //    Vector3 playerPosition;
    //    playerPosition.x = float.Parse(substrings[0]);
    //    playerPosition.y = float.Parse(substrings[1]);
    //    playerPosition.z = float.Parse(substrings[2]);
    //    playerSpawn.spawnedPlayer.transform.position = playerPosition;
    //}
}
