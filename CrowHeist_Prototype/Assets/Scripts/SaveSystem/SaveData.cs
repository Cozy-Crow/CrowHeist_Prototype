using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    // Player Data
    public Vector3Data playerPosition;
    public QuaternionData playerRotation;
    public bool playerIsDirty;
    public string heldObjectID; // ID of the object the player is holding, empty if none

    // Score Data
    public int score;
    public int altCoinsScore;

    // Roomba Data
    public RoombaData roombaData;

    // Items Data
    public List<ItemData> items = new List<ItemData>();

    // Timestamp
    public string saveTimestamp;
}

[System.Serializable]
public class Vector3Data
{
    public float x;
    public float y;
    public float z;

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
}

[System.Serializable]
public class QuaternionData
{
    public float x;
    public float y;
    public float z;
    public float w;

    public QuaternionData(Quaternion quaternion)
    {
        x = quaternion.x;
        y = quaternion.y;
        z = quaternion.z;
        w = quaternion.w;
    }

    public Quaternion ToQuaternion()
    {
        return new Quaternion(x, y, z, w);
    }
}

[System.Serializable]
public class RoombaData
{
    public Vector3Data position;
    public bool isActivated;
    public bool isBroken;
    // Note: isDocked is private in RoombAi and cannot be accessed/saved

    public RoombaData()
    {
        position = new Vector3Data(Vector3.zero);
        isActivated = false;
        isBroken = false;
    }
}

[System.Serializable]
public class ItemData
{
    public string uniqueID;
    public string itemType; // Type of the item (e.g., "Coin", "Soda", etc.)
    public Vector3Data position;
    public Vector3Data rotation;
    public bool isDirty;
    public bool isHeld; // Is this item currently held by player
    public bool isActive; // Is this item active in the scene

    public ItemData(string id, string type, Vector3 pos, Vector3 rot, bool dirty, bool held, bool active)
    {
        uniqueID = id;
        itemType = type;
        position = new Vector3Data(pos);
        rotation = new Vector3Data(rot);
        isDirty = dirty;
        isHeld = held;
        isActive = active;
    }
}
