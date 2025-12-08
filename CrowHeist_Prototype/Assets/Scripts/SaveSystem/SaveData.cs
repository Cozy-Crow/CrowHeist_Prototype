using System;
using System.Collections.Generic;
using UnityEngine;


/// Main container for all save data

[System.Serializable]
public class SaveData
{
    // Player data
    public PlayerData playerData;

    // Game state
    public int score;
    public int altCoinsScore;
    public string currentSceneName;

    // Items in the scene
    public List<SaveableObjectData> saveableObjects;

    // Timestamp
    public string saveTime;

    public SaveData()
    {
        playerData = new PlayerData();
        saveableObjects = new List<SaveableObjectData>();
        saveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}


[System.Serializable]
public class PlayerData
{
    public Vector3Data position;
    public Vector3Data rotation;
    public bool isFacingRight;
    public bool isDirty;

    
    public string heldObjectId;

    
    public Vector3Data velocity; // Character controller velocity

    public PlayerData()
    {
        position = new Vector3Data();
        rotation = new Vector3Data();
        velocity = new Vector3Data();
        isFacingRight = true;
        isDirty = false;
        heldObjectId = "";
    }
}

[System.Serializable]
public class SaveableObjectData
{
    public string uniqueId;
    public string objectType; // "Coin", "AltCoin", "Pickable", etc.
    public Vector3Data position;
    public Vector3Data rotation;
    public Vector3Data scale;
    public bool isActive;

    public int intValue; // For coin values, etc.
    public float floatValue; // For speeds, durations, etc.
    public bool boolValue; // For states
    public string stringValue; // For tags or special data

 
    public bool customBool1; // For isUsed flag, etc.
    public bool customBool2; // Additional boolean state
    public string customString1; // For custom state data
    public string customString2; // Additional string data

   
    public Vector3Data velocity; // For objects in motion
    public Vector3Data angularVelocity; // For rotating objects
    public bool hadVelocity; // Track if object had velocity when saved
    public bool wasKinematic; // CRITICAL: Save original kinematic state!

    public SaveableObjectData()
    {
        position = new Vector3Data();
        rotation = new Vector3Data();
        scale = new Vector3Data(1, 1, 1);
        velocity = new Vector3Data();
        angularVelocity = new Vector3Data();
        isActive = true;
        hadVelocity = false;
        wasKinematic = false;
    }
}


[System.Serializable]
public class Vector3Data
{
    public float x;
    public float y;
    public float z;

    public Vector3Data()
    {
        x = 0;
        y = 0;
        z = 0;
    }

    public Vector3Data(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3Data(Vector3 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }

    public static implicit operator Vector3(Vector3Data data)
    {
        return data.ToVector3();
    }

    public static implicit operator Vector3Data(Vector3 vector)
    {
        return new Vector3Data(vector);
    }
}