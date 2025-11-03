using System.Collections.Generic;

[System.Serializable]
public class PlayerDataResponse
{
    public PlayerRecord record;
    public Metadata metadata;
}

[System.Serializable]
public class PlayerRecord
{
    public string playerName;
    public int level;
    public float health;
    public Position position;
    public List<InventoryItem> inventory;
}

[System.Serializable]
public class Position
{
    public float x;
    public float y;
    public float z;
}

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public int quantity;
    public float weight;
}

[System.Serializable]
public class Metadata
{
    public string id;
    public bool @private;
    public string createdAt;
    public string name;
}